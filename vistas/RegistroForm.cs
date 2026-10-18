using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic; // Necesario para el cuadro de diálogo de cambio de contraseña (InputBox)
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public partial class RegistroForm : Form
    {
        // Cadena de conexión a tu base de datos MySQL
        private string cadenaConexion = "server=localhost; database=clínica_seprice; uid=root; pwd=1234;";

        public RegistroForm()
        {
            InitializeComponent();
        }

        private void RegistroForm_Load(object sender, EventArgs e)
        {
            // Lógica inicial del formulario si la necesitas
        }

        // Este es el evento asociado a tu botón llamado "Enviar"
        private void Enviar_Click(object sender, EventArgs e)
        {
            // 1. Validar que todos los campos estén completos y se haya seleccionado un rol
            if (!string.IsNullOrWhiteSpace(nombre.Text) &&
                !string.IsNullOrWhiteSpace(apellido.Text) &&
                !string.IsNullOrWhiteSpace(usuario.Text) &&
                !string.IsNullOrWhiteSpace(email.Text) &&
                !string.IsNullOrWhiteSpace(telefono.Text) &&
                (paciente.Checked || Médico.Checked || administrativo.Checked))
            {
                // 2. Generar una contraseña aleatoria inicial
                string passwordGenerada = GenerarContrasenaAleatoria();

                // 3. Dar la opción de cambiar la contraseña generada
                DialogResult respuesta = MessageBox.Show(
                    $"Se ha generado la siguiente contraseña temporal: '{passwordGenerada}'\n\n¿Desea cambiarla por una propia?",
                    "Contraseña Generada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    // Ventana emergente para que el usuario ingrese su propia contraseña
                    string nuevaPassword = Interaction.InputBox("Ingrese su nueva contraseña:", "Cambiar Contraseña", passwordGenerada);

                    if (!string.IsNullOrWhiteSpace(nuevaPassword))
                    {
                        passwordGenerada = nuevaPassword;
                    }
                }

                // 4. Determinar el rol seleccionado en los RadioButtons
                string rolSeleccionado = "";
                if (paciente.Checked) rolSeleccionado = "paciente";
                else if (Médico.Checked) rolSeleccionado = "medico";
                else if (administrativo.Checked) rolSeleccionado = "administrativo"; // Corregido por seguridad en minúsculas como tu BD

                // 5. Ejecutar la inserción en la Base de Datos
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    try
                    {
                        conexion.Open();

                        string query = "INSERT INTO usuarios (nombre, apellido, usuario, contraseña, rol, email, telefono) " +
                                       "VALUES (@nom, @ape, @usua, @pass, @rol, @correo, @tel);";

                        using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                        {
                            cmd.Parameters.AddWithValue("@nom", nombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@ape", apellido.Text.Trim());
                            cmd.Parameters.AddWithValue("@usua", usuario.Text.Trim());
                            cmd.Parameters.AddWithValue("@pass", passwordGenerada); // Guarda la generada o la modificada
                            cmd.Parameters.AddWithValue("@rol", rolSeleccionado);
                            cmd.Parameters.AddWithValue("@correo", email.Text.Trim());
                            cmd.Parameters.AddWithValue("@tel", telefono.Text.Trim());

                            int resultado = cmd.ExecuteNonQuery();

                            if (resultado > 0)
                            {
                                MessageBox.Show("¡Registro exitoso! La contraseña ha sido guardada en el sistema.", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("No se pudo registrar el usuario en la base de datos.", "Error de registro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error de conexión con la base de datos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, complete todos los campos y seleccione un rol.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // Método auxiliar para generar una contraseña aleatoria de 8 caracteres
        private string GenerarContrasenaAleatoria()
        {
            string uuid = Guid.NewGuid().ToString("N");
            return uuid.Substring(0, 8);
        }

        // Este es el evento asociado a tu botón llamado "atras"
        private void atras_Click(object sender, EventArgs e)
        {
            this.Hide();
            logueo logueo = new logueo();
            logueo.Show();
        }
    }
}