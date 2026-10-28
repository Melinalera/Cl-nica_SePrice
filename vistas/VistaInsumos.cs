using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Clínica_SePrice.vistas
{
    public partial class VistaInsumos : Form
    {
        private Form ventanaAnterior;
        public VistaInsumos(Form anterior)
        {
            InitializeComponent();
            ventanaAnterior = anterior; // Guardamos la referencia
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (ventanaAnterior != null)
            {
                ventanaAnterior.Show(); // Muestra el menú principal de nuevo
            }
            this.Close(); // Cierra la vista actual
        }
    }
}
