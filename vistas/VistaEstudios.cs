using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Clínica_SePrice.vistas
{
    public partial class VistaEstudios : Form
    {
        private Form ventanaAnterior;
        public VistaEstudios(Form anterior, string rol = "")
        {
            InitializeComponent();
            ventanaAnterior = anterior; // Guardamos la referencia
            var btnGuardia = new Button
            {
                Name = "btnGuardia",
                Text = "Guardia / Internación",
                Location = new Point(280, 130),
                Size = new Size(240, 30),
                Visible = rol == "administrativo"
            };
            btnGuardia.Click += (_, _) =>
            {
                using var ventana = new VistaEstudiosGuardia();
                ventana.ShowDialog(this);
            };
            Controls.Add(btnGuardia);
            var btnSolicitudes = new Button
            {
                Name = "btnSolicitudesGuardia",
                Text = "Solicitudes de guardia / internación",
                Location = new Point(280, 180),
                Size = new Size(240, 35),
                Visible = rol == "administrativo"
            };
            btnSolicitudes.Click += (_, _) =>
            {
                using var ventana = new VistaSolicitudesGuardia();
                ventana.ShowDialog(this);
            };
            Controls.Add(btnSolicitudes);
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
