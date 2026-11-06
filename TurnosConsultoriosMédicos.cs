using System.Data;
using System.Drawing.Printing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Clínica_SePrice.vistas;

namespace Clínica_SePrice
{
    public partial class TurnosConsultoriosMédicos : Form
    {
        private int? idPaciente;
        private long? idTurnoGuardado;
        private string dniSeleccionado = "";
        private bool cargando;
        private readonly TextBox txtDni = new()
        {
            Name = "txtDni", Location = new Point(466, 230), Width = 265, MaxLength = 20
        };
        private readonly Label lblPaciente = new()
        {
            Location = new Point(466, 310), Size = new Size(285, 45)
        };
        private readonly Button btnBuscarPaciente = new()
        {
            Text = "Buscar / Registrar paciente", Location = new Point(466, 265), Size = new Size(265, 30)
        };
        private readonly ComboBox cmbEspecialidad = new()
        {
            Location = new Point(34, 230), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly ComboBox cmbAgenda = new()
        {
            Location = new Point(34, 435), Width = 730, DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly CheckBox chkSobreturno = new()
        {
            Text = "Buscar sobreturno (máximo uno por hora)",
            Location = new Point(325, 480), Size = new Size(300, 27)
        };
        private readonly Label lblAviso = new()
        {
            Location = new Point(34, 530), Size = new Size(730, 45), ForeColor = Color.Firebrick
        };
        private readonly Button btnComprobante = new()
        {
            Text = "Ver comprobante", Location = new Point(34, 590), Size = new Size(140, 27), Enabled = false
        };
        private readonly Button btnImprimir = new()
        {
            Text = "Imprimir / PDF", Location = new Point(185, 590), Size = new Size(140, 27), Enabled = false
        };

        public TurnosConsultoriosMédicos()
        {
            InitializeComponent();
            ClientSize = new Size(800, 635);
            label2.Location = new Point(34, 203);
            foreach (RadioButton opcion in new[] { Cardiología, Dermatología, Gastroenterología, Neumonología, Neurología, Guardia })
                opcion.Visible = false;
            btnAceptar.Location = new Point(340, 590);
            button2.Location = new Point(490, 590);
            comboBox1.Location = new Point(135, 482);
            comboBox1.Size = new Size(180, 23);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Visible = true;
            var actualizar = new Button
            {
                Text = "Actualizar horarios", Location = new Point(625, 480), Size = new Size(140, 27)
            };
            actualizar.Click += (_, _) =>
            {
                if (cmbAgenda.SelectedValue is int)
                    CargarHorarios();
                else
                    CargarAgendas();
            };
            Controls.AddRange(new Control[]
            {
                new Label { Text = "DNI del paciente", Location = new Point(466, 203), AutoSize = true },
                new Label { Text = "Día de la reserva", Location = new Point(215, 180), AutoSize = true },
                new Label { Text = "Profesional y consultorio", Location = new Point(34, 405), AutoSize = true },
                new Label { Text = "Horario", Location = new Point(34, 485), AutoSize = true },
                txtDni, lblPaciente, btnBuscarPaciente, cmbEspecialidad, cmbAgenda,
                chkSobreturno, actualizar, lblAviso, btnComprobante, btnImprimir
            });
            btnBuscarPaciente.Click += (_, _) => BuscarPaciente();
            txtDni.TextChanged += (_, _) =>
            {
                idPaciente = null;
                dniSeleccionado = "";
                lblPaciente.Text = "";
            };
            cmbEspecialidad.SelectedIndexChanged += (_, _) => CargarAgendas();
            monthCalendar1.DateChanged += (_, _) => CargarAgendas();
            cmbAgenda.SelectedIndexChanged += (_, _) => CargarHorarios();
            chkSobreturno.CheckedChanged += (_, _) => CargarHorarios();
            btnComprobante.Click += (_, _) => MostrarComprobante(imprimir: false);
            btnImprimir.Click += (_, _) => MostrarComprobante(imprimir: true);
            button3.Click += (_, _) => Close();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var adaptador = new MySqlDataAdapter("""
                    SELECT DISTINCT TRIM(m.especialidad) AS especialidad
                    FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                    WHERE u.rol = 'medico' AND TRIM(COALESCE(m.especialidad, '')) <> ''
                    ORDER BY especialidad
                    """, conexion);
                var especialidades = new DataTable();
                adaptador.Fill(especialidades);
                cargando = true;
                cmbEspecialidad.DisplayMember = "especialidad";
                cmbEspecialidad.ValueMember = "especialidad";
                cmbEspecialidad.DataSource = especialidades;
                cmbEspecialidad.SelectedIndex = -1;
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudieron cargar las especialidades. Revisá la conexión.";
            }
            finally
            {
                cargando = false;
            }
        }

        private void CargarAgendas()
        {
            if (cargando || idTurnoGuardado.HasValue)
                return;
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Text = "";
            cmbAgenda.DataSource = null;
            comboBox1.DataSource = null;
            if (cmbEspecialidad.SelectedValue is not string especialidad)
                return;
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var comando = new MySqlCommand("""
                    SELECT a.id_agenda,
                           CONCAT(u.apellido, ', ', u.nombre, ' — ', c.nombre, ' — ',
                                  TIME_FORMAT(a.hora_inicio, '%H:%i'), ' a ',
                                  TIME_FORMAT(a.hora_fin, '%H:%i'), ' — ',
                                  a.duracion_minutos, ' min') AS descripcion
                    FROM agendas_consultorios a
                    JOIN medicos m ON m.id_usuario = a.id_medico
                    JOIN usuarios u ON u.id_usuario = m.id_usuario
                    JOIN consultorios c ON c.id_consultorio = a.id_consultorio
                    WHERE a.fecha = @fecha AND TRIM(m.especialidad) = @especialidad
                      AND c.activo = TRUE AND u.rol = 'medico'
                    ORDER BY u.apellido, u.nombre, a.hora_inicio, a.id_agenda
                    """, conexion);
                comando.Parameters.AddWithValue("@fecha", monthCalendar1.SelectionStart.Date);
                comando.Parameters.AddWithValue("@especialidad", especialidad);
                using var adaptador = new MySqlDataAdapter(comando);
                var agendas = new DataTable();
                adaptador.Fill(agendas);
                cargando = true;
                cmbAgenda.DisplayMember = "descripcion";
                cmbAgenda.ValueMember = "id_agenda";
                cmbAgenda.DataSource = agendas;
                cmbAgenda.SelectedIndex = -1;
                if (agendas.Rows.Count == 0)
                    lblAviso.Text = "No hay agendas para esa especialidad y fecha. Configurá una agenda o elegí otro día.";
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudieron consultar las agendas. Revisá la conexión y el script 09.";
            }
            finally
            {
                cargando = false;
            }
        }

