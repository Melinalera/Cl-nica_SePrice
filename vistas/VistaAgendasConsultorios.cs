using System.Data;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaAgendasConsultorios : Form
    {
        private readonly ComboBox medico = new()
        {
            Location = new Point(150, 95), Width = 760, DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly ComboBox consultorio = new()
        {
            Location = new Point(150, 135), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly TextBox nuevoConsultorio = new()
        {
            Location = new Point(465, 135), Width = 265, MaxLength = 100
        };
        private readonly DateTimePicker fecha = new()
        {
            Location = new Point(150, 185), Width = 150, Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today, Value = DateTime.Today
        };
        private readonly DateTimePicker inicio = new()
        {
            Location = new Point(390, 185), Width = 100, Format = DateTimePickerFormat.Custom,
            CustomFormat = "HH:mm", ShowUpDown = true, Value = DateTime.Today.AddHours(8)
        };
        private readonly DateTimePicker fin = new()
        {
            Location = new Point(570, 185), Width = 100, Format = DateTimePickerFormat.Custom,
            CustomFormat = "HH:mm", ShowUpDown = true, Value = DateTime.Today.AddHours(12)
        };
        private readonly NumericUpDown duracion = new()
        {
            Location = new Point(150, 235), Width = 100, Minimum = 15, Maximum = 1439, Value = 15
        };
        private readonly Label minimo = new()
        {
            Location = new Point(270, 238), Size = new Size(640, 25)
        };
        private readonly DataGridView grilla = new()
        {
            Location = new Point(20, 320), Size = new Size(910, 230),
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 570), Size = new Size(910, 55), ForeColor = Color.Firebrick
        };

        public VistaAgendasConsultorios()
        {
            Text = "Agendas de consultorios externos";
            ClientSize = new Size(950, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            var volver = new Button
            {
                Text = "Volver", Location = new Point(12, 12), Size = new Size(75, 23),
                BackColor = Color.Teal, ForeColor = SystemColors.ControlLight,
                Font = new Font("Tahoma", 8.25F, FontStyle.Bold), UseVisualStyleBackColor = false
            };
            volver.Click += (_, _) => Close();
            var crearConsultorio = new Button
            {
                Text = "Agregar consultorio", Location = new Point(745, 133), Size = new Size(165, 27)
            };
            crearConsultorio.Click += (_, _) => AgregarConsultorio();
            var guardar = new Button
            {
                Text = "Guardar agenda", Location = new Point(730, 275), Size = new Size(180, 30)
            };
            guardar.Click += (_, _) => GuardarAgenda();
            var actualizar = new Button
            {
                Text = "Actualizar listado", Location = new Point(730, 183), Size = new Size(180, 27)
            };
            actualizar.Click += (_, _) => CargarAgendas();
            Controls.AddRange(new Control[]
            {
                volver,
                new Label
                {
                    Text = "Agendas de consultorios externos",
                    Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                    Location = new Point(100, 15), Size = new Size(730, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                },
                new Label { Text = "Profesional", Location = new Point(20, 98), AutoSize = true }, medico,
                new Label { Text = "Consultorio", Location = new Point(20, 138), AutoSize = true }, consultorio,
                nuevoConsultorio, crearConsultorio,
                new Label { Text = "Fecha", Location = new Point(20, 188), AutoSize = true }, fecha,
                new Label { Text = "Desde", Location = new Point(330, 188), AutoSize = true }, inicio,
                new Label { Text = "Hasta", Location = new Point(510, 188), AutoSize = true }, fin,
                new Label { Text = "Minutos por turno", Location = new Point(20, 238), AutoSize = true },
                duracion, minimo, guardar, actualizar, grilla, aviso
            });
            medico.SelectedIndexChanged += (_, _) =>
            {
                if (medico.SelectedItem is DataRowView profesional)
                {
                    int minutos = ReglasAgendaConsultorios.DuracionMinima(profesional["especialidad"].ToString() ?? "");
                    duracion.Minimum = minutos;
                    duracion.Value = minutos;
                    minimo.Text = $"Mínimo de la especialidad: {minutos} minutos.";
                }
            };
            fecha.ValueChanged += (_, _) => CargarAgendas();
            Load += (_, _) => CargarCatalogos();
        }

        private void CargarCatalogos()
        {
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var profesionales = new MySqlDataAdapter("""
                    SELECT m.id_usuario, m.especialidad,
                           CONCAT(u.apellido, ', ', u.nombre, ' — ', COALESCE(m.especialidad, '')) AS nombre
                    FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                    WHERE u.rol = 'medico' AND TRIM(COALESCE(m.especialidad, '')) <> ''
                    ORDER BY u.apellido, u.nombre, m.id_usuario
                    """, conexion);
                var medicos = new DataTable();
                profesionales.Fill(medicos);
                medico.DisplayMember = "nombre";
                medico.ValueMember = "id_usuario";
                medico.DataSource = medicos;
                medico.SelectedIndex = -1;
                CargarConsultorios();
                CargarAgendas();
                if (medicos.Rows.Count == 0)
                    aviso.Text = "Registrá médicos con especialidad antes de configurar sus agendas.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudieron cargar los profesionales. Revisá la conexión y sus perfiles.";
            }
        }

        private void CargarConsultorios()
        {
            using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
            using var adaptador = new MySqlDataAdapter(
                "SELECT id_consultorio, nombre FROM consultorios WHERE activo = TRUE ORDER BY nombre", conexion);
            var datos = new DataTable();
            adaptador.Fill(datos);
            consultorio.DisplayMember = "nombre";
            consultorio.ValueMember = "id_consultorio";
            consultorio.DataSource = datos;
            consultorio.SelectedIndex = -1;
        }

        private void AgregarConsultorio()
        {
            aviso.Text = "";
            if (string.IsNullOrWhiteSpace(nuevoConsultorio.Text))
            {
                aviso.Text = "Escribí el nombre del nuevo consultorio.";
                nuevoConsultorio.Focus();
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var comando = new MySqlCommand(
                    "INSERT INTO consultorios (nombre) VALUES (@nombre)", conexion);
                comando.Parameters.AddWithValue("@nombre", nuevoConsultorio.Text.Trim());
                comando.ExecuteNonQuery();
                int id = checked((int)comando.LastInsertedId);
                CargarConsultorios();
                consultorio.SelectedValue = id;
                nuevoConsultorio.Clear();
                aviso.Text = "Consultorio agregado.";
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                aviso.Text = "Ya existe un consultorio con ese nombre.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar el consultorio. Revisá los datos antes de reintentar.";
            }
        }

        private void CargarAgendas()
        {
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var comando = new MySqlCommand("""
                    SELECT a.id_agenda AS Agenda, CONCAT(u.apellido, ', ', u.nombre) AS Medico,
                           m.especialidad AS Especialidad, c.nombre AS Consultorio,
                           a.fecha AS Fecha, a.hora_inicio AS Desde, a.hora_fin AS Hasta,
                           a.duracion_minutos AS Minutos
                    FROM agendas_consultorios a
                    JOIN medicos m ON m.id_usuario = a.id_medico
                    JOIN usuarios u ON u.id_usuario = m.id_usuario
                    JOIN consultorios c ON c.id_consultorio = a.id_consultorio
                    WHERE a.fecha = @fecha ORDER BY a.hora_inicio, u.apellido, a.id_agenda
                    """, conexion);
                comando.Parameters.AddWithValue("@fecha", fecha.Value.Date);
                using var adaptador = new MySqlDataAdapter(comando);
                var datos = new DataTable();
                adaptador.Fill(datos);
                grilla.DataSource = datos;
                if (datos.Rows.Count == 0)
                    aviso.Text = "No hay agendas para la fecha seleccionada.";
            }
            catch (MySqlException)
            {
                grilla.DataSource = null;
                aviso.Text = "No se pudieron consultar las agendas. Revisá el script 09 y la conexión.";
            }
        }

        private void GuardarAgenda()
        {
            aviso.Text = "";
            if (medico.SelectedValue is not int idMedico || consultorio.SelectedValue is not int idConsultorio)
            {
                aviso.Text = "Seleccioná un profesional y un consultorio.";
                return;
            }
            TimeSpan horaInicio = new(inicio.Value.Hour, inicio.Value.Minute, 0);
            TimeSpan horaFin = new(fin.Value.Hour, fin.Value.Minute, 0);
            int minutos = (int)duracion.Value;
            DateTime dia = fecha.Value.Date;
            if (dia < DateTime.Today || dia.Add(horaInicio) <= DateTime.Now ||
                horaFin <= horaInicio || (horaFin - horaInicio).TotalMinutes < minutos)
            {
                aviso.Text = "La agenda debe comenzar en el futuro y permitir al menos un turno completo.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();

                // Bloquear los recursos antes de verificar superposiciones.
                using var profesional = new MySqlCommand("""
                    SELECT m.especialidad FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                    WHERE m.id_usuario = @id AND u.rol = 'medico' FOR UPDATE
                    """, conexion, transaccion);
                profesional.Parameters.AddWithValue("@id", idMedico);
                string? especialidad = profesional.ExecuteScalar()?.ToString();
                if (string.IsNullOrWhiteSpace(especialidad) ||
                    minutos < ReglasAgendaConsultorios.DuracionMinima(especialidad))
                {
                    aviso.Text = "Revisá la especialidad del médico y la duración mínima del turno.";
                    return;
                }
                using var espacio = new MySqlCommand(
                    "SELECT id_consultorio FROM consultorios WHERE id_consultorio = @id AND activo = TRUE FOR UPDATE",
                    conexion, transaccion);
                espacio.Parameters.AddWithValue("@id", idConsultorio);
                if (espacio.ExecuteScalar() == null)
                {
                    aviso.Text = "El consultorio ya no está disponible.";
                    return;
                }
                using var superposicion = new MySqlCommand("""
                    SELECT id_agenda FROM agendas_consultorios
                    WHERE fecha = @fecha AND (id_medico = @medico OR id_consultorio = @consultorio)
                      AND hora_inicio < @fin AND hora_fin > @inicio
                    LIMIT 1 FOR UPDATE
                    """, conexion, transaccion);
                superposicion.Parameters.AddWithValue("@fecha", dia);
                superposicion.Parameters.AddWithValue("@medico", idMedico);
                superposicion.Parameters.AddWithValue("@consultorio", idConsultorio);
                superposicion.Parameters.AddWithValue("@inicio", horaInicio);
                superposicion.Parameters.AddWithValue("@fin", horaFin);
                if (superposicion.ExecuteScalar() != null)
                {
                    aviso.Text = "El médico o el consultorio ya tienen una agenda que se superpone con ese horario.";
                    return;
                }
                using var guardar = new MySqlCommand("""
                    INSERT INTO agendas_consultorios
                        (id_medico, id_consultorio, fecha, hora_inicio, hora_fin, duracion_minutos)
                    VALUES (@medico, @consultorio, @fecha, @inicio, @fin, @duracion)
                    """, conexion, transaccion);
                guardar.Parameters.AddWithValue("@medico", idMedico);
                guardar.Parameters.AddWithValue("@consultorio", idConsultorio);
                guardar.Parameters.AddWithValue("@fecha", dia);
                guardar.Parameters.AddWithValue("@inicio", horaInicio);
                guardar.Parameters.AddWithValue("@fin", horaFin);
                guardar.Parameters.AddWithValue("@duracion", minutos);
                guardar.ExecuteNonQuery();
                transaccion.Commit();
                CargarAgendas();
                aviso.Text = $"Agenda N.º {guardar.LastInsertedId} registrada.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar la agenda. Actualizá el listado antes de reintentar.";
            }
        }
    }
}
