using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using MySql.Data.MySqlClient;
using Clínica_SePrice.vistas;

namespace Clínica_SePrice
{
    public partial class Estudios_Clínicos : Form
    {
        private int? idPaciente;
        private long? idTurnoGuardado;
        private string dniSeleccionado = "";
        private readonly TextBox txtDni = new()
        {
            Location = new Point(487, 305), Width = 280, MaxLength = 20
        };
        private readonly Button buscarPaciente = new()
        {
            Text = "Buscar / Registrar paciente", Location = new Point(487, 340), Size = new Size(280, 30)
        };
        private readonly Label paciente = new()
        {
            Location = new Point(487, 385), Size = new Size(280, 50)
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 460), Size = new Size(750, 55), ForeColor = Color.Firebrick
        };
        private readonly Button verComprobante = new()
        {
            Text = "Ver comprobante", Location = new Point(20, 550), Size = new Size(150, 30), Enabled = false
        };
        private readonly Button imprimir = new()
        {
            Text = "Imprimir / PDF", Location = new Point(185, 550), Size = new Size(150, 30), Enabled = false
        };

        public Estudios_Clínicos()
        {
            InitializeComponent();
            ClientSize = new Size(800, 610);
            label1.AutoSize = false;
            label1.Location = new Point(20, 155);
            label1.Size = new Size(750, 30);
            label1.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label2.AutoSize = false;
            label2.Location = new Point(20, 190);
            label2.Size = new Size(750, 25);
            monthCalendar1.MaxSelectionCount = 1;
            monthCalendar1.MinDate = DateTime.Today;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Size = new Size(280, 23);
            Atrás.Location = new Point(12, 12);
            Atrás.Size = new Size(75, 23);
            Atrás.Text = "Volver";
            Atrás.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            Atrás.BackColor = Color.Teal;
            Atrás.ForeColor = SystemColors.ControlLight;
            Atrás.UseVisualStyleBackColor = false;
            Aceptar.Location = new Point(380, 550);
            Aceptar.Size = new Size(175, 30);
            Aceptar.Text = "Guardar reserva";
            Cancelar.Location = new Point(570, 550);
            Cancelar.Size = new Size(175, 30);
            Cancelar.Text = "Nueva reserva";
            Controls.AddRange(new Control[]
            {
                new Label { Text = "DNI del paciente", Location = new Point(487, 280), AutoSize = true },
                new Label { Text = "Horario disponible", Location = new Point(487, 215), AutoSize = true },
                txtDni, buscarPaciente, paciente, aviso, verComprobante, imprimir
            });
            foreach (RadioButton estudio in new[] { Laboratorio, Electrocardiograma, Ecografía, Radiografía, Tomografía })
                estudio.CheckedChanged += Estudio_CheckedChanged;
            monthCalendar1.DateChanged += (_, _) => CargarHorariosDisponibles();
            buscarPaciente.Click += (_, _) => BuscarPaciente();
            txtDni.TextChanged += (_, _) =>
            {
                idPaciente = null;
                dniSeleccionado = "";
                paciente.Text = "";
            };
            verComprobante.Click += (_, _) => MostrarComprobante(imprimirDocumento: false);
            imprimir.Click += (_, _) => MostrarComprobante(imprimirDocumento: true);
            Load += Estudios_Clínicos_Load;
        }

        private string EstudioSeleccionado() =>
            new[] { Laboratorio, Electrocardiograma, Ecografía, Radiografía, Tomografía }
                .FirstOrDefault(estudio => estudio.Checked)?.Text.Trim() ?? "";

        private void Estudios_Clínicos_Load(object? sender, EventArgs e)
        {
            label1.Text = "Reserva de estudios clínicos";
            label2.Text = "Seleccioná el estudio y la fecha.";
        }

