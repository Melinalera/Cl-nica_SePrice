using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaRegistrarAtencion : Form
    {
        private readonly DataGridView grilla = new()
        {
            Location = new Point(20, 100), Size = new Size(860, 220),
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        private readonly ComboBox cmbMedico = new()
        {
            Location = new Point(170, 340), Size = new Size(600, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly TextBox txtPractica = new()
        {
            Location = new Point(170, 380), Width = 600, MaxLength = 100
        };
        private readonly TextBox txtImporte = new()
        {
            Location = new Point(170, 420), Width = 180, MaxLength = 13
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 470), Size = new Size(860, 45),
            ForeColor = Color.Firebrick
        };
        private readonly Button guardar = new()
        {
            Text = "Registrar atención", Location = new Point(660, 530), Size = new Size(220, 30)
        };

        public VistaRegistrarAtencion()
        {
            Text = "Registrar atención de consultorios externos";
            ClientSize = new Size(900, 580);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            var volver = new Button
            {
                Text = "Volver", Location = new Point(12, 12), Size = new Size(75, 23),
                Font = new Font("Tahoma", 8.25F, FontStyle.Bold),
                BackColor = Color.Teal, ForeColor = SystemColors.ControlLight,
                UseVisualStyleBackColor = false
            };
            volver.Click += (_, _) => Close();
            var titulo = new Label
            {
                Text = "Registrar atención",
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(100, 15), Size = new Size(680, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var actualizar = new Button
            {
                Text = "Actualizar", Location = new Point(770, 60), Size = new Size(110, 27)
            };
            actualizar.Click += (_, _) => CargarDatos();
            guardar.Click += Guardar_Click;
            Controls.AddRange(new Control[]
            {
                volver, titulo, actualizar,
                new Label { Text = "Turnos acreditados para hoy", Location = new Point(20, 67), AutoSize = true },
                grilla,
                new Label { Text = "Médico que atendió", Location = new Point(20, 343), AutoSize = true },
                cmbMedico,
                new Label { Text = "Práctica realizada", Location = new Point(20, 383), AutoSize = true },
                txtPractica,
                new Label { Text = "Honorario ($)", Location = new Point(20, 423), AutoSize = true },
                txtImporte,
                new Label { Text = "Ejemplo: 15000,00 (sin puntos de miles)", Location = new Point(370, 423), AutoSize = true },
                aviso, guardar
            });
            Load += (_, _) => CargarDatos();
        }

        private void CargarDatos()
        {
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consultaMedicos = """
                    SELECT m.id_usuario, m.especialidad,
                           CONCAT(u.apellido, ', ', u.nombre, ' — ', COALESCE(m.especialidad, 'Sin especialidad')) AS medico
                    FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                    WHERE u.rol = 'medico'
                    ORDER BY u.apellido, u.nombre, m.id_usuario
                    """;
                using var adaptadorMedicos = new MySqlDataAdapter(consultaMedicos, conexion);
                var medicos = new DataTable();
                adaptadorMedicos.Fill(medicos);
                cmbMedico.DisplayMember = "medico";
                cmbMedico.ValueMember = "id_usuario";
                cmbMedico.DataSource = medicos;
                cmbMedico.SelectedIndex = -1;

                const string consultaTurnos = """
                    SELECT t.id_turno, t.dni_paciente, t.especialidad, t.fecha, t.estado,
                           r.hora, CONCAT(u.apellido, ', ', u.nombre) AS medico_asignado
                    FROM `turnosConsultoriosMédicos` t
                    JOIN acreditaciones_consultorios a ON a.id_turno = t.id_turno
                    LEFT JOIN reservas_agenda_consultorios r ON r.id_turno = t.id_turno
                    LEFT JOIN agendas_consultorios g ON g.id_agenda = r.id_agenda
                    LEFT JOIN usuarios u ON u.id_usuario = g.id_medico
                    WHERE t.fecha = @fecha AND t.estado = 'Acreditado'
                    ORDER BY a.fecha_acreditacion, t.id_turno
                    """;
                using var comando = new MySqlCommand(consultaTurnos, conexion);
                comando.Parameters.AddWithValue("@fecha", DateTime.Today);
                using var adaptadorTurnos = new MySqlDataAdapter(comando);
                var turnos = new DataTable();
                adaptadorTurnos.Fill(turnos);
                grilla.DataSource = turnos;
                if (turnos.Rows.Count == 0)
                    aviso.Text = "No hay turnos acreditados para hoy.";
                else if (medicos.Rows.Count == 0)
                    aviso.Text = "No hay médicos registrados. Revisá las tablas medicos y usuarios.";
            }
            catch (MySqlException)
            {
                grilla.DataSource = null;
                cmbMedico.DataSource = null;
                aviso.Text = "No se pudieron cargar los turnos y médicos. Revisá la conexión y las tablas.";
            }
        }

        private void Guardar_Click(object? sender, EventArgs e)
        {
            aviso.Text = "";

            // 1. Validar la atención y el importe informado.
            if (grilla.CurrentRow?.DataBoundItem is not DataRowView turno ||
                cmbMedico.SelectedItem is not DataRowView medico)
            {
                aviso.Text = "Seleccioná un turno acreditado y el médico que lo atendió.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPractica.Text))
            {
                aviso.Text = "Ingresá la práctica realizada.";
                txtPractica.Focus();
                return;
            }
            if (!decimal.TryParse(txtImporte.Text.Trim(), NumberStyles.AllowDecimalPoint,
                    CultureInfo.GetCultureInfo("es-AR"), out decimal importe) ||
                importe < 0 || importe > 9999999999.99m || decimal.Round(importe, 2) != importe)
            {
                aviso.Text = "Ingresá un importe válido con hasta dos decimales, usando coma y sin separadores de miles.";
                txtImporte.Focus();
                return;
            }

            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();

                // 2. Confirmar que el turno siga acreditado y corresponda a la especialidad del médico.
                using var actualizar = new MySqlCommand("""
                    UPDATE `turnosConsultoriosMédicos` t
                    SET t.estado = 'Atendido'
                    WHERE t.id_turno = @turno AND t.fecha = @fecha AND t.estado = 'Acreditado'
                      AND EXISTS (SELECT 1 FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                                  WHERE m.id_usuario = @medico AND u.rol = 'medico'
                                    AND TRIM(m.especialidad) = TRIM(t.especialidad))
                      AND EXISTS (SELECT 1 FROM acreditaciones_consultorios a WHERE a.id_turno = t.id_turno)
                      AND (NOT EXISTS (SELECT 1 FROM reservas_agenda_consultorios r WHERE r.id_turno = t.id_turno)
                           OR EXISTS (SELECT 1 FROM reservas_agenda_consultorios r
                                      JOIN agendas_consultorios a ON a.id_agenda = r.id_agenda
                                      WHERE r.id_turno = t.id_turno AND a.id_medico = @medico))
                    """, conexion, transaccion);
                actualizar.Parameters.AddWithValue("@turno", turno["id_turno"]);
                actualizar.Parameters.AddWithValue("@fecha", DateTime.Today);
                actualizar.Parameters.AddWithValue("@medico", medico["id_usuario"]);
                if (actualizar.ExecuteNonQuery() != 1)
                {
                    aviso.Text = "Revisá el turno acreditado de hoy y el médico asignado. En turnos anteriores sin agenda, debe coincidir la especialidad.";
                    return;
                }

                // 3. Guardar la atención y el honorario sin duplicar el turno.
                using var insertar = new MySqlCommand("""
                    INSERT INTO atenciones_consultorios (id_turno, id_medico, practica, importe)
                    VALUES (@turno, @medico, @practica, @importe)
                    """, conexion, transaccion);
                insertar.Parameters.AddWithValue("@turno", turno["id_turno"]);
                insertar.Parameters.AddWithValue("@medico", medico["id_usuario"]);
                insertar.Parameters.AddWithValue("@practica", txtPractica.Text.Trim());
                insertar.Parameters.AddWithValue("@importe", importe);
                insertar.ExecuteNonQuery();
                transaccion.Commit();

                txtPractica.Clear();
                txtImporte.Clear();
                CargarDatos();
                aviso.Text = $"Atención N.º {insertar.LastInsertedId} registrada. Turno marcado como Atendido.";
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                aviso.Text = "Ese turno ya tiene una atención registrada. Actualizá el listado.";
            }
            catch (MySqlException ex) when (ex.Number == 1146)
            {
                aviso.Text = "Falta una tabla. Revisá los scripts de acreditaciones y honorarios (02 y 05).";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar la atención. Actualizá el listado antes de reintentar.";
            }
        }
    }
}
