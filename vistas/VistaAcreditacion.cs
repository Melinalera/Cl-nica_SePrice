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
                    
                    SELECT t.id_turno, t.especialidad, t.fecha, t.dni_paciente, t.estado,
                           r.hora, CONCAT(u.apellido, ', ', u.nombre) AS medico,
                           c.nombre AS consultorio
                    FROM `turnosConsultoriosMédicos` t
                    LEFT JOIN reservas_agenda_consultorios r ON r.id_turno = t.id_turno
                    LEFT JOIN agendas_consultorios a ON a.id_agenda = r.id_agenda
                    LEFT JOIN usuarios u ON u.id_usuario = a.id_medico
                    LEFT JOIN consultorios c ON c.id_consultorio = a.id_consultorio
                    WHERE t.dni_paciente = @dni
                        AND t.fecha = @fecha
                    ORDER BY r.hora, t.id_turno
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

        private void chkIdentidad_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cmbModalidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Habilitar los datos de afiliación cuando se elige obra social.
            bool usaObraSocial = cmbModalidad.SelectedItem?.ToString() == "Obra social";

            txtObraSocial.Enabled = usaObraSocial;
            txtAfiliado.Enabled = usaObraSocial;

            // Limpiar los datos de afiliación al cambiar a partucular
            if (!usaObraSocial)
            {
                txtObraSocial.Clear();
                txtAfiliado.Clear();
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            // 1. Validar el turno seleccionado y la verificación de identidad.
            lblAviso.Text = "";

            if (dgvTurnos.CurrentRow?.DataBoundItem is not DataRowView turno)
            {
                lblAviso.Text = "Buscá y seleccioná un turno.";
                return;
            }

            string dni = txtDni.Text.Trim();
            string dniTurno = turno["dni_paciente"].ToString()?.Trim() ?? "";

            if (dni != dniTurno || dni.Length == 0)
            {
                lblAviso.Text = "El DNI cambió. Volvé a buscar el turno.";
                return;
            }

            if (!chkIdentidad.Checked)
            {
                lblAviso.Text = "Verificá el documento y marcá la confirmación de identidad.";
                return;
            }

            // 2. Validar la modalidad de pago y los datos de afiliación.
            string modalidad = cmbModalidad.SelectedItem?.ToString() ?? "";

            if (modalidad != "Particular" && modalidad != "Obra social")
            {
                lblAviso.Text = "Seleccioná la modalidad de pago.";
                return;
            }

            bool usaObraSocial = modalidad == "Obra social";

            if (usaObraSocial &&
                (string.IsNullOrWhiteSpace(txtObraSocial.Text) ||
                 string.IsNullOrWhiteSpace(txtAfiliado.Text)))
            {
                lblAviso.Text = "Completá la obra social y el número de afiliado.";
                return;
            }

            try
            {
                using var conexion = new MySqlConnection(
                    ConexionBaseDatos.CadenaConexion);

                conexion.Open();

                // 3. Iniciar una transacción para guardar ambas operaciones juntas.
                using var transaccion = conexion.BeginTransaction();

                // Solo acreditar un turno reservado para hoy y para ese DNI.
                const string actualizarTurno = """
        UPDATE `turnosConsultoriosMédicos`
        SET estado = 'Acreditado'
        WHERE id_turno = @id
          AND dni_paciente = @dni
          AND fecha = @fecha
          AND estado = 'Reservado'
        """;

                using var actualizar = new MySqlCommand(
                    actualizarTurno, conexion, transaccion);

                actualizar.Parameters.AddWithValue("@id", turno["id_turno"]);
                actualizar.Parameters.AddWithValue("@dni", dni);
                actualizar.Parameters.AddWithValue("@fecha", DateTime.Today);

                if (actualizar.ExecuteNonQuery() != 1)
                {
                    lblAviso.Text =
                        "El turno debe estar reservado para hoy. Actualizá la búsqueda.";
                    return;
                }

                // 4. Registrar la identidad verificada y la modalidad de pago.
                const string insertarAcreditacion = """
        INSERT INTO acreditaciones_consultorios
            (id_turno, dni_verificado, modalidad_pago,
             obra_social, numero_afiliado)
        VALUES
            (@id, @dni, @modalidad, @obraSocial, @afiliado)
        """;

                using var insertar = new MySqlCommand(
                    insertarAcreditacion, conexion, transaccion);

                insertar.Parameters.AddWithValue("@id", turno["id_turno"]);
                insertar.Parameters.AddWithValue("@dni", dni);
                insertar.Parameters.AddWithValue(
                    "@modalidad", usaObraSocial ? "obra_social" : "particular");

                insertar.Parameters.AddWithValue(
                    "@obraSocial",
                    usaObraSocial ? (object)txtObraSocial.Text.Trim() : DBNull.Value);

                insertar.Parameters.AddWithValue(
                    "@afiliado",
                    usaObraSocial ? (object)txtAfiliado.Text.Trim() : DBNull.Value);

                insertar.ExecuteNonQuery();

                // 5. Confirmar la operación y actualizar el estado en la grilla.
                transaccion.Commit();

                turno["estado"] = "Acreditado";
                chkIdentidad.Checked = false;
                cmbModalidad.SelectedIndex = -1;
                txtObraSocial.Clear();
                txtAfiliado.Clear();

                lblAviso.Text = "Acreditación registrada. Paciente en espera.";
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                lblAviso.Text = "Este turno ya tiene una acreditación registrada.";
            }
            catch (MySqlException)
            {
                lblAviso.Text =
                    "No se pudo confirmar la acreditación. Actualizá la búsqueda antes de reintentar.";
            }
        }
    }
}