        private void Estudio_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RadioButton seleccion && seleccion.Checked)
                CargarHorariosDisponibles();
        }

        private void CargarHorariosDisponibles()
        {
            if (idTurnoGuardado.HasValue)
                return;
            comboBox1.DataSource = null;
            aviso.Text = "";
            string estudio = EstudioSeleccionado();
            if (estudio.Length == 0)
                return;
            if (estudio == "Laboratorio")
            {
                label2.Text = "Laboratorio sin turno. Con ayuno: 07:00 a 11:00. Sin ayuno: 07:00 a 17:00.";
                aviso.Text = "La recepción de laboratorio por orden de llegada todavía está pendiente de implementación.";
                return;
            }
            label2.Text = "Días de atención: " + string.Join(", ",
                ReglasEstudiosClinicos.DiasAtencion(estudio)
                    .Select(dia => CultureInfo.GetCultureInfo("es-AR").DateTimeFormat.GetDayName(dia)));
            DateTime fecha = monthCalendar1.SelectionStart.Date;
            if (!ReglasEstudiosClinicos.DiasAtencion(estudio).Contains(fecha.DayOfWeek))
            {
                aviso.Text = "El estudio no se realiza ese día. Seleccioná una fecha de los días habilitados.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var comando = new MySqlCommand("""
                    SELECT hora FROM `turnosEstudiosClínicos` WHERE estudio = @estudio AND fecha = @fecha
                    """, conexion);
                comando.Parameters.AddWithValue("@estudio", estudio);
                comando.Parameters.AddWithValue("@fecha", fecha);
                using var adaptador = new MySqlDataAdapter(comando);
                using var ocupados = new DataTable();
                adaptador.Fill(ocupados);
                var horasOcupadas = ocupados.AsEnumerable().Select(fila => fila.Field<TimeSpan>("hora")).ToHashSet();
                var horarios = new DataTable();
                horarios.Columns.Add("Hora", typeof(TimeSpan));
                horarios.Columns.Add("Descripcion", typeof(string));
                DateTime ahora = DateTime.Now;
                foreach (TimeSpan hora in ReglasEstudiosClinicos.HorariosBase(estudio))
                {
                    if (ReglasEstudiosClinicos.HorarioValido(estudio, fecha, hora, ahora) && !horasOcupadas.Contains(hora))
                        horarios.Rows.Add(hora, $"{hora.ToString("hh':'mm")} - {hora.Add(TimeSpan.FromHours(1)).ToString("hh':'mm")}");
                }
                comboBox1.DisplayMember = "Descripcion";
                comboBox1.ValueMember = "Hora";
                comboBox1.DataSource = horarios;
                comboBox1.SelectedIndex = -1;
                if (horarios.Rows.Count == 0)
                    aviso.Text = "No hay horarios disponibles para ese estudio y fecha.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudieron consultar los horarios. Revisá la conexión y el script 11.";
            }
        }

        private void BuscarPaciente()
        {
            aviso.Text = "";
            string dni = txtDni.Text.Trim();
            if (dni.Length == 0 || !dni.All(caracter => caracter >= '0' && caracter <= '9'))
            {
                aviso.Text = "Ingresá el DNI con números, sin puntos ni espacios.";
                return;
            }
            using var ficha = new VistaFichaPaciente(dni);
            if (ficha.ShowDialog(this) == DialogResult.OK)
            {
                idPaciente = ficha.IdPaciente;
                dniSeleccionado = dni;
                paciente.Text = $"Paciente N.º {ficha.IdPaciente}\n{ficha.NombreCompleto}";
            }
        }

        private void btnAceptar_Click(object? sender, EventArgs e)
        {
            aviso.Text = "";
            string estudio = EstudioSeleccionado();
            DateTime fecha = monthCalendar1.SelectionStart.Date;
            if (!idPaciente.HasValue || dniSeleccionado != txtDni.Text.Trim())
            {
                aviso.Text = "Buscá o registrá al paciente antes de reservar.";
                return;
            }
            if (comboBox1.SelectedValue is not TimeSpan hora ||
                !ReglasEstudiosClinicos.HorarioValido(estudio, fecha, hora, DateTime.Now))
            {
                aviso.Text = "Seleccioná un estudio con turno, una fecha habilitada y un horario futuro disponible.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();
                using var bloquear = new MySqlCommand(
                    "SELECT id_paciente FROM pacientes WHERE id_paciente = @paciente AND dni = @dni FOR UPDATE",
                    conexion, transaccion);
                bloquear.Parameters.AddWithValue("@paciente", idPaciente.Value);
                bloquear.Parameters.AddWithValue("@dni", dniSeleccionado);
                if (bloquear.ExecuteScalar() == null)
                {
                    aviso.Text = "La ficha cambió. Volvé a buscar el DNI.";
                    return;
                }
                ServicioReservasConsultorios.VerificarDisponibilidadPaciente(
                    conexion, transaccion, dniSeleccionado, fecha, hora, hora.Add(TimeSpan.FromHours(1)));
                using var insertar = new MySqlCommand("""
                    INSERT INTO `turnosEstudiosClínicos` (estudio, fecha, hora, dni_paciente, estado)
                    VALUES (@estudio, @fecha, @hora, @dni, 'Reservado')
                    """, conexion, transaccion);
                insertar.Parameters.AddWithValue("@estudio", estudio);
                insertar.Parameters.AddWithValue("@fecha", fecha);
                insertar.Parameters.AddWithValue("@hora", hora);
                insertar.Parameters.AddWithValue("@dni", dniSeleccionado);
                insertar.ExecuteNonQuery();
                transaccion.Commit();
                idTurnoGuardado = insertar.LastInsertedId;
                Aceptar.Enabled = false;
                txtDni.ReadOnly = true;
                buscarPaciente.Enabled = false;
                monthCalendar1.Enabled = false;
                comboBox1.Enabled = false;
                foreach (RadioButton opcion in new[] { Laboratorio, Electrocardiograma, Ecografía, Radiografía, Tomografía })
                    opcion.Enabled = false;
                verComprobante.Enabled = true;
                imprimir.Enabled = true;
                aviso.Text = $"Turno de estudio N.º {idTurnoGuardado} registrado para el {fecha:dd/MM/yyyy} a las {hora.ToString("hh':'mm")}.";
            }
            catch (InvalidOperationException ex)
            {
                aviso.Text = ex.Message;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                CargarHorariosDisponibles();
                aviso.Text = "Ese horario acaba de reservarse. Elegí uno de los horarios actualizados.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar la reserva. Revisá el script 11 y consultá los turnos antes de reintentar.";
            }
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            idTurnoGuardado = null;
            txtDni.Clear();
            txtDni.ReadOnly = false;
            buscarPaciente.Enabled = true;
            monthCalendar1.MinDate = DateTime.Today;
            monthCalendar1.Enabled = true;
            comboBox1.Enabled = true;
            Aceptar.Enabled = true;
            verComprobante.Enabled = false;
            imprimir.Enabled = false;
            foreach (RadioButton opcion in new[] { Laboratorio, Electrocardiograma, Ecografía, Radiografía, Tomografía })
                opcion.Enabled = true;
            CargarHorariosDisponibles();
            txtDni.Focus();
        }

        private void MostrarComprobante(bool imprimirDocumento)
        {
            if (!idTurnoGuardado.HasValue)
                return;
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var comando = new MySqlCommand("""
                    SELECT t.*, CONCAT(p.apellido, ', ', p.nombre) AS paciente
                    FROM `turnosEstudiosClínicos` t
                    JOIN pacientes p ON p.dni = t.dni_paciente
                    WHERE t.id_turno = @id
                    """, conexion);
                comando.Parameters.AddWithValue("@id", idTurnoGuardado.Value);
                using var adaptador = new MySqlDataAdapter(comando);
                using var datos = new DataTable();
                adaptador.Fill(datos);
                if (datos.Rows.Count == 0)
                {
                    aviso.Text = "No se encontraron los datos del turno.";
                    return;
                }
                using var documento = new DocumentoReservaEstudio(datos.Rows[0]);
                if (!documento.PrinterSettings.IsValid)
                {
                    aviso.Text = "Configurá una impresora o Microsoft Print to PDF en Windows.";
                    return;
                }
                if (imprimirDocumento)
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
                aviso.Text = "No se pudieron cargar los datos del comprobante.";
            }
            catch (InvalidPrinterException)
            {
                aviso.Text = "La impresora seleccionada no está disponible.";
            }
            catch (System.ComponentModel.Win32Exception)
            {
                aviso.Text = "No se pudo abrir la impresión. Revisá las impresoras de Windows.";
            }
        }

        private void btnAtrás_Click(object? sender, EventArgs e) => Close();
        private void Laboratorio_Click(object? sender, EventArgs e) { }
        private void Electrocardiograma_Click(object? sender, EventArgs e) { }
        private void radioButton3_CheckedChanged(object? sender, EventArgs e) { }
        private void Ecografía_Click(object? sender, EventArgs e) { }
        private void radioButton4_CheckedChanged(object? sender, EventArgs e) { }
        private void Radiografía_Click(object? sender, EventArgs e) { }
        private void Tomografía_Click(object? sender, EventArgs e) { }
    }
}
