using System;
using System.Windows.Forms;

namespace Clínica_SePrice.vistas
{
    public partial class pantalla_principal : Form
    {
        private string rolActual;

        // 1. Constructor que recibe el rol
        public pantalla_principal(string rol)
        {
            InitializeComponent();

            // Guardamos el rol limpiando espacios y minúsculas
            rolActual = rol != null ? rol.ToLower().Trim() : "";

            // Vinculamos los eventos Click de los botones por código
            btnTurnos.Click += btnTurnos_Click;
            btnEstudios.Click += btnEstudios_Click;
            btnHistorial.Click += btnHistorial_Click;
            btnInsumos.Click += btnInsumos_Click;

            // Ejecutamos la configuración visual inmediatamente
            ConfigurarInterfazSegunRol();
        }

        // 2. Método de configuración por rol
        private void ConfigurarInterfazSegunRol()
        {
            if (rolActual == "administrativo")
            {
                if (btnInsumos != null)
                {
                    btnInsumos.Visible = true;
                }
                this.Text = "Clínica SePrice - Panel de ADMINISTRACIÓN";
            }
            else
            {
                if (btnInsumos != null)
                {
                    btnInsumos.Visible = false;
                }
                this.Text = $"Clínica SePrice - Panel de {rolActual.ToUpper()}";
            }
        }

        // 3. Evento para cerrar la aplicación al cerrar la ventana principal
        private void pantalla_principal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        // 4. AQUÍ ES DONDE DEBES AGREGAR LOS EVENTOS DE LOS BOTONES NUEVOS
        private void btnTurnos_Click(object sender, EventArgs e)
        {
            VistaTurnos ventanaTurnos = new VistaTurnos();
            ventanaTurnos.Show();
        }

        private void btnEstudios_Click(object sender, EventArgs e)
        {
            VistaEstudios ventanaEstudios = new VistaEstudios();
            ventanaEstudios.Show();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            VistaHistorial ventanaHistorial = new VistaHistorial();
            ventanaHistorial.Show();
        }

        private void btnInsumos_Click(object sender, EventArgs e)
        {
            VistaInsumos ventanaInsumos = new VistaInsumos();
            ventanaInsumos.Show();
        }
    }
}