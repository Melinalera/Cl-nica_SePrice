using System;
using System.Windows.Forms;

namespace Clínica_SePrice.vistas
{
    public partial class pantalla_principal : Form
    {
        private string rolActual;

        // Modificamos el constructor para que configure el rol ni bien nace la ventana
        public pantalla_principal(string rol)
        {
            InitializeComponent();

            // 1. Guardamos el rol limpiando espacios y mayúsculas
            rolActual = rol != null ? rol.ToLower().Trim() : "";

            // 3. Ejecutamos la configuración visual inmediatamente
            ConfigurarInterfazSegunRol();
        }

        private void ConfigurarInterfazSegunRol()
        {
            // Verificamos si es administrativo
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
                // Para paciente, médico o cualquier otro
                if (btnInsumos != null)
                {
                    btnInsumos.Visible = false; // Se oculta sí o sí
                }
                this.Text = $"Clínica SePrice - Panel de {rolActual.ToUpper()}";
            }
        }

        private void pantalla_principal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}