        private void CargarHorarios()
        {
            if (cargando || idTurnoGuardado.HasValue)
                return;
            comboBox1.DataSource = null;
            if (cmbAgenda.SelectedValue is not int agenda)
                return;
            lblAviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                DataTable horarios = ServicioReservasConsultorios.ConsultarHorarios(
                    conexion, agenda, chkSobreturno.Checked);
                comboBox1.DisplayMember = "Descripcion";
                comboBox1.ValueMember = "Hora";
                comboBox1.DataSource = horarios;
                comboBox1.SelectedIndex = -1;
                if (horarios.Rows.Count == 0)
                    lblAviso.Text = "No hay horarios disponibles para la opción seleccionada.";
            }
            catch (InvalidOperationException ex)
            {
                lblAviso.Text = ex.Message;
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudieron cargar los horarios. Revisá la conexión y el script 09.";
            }
        }

        private void BuscarPaciente()
        {
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Text = "";
            string dni = txtDni.Text.Trim();
            if (dni.Length == 0 || !dni.All(caracter => caracter >= '0' && caracter <= '9'))
            {
                lblAviso.Text = "Ingresá el DNI con números, sin puntos ni espacios.";
                txtDni.Focus();
                return;
            }
            using var ficha = new VistaFichaPaciente(dni);
            if (ficha.ShowDialog(this) == DialogResult.OK)
            {
                idPaciente = ficha.IdPaciente;
                dniSeleccionado = dni;
                lblPaciente.Text = $"Paciente N.º {ficha.IdPaciente}\n{ficha.NombreCompleto}";
            }
        }

