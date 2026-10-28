using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public partial class logueo : Form
    {
        public logueo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Cadena de conexión a MySQL
            string cadenaConexion = "server=localhost; database=clínica_seprice; uid=root; pwd=1234;";

            // Modificamos la consulta para traernos el rol y el nombre del usuario
            string query = "SELECT rol, nombre FROM usuarios WHERE usuario = @user AND contraseña = @pass";

            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        // Asignamos los valores de tus cajas de texto
                        cmd.Parameters.AddWithValue("@user", usuario.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", contraseña.Text.Trim());

                        // Usamos ExecuteReader para leer los datos que devuelve la consulta
                        using (MySqlDataReader lector = cmd.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                // Extraemos el rol y el nombre directamente de la base de datos
                                string rolUsuario = lector["rol"].ToString();
                                string nombreUsuario = lector["nombre"].ToString();

                                MessageBox.Show($"¡Bienvenido, {nombreUsuario}!", "Ingreso exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Abrimos la pantalla principal pasándole el rol por parámetro y ocultamos el login
                                pantalla_principal principal = new pantalla_principal(rolUsuario);
                                principal.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handlers añadidos para las referencias del diseñador
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Si necesita lógica al cambiar 'usuario', colóquela aquí.
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Si necesita lógica al cambiar 'contraseña', colóquela aquí.
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrarse_Click(object sender, EventArgs e)
        {
            // Instanciar el formulario de registro
            RegistroForm ventanaRegistro = new RegistroForm();
            ventanaRegistro.Show();
        }

        private void logueo_Load(object sender, EventArgs e)
        {

        }
    }
}