using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Clínica_SePrice.vistas
{
    public partial class VistaTurnos : Form
    {
        private Form ventanaAnterior;

        // El constructor OBLIGATORIAMENTE debe recibir esto:
        public VistaTurnos(Form anterior, string rol = "")
        {
            InitializeComponent();
            ventanaAnterior = anterior; // Guardamos la referencia

            btnAcreditar.Visible = rol == "administrativo";

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (ventanaAnterior != null)
            {
                ventanaAnterior.Show(); // Muestra el menú principal de nuevo
            }
            this.Close(); // Cierra la vista actual
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ConsultoriosExternos_click(object sender, EventArgs e)
        {
            TurnosConsultoriosMédicos turnosConsultoriosMédicos = new TurnosConsultoriosMédicos();
            turnosConsultoriosMédicos.Show();
        }

        private void EstudiosClínicos_Click(object sender, EventArgs e)
        {

        }

        private void btnAcreditar_Click(object sender, EventArgs e)
        {
            using var ventana = new VistaAcreditacion();
            ventana.ShowDialog(this);
        }
    }
}