        private void btnAceptar_Click(object? sender, EventArgs e)
        {
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Text = "";
            string dni = txtDni.Text.Trim();
            if (!idPaciente.HasValue || dniSeleccionado != dni)
            {
                lblAviso.Text = "Buscá o registrá al paciente antes de guardar la reserva.";
                return;
            }
            if (cmbAgenda.SelectedValue is not int agenda || comboBox1.SelectedValue is not TimeSpan hora)
            {
                lblAviso.Text = "Seleccioná especialidad, fecha, profesional y un horario disponible.";
                return;
            }
            try
            {
                idTurnoGuardado = ServicioReservasConsultorios.Reservar(
                    idPaciente.Value, dni, agenda, hora, chkSobreturno.Checked);
                btnAceptar.Enabled = false;
                txtDni.ReadOnly = true;
                btnBuscarPaciente.Enabled = false;
                monthCalendar1.Enabled = false;
                cmbEspecialidad.Enabled = false;
                cmbAgenda.Enabled = false;
                comboBox1.Enabled = false;
                chkSobreturno.Enabled = false;
                btnComprobante.Enabled = true;
                btnImprimir.Enabled = true;
                lblAviso.ForeColor = Color.DarkGreen;
                lblAviso.Text = $"Turno N.º {idTurnoGuardado} reservado para el {monthCalendar1.SelectionStart:dd/MM/yyyy} a las {hora.ToString("hh':'mm")}.";
            }
            catch (InvalidOperationException ex)
            {
                lblAviso.Text = ex.Message;
            }
            catch (MySqlException ex) when (ex.Number == 1054 || ex.Number == 1146)
            {
                lblAviso.Text = "Falta actualizar la base. Ejecutá los scripts 08, 09 y 10.";
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudo confirmar la reserva. Actualizá los horarios antes de reintentar.";
            }
        }

        private void button2_Click(object? sender, EventArgs e)
        {
            idTurnoGuardado = null;
            txtDni.Clear();
            txtDni.ReadOnly = false;
            btnBuscarPaciente.Enabled = true;
            monthCalendar1.MinDate = DateTime.Today;
            monthCalendar1.Enabled = true;
            cmbEspecialidad.Enabled = true;
            cmbAgenda.Enabled = true;
            comboBox1.Enabled = true;
            chkSobreturno.Enabled = true;
            btnAceptar.Enabled = true;
            btnComprobante.Enabled = false;
            btnImprimir.Enabled = false;
            chkSobreturno.Checked = false;
            lblAviso.Text = "";
            CargarAgendas();
            txtDni.Focus();
        }

        private void MostrarComprobante(bool imprimir)
        {
            if (!idTurnoGuardado.HasValue)
                return;
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var comando = new MySqlCommand("""
                    SELECT t.id_turno, t.fecha, t.hora, t.especialidad, t.dni_paciente, t.estado,
                           CONCAT(p.apellido, ', ', p.nombre) AS paciente,
                           CONCAT(u.apellido, ', ', u.nombre) AS medico,
                           c.nombre AS consultorio, r.es_sobreturno
                    FROM `turnosConsultoriosMédicos` t
                    JOIN pacientes p ON p.dni = t.dni_paciente
                    JOIN reservas_agenda_consultorios r ON r.id_turno = t.id_turno
                    JOIN agendas_consultorios a ON a.id_agenda = r.id_agenda
                    JOIN usuarios u ON u.id_usuario = a.id_medico
                    JOIN consultorios c ON c.id_consultorio = a.id_consultorio
                    WHERE t.id_turno = @id
                    """, conexion);
                comando.Parameters.AddWithValue("@id", idTurnoGuardado.Value);
                using var adaptador = new MySqlDataAdapter(comando);
                using var datos = new DataTable();
                adaptador.Fill(datos);
                if (datos.Rows.Count == 0)
                {
                    lblAviso.Text = "No se encontraron los datos del turno.";
                    return;
                }
                using var documento = new DocumentoTurnoConsultorio(datos.Rows[0]);
                if (!documento.PrinterSettings.IsValid)
                {
                    lblAviso.Text = "Configurá una impresora o Microsoft Print to PDF en Windows.";
                    return;
                }
                if (imprimir)
                {
                    using var dialogo = new PrintDialog { Document = documento, UseEXDialog = true };
                    if (dialogo.ShowDialog(this) == DialogResult.OK)
                        documento.Print();
                }
                else
                {
                    using var vistaPrevia = new PrintPreviewDialog
                    {
                        Document = documento, Width = 1000, Height = 750,
                        StartPosition = FormStartPosition.CenterParent
                    };
                    vistaPrevia.ShowDialog(this);
                }
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudieron consultar los datos del comprobante.";
            }
            catch (InvalidPrinterException)
            {
                lblAviso.Text = "La impresora seleccionada no está disponible.";
            }
            catch (System.ComponentModel.Win32Exception)
            {
                lblAviso.Text = "No se pudo abrir la impresión. Revisá las impresoras de Windows.";
            }
        }

        private void radioButton1_CheckedChanged(object? sender, EventArgs e) { }
        private void label2_Click(object? sender, EventArgs e) { }
    }
}
