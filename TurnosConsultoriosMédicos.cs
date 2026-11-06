using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Clínica_SePrice.vistas;

namespace Clínica_SePrice
{
    public partial class TurnosConsultoriosMédicos : Form
    {
        private int? idPaciente;
        private string dniSeleccionado = "";
        private readonly Label lblPaciente = new()
        {
            Location = new Point(466, 310), Size = new Size(285, 45)
        };
        private readonly Button btnBuscarPaciente = new()
        {
            Text = "Buscar / Registrar paciente",
            Location = new Point(466, 265), Size = new Size(265, 30)
        };
        private readonly TextBox txtDni = new()
        {
            Name = "txtDni", Location = new Point(466, 230), Width = 265, MaxLength = 20
        };
        private readonly Label lblAviso = new()
        {
            Location = new Point(34, 365), Size = new Size(730, 35), ForeColor = Color.Firebrick
        };

        public TurnosConsultoriosMédicos()
        {
            InitializeComponent();
            Controls.Add(new Label
            {
                Text = "DNI del paciente", Location = new Point(466, 203), AutoSize = true
            });
            Controls.Add(new Label
            {
                Text = "Día de la reserva", Location = new Point(215, 180), AutoSize = true
            });
            Controls.Add(txtDni);
            Controls.Add(lblAviso);
            Controls.Add(lblPaciente);
            Controls.Add(btnBuscarPaciente);
            btnBuscarPaciente.Click += (_, _) => BuscarPaciente();
            txtDni.TextChanged += (_, _) =>
            {
                idPaciente = null;
                dniSeleccionado = "";
                lblPaciente.Text = "";
            };
            button3.Click += (_, _) => Close();
        }

        private void BuscarPaciente()
        {
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Text = "";
            string dni = txtDni.Text.Trim();
            if (dni.Length == 0 || !dni.All(caracter => caracter >= '0' && caracter <= '9'))
            {
                lblAviso.Text = "Ingresá el DNI con números, sin puntos ni espacios.";
                txtDni.Focus();
                return;
            }
            using var ficha = new VistaFichaPaciente(dni);
            if (ficha.ShowDialog(this) == DialogResult.OK)
            {
                idPaciente = ficha.IdPaciente;
                dniSeleccionado = dni;
                lblPaciente.Text = $"Paciente N.º {ficha.IdPaciente}\n{ficha.NombreCompleto}";
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtDni.Clear();
            txtDni.ReadOnly = false;
            btnBuscarPaciente.Enabled = true;
            foreach (RadioButton especialidad in new[] { Cardiología, Dermatología, Gastroenterología, Neumonología, Neurología })
            {
                especialidad.Checked = false;
                especialidad.Enabled = true;
            }
            monthCalendar1.MinDate = DateTime.Today;
            monthCalendar1.SetDate(DateTime.Today);
            monthCalendar1.Enabled = true;
            btnAceptar.Enabled = true;
            lblAviso.Text = "";
            txtDni.Focus();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Text = "";
            string dni = txtDni.Text.Trim();
            if (dni.Length == 0 || !dni.All(caracter => caracter >= '0' && caracter <= '9'))
            {
                lblAviso.Text = "Ingresá el DNI del paciente con números, sin puntos ni espacios.";
                txtDni.Focus();
                return;
            }
            if (!idPaciente.HasValue || dniSeleccionado != dni)
            {
                lblAviso.Text = "Buscá o registrá al paciente antes de guardar la reserva.";
                return;
            }
            RadioButton? seleccion = new[] { Cardiología, Dermatología, Gastroenterología, Neumonología, Neurología }
                .FirstOrDefault(especialidad => especialidad.Checked);
            if (seleccion == null)
            {
                lblAviso.Text = "Seleccioná una especialidad.";
                return;
            }
            DateTime fecha = monthCalendar1.SelectionStart.Date;
            if (fecha < DateTime.Today)
            {
                lblAviso.Text = "La reserva debe tener fecha de hoy o posterior.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var comando = new MySqlCommand("""
                    INSERT INTO `turnosConsultoriosMédicos`
                        (especialidad, fecha, dni_paciente, estado)
                    SELECT @especialidad, @fecha, p.dni, 'Reservado'
                    FROM pacientes p WHERE p.id_paciente = @paciente AND p.dni = @dni
                    """, conexion);
                comando.Parameters.AddWithValue("@especialidad", seleccion.Text.Trim());
                comando.Parameters.AddWithValue("@fecha", fecha);
                comando.Parameters.AddWithValue("@dni", dni);
                comando.Parameters.AddWithValue("@paciente", idPaciente.Value);
                if (comando.ExecuteNonQuery() != 1)
                {
                    lblAviso.Text = "La ficha del paciente cambió. Volvé a buscar el DNI.";
                    return;
                }

                // Conservar la reserva visible y evitar enviarla dos veces.
                btnAceptar.Enabled = false;
                txtDni.ReadOnly = true;
                btnBuscarPaciente.Enabled = false;
                monthCalendar1.Enabled = false;
                foreach (RadioButton especialidad in new[] { Cardiología, Dermatología, Gastroenterología, Neumonología, Neurología })
                    especialidad.Enabled = false;
                lblAviso.ForeColor = Color.DarkGreen;
                lblAviso.Text = $"Reserva N.º {comando.LastInsertedId} registrada para el {fecha:dd/MM/yyyy}.";
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudo confirmar la reserva. Consultá los turnos en MySQL antes de reintentar.";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
