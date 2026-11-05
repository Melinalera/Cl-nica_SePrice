using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public partial class VistaEstudiosGuardia : Form
    {
        public VistaEstudiosGuardia()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void btnRegistrar_Click(object? sender, EventArgs e)
        {
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Text = "";

            // 1. Validar los datos del paciente, el estudio y la orden médica.
            foreach (TextBox campo in new[] { txtDni, txtPaciente, txtEstudio, txtOrden })
            {
                if (string.IsNullOrWhiteSpace(campo.Text))
                {
                    lblAviso.Text = "Completá el DNI, nombre y apellido, estudio y referencia de la orden.";
                    campo.Focus();
                    return;
                }
            }

            if (!txtDni.Text.Trim().All(caracter => caracter >= '0' && caracter <= '9'))
            {
                lblAviso.Text = "El DNI debe contener solamente números, sin puntos ni espacios.";
                txtDni.Focus();
                return;
            }

            if (cmbOrigen.SelectedIndex < 0 || cmbPrioridad.SelectedIndex < 0)
            {
                lblAviso.Text = "Seleccioná el origen y la prioridad de la solicitud.";
                return;
            }

            if (!chkOrdenValida.Checked)
            {
                lblAviso.Text = "Verificá la orden médica y marcá la casilla de confirmación.";
                chkOrdenValida.Focus();
                return;
            }

            // 2. Guardar la solicitud sin crear un turno.
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                const string consulta = """
                    INSERT INTO solicitudes_estudios_guardia
                        (dni_paciente, nombre_paciente, estudio_solicitado,
                         origen, prioridad, referencia_orden, orden_verificada)
                    VALUES
                        (@dni, @paciente, @estudio, @origen, @prioridad, @orden, @verificada)
                    """;
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                comando.Parameters.AddWithValue("@paciente", txtPaciente.Text.Trim());
                comando.Parameters.AddWithValue("@estudio", txtEstudio.Text.Trim());
                comando.Parameters.AddWithValue("@origen", cmbOrigen.SelectedItem!.ToString());
                comando.Parameters.AddWithValue("@prioridad", cmbPrioridad.SelectedItem!.ToString());
                comando.Parameters.AddWithValue("@orden", txtOrden.Text.Trim());
                comando.Parameters.AddWithValue("@verificada", true);
                comando.ExecuteNonQuery();

                // 3. Conservar los datos visibles y evitar registrar dos veces la misma solicitud.
                btnRegistrar.Enabled = false;
                txtDni.ReadOnly = true;
                txtPaciente.ReadOnly = true;
                txtEstudio.ReadOnly = true;
                txtOrden.ReadOnly = true;
                cmbOrigen.Enabled = false;
                cmbPrioridad.Enabled = false;
                chkOrdenValida.Enabled = false;
                lblAviso.ForeColor = Color.DarkGreen;
                lblAviso.Text = $"Solicitud N.º {comando.LastInsertedId} registrada. Estado: Pendiente.";
            }
            catch (MySqlException ex) when (ex.Number == 1146)
            {
                lblAviso.Text = "Falta crear la tabla. Ejecutá el script 03_solicitudes_estudios_guardia.sql.";
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudo confirmar el registro. Revisá las solicitudes en MySQL antes de reintentar.";
            }
        }
    }
}
