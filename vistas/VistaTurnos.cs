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
            var btnAtencion = new Button
            {
                Name = "btnRegistrarAtencion",
                Text = "Registrar atención",
                Location = new Point(286, 400),
                Size = new Size(239, 27),
                Visible = rol == "administrativo"
            };
            btnAtencion.Click += (_, _) =>
            {
                using var ventana = new VistaRegistrarAtencion();
                ventana.ShowDialog(this);
            };
            Controls.Add(btnAtencion);
            var btnAgendas = new Button
            {
                Name = "btnAgendas",
                Text = "Configurar agendas",
                Location = new Point(286, 273),
                Size = new Size(239, 27),
                Visible = rol == "administrativo"
            };
            btnAgendas.Click += (_, _) =>
            {
                using var ventana = new VistaAgendasConsultorios();
                ventana.ShowDialog(this);
            };
            Controls.Add(btnAgendas);

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
            using var turnosConsultoriosMédicos = new TurnosConsultoriosMédicos();
            turnosConsultoriosMédicos.ShowDialog(this);
        }

        private void EstudiosClínicos_Click(object sender, EventArgs e)
        {
            using var ventana = new Estudios_Clínicos();
            ventana.ShowDialog(this);
        }

        private void btnAcreditar_Click(object sender, EventArgs e)
        {
            using var ventana = new VistaAcreditacion();
            ventana.ShowDialog(this);
        }
    }
}
