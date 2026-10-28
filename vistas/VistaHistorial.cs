using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Clínica_SePrice.vistas
{
    public partial class VistaHistorial : Form
    {
        private Form ventanaAnterior;

        // Modificamos el constructor para que reciba la ventana anterior por parámetro
        public VistaHistorial(Form anterior)
        {
            InitializeComponent();
            ventanaAnterior = anterior; // ¡Acá ya reconoce a 'anterior' sin problemas!
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            if (ventanaAnterior != null)
            {
                ventanaAnterior.Show(); // Volvemos a mostrar la ventana anterior
            }

            this.Close(); // Cerramos la ventana actual del historial
        } 

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}