using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public partial class VistaAcreditacion : Form
    {
        public VistaAcreditacion()
        {
            InitializeComponent();
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // 1. Limpiar el resultado de la busqueda anterior
            lblAviso.Text = "";
            dgvTurnos.DataSource = null;

            string dni = txtDni.Text.Trim();

            // 2. Comprobar que se haya ingresado un DNI.
            if (string.IsNullOrWhiteSpace(dni))
            {
                lblAviso.Text = " Ingresá el DNI del paciente. ";
                txtDni.Focus();
                return;
            }

            // 3. Buscar los turnos del dia para ese DNI.
            try
            {
                using var conexion = new MySqlConnection(
                    ConexionBaseDatos.CadenaConexion);

                const string consulta = """
                    
                    SELECT id_turno , especialidad, fecha, dni_paciente, estado
                    FROM `turnosConsultoriosMédicos`
                    WHERE dni_paciente = @dni
                        AND fecha = @fecha
                    ORDER BY id_turno
                    """;

                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@dni", dni);
                comando.Parameters.AddWithValue("@fecha", DateTime.Today);

                using var adaptador = new MySqlDataAdapter(comando);
                var turnos = new DataTable();
                adaptador.Fill(turnos);


                // 4. Mostrar los turnos encontrados.
                dgvTurnos.DataSource = turnos;

                if (turnos.Rows.Count == 0)
                {
                    lblAviso.Text = "El paciente no tiene turno asignado para hoy. ";
                }
            }

            catch (MySqlException)
            {
                lblAviso.Text = "No se pudiron consultar los turnos. Revisá la conexión a MYSQL. ";
            }


        }    
    }
}
