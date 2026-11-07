using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clínica_SePrice
{
    public partial class Estudios_Clínicos : Form
    {
        private string cadenaConexion = "server=localhost; database=clínica_seprice; uid=root; pwd=1234;";

        // Variables para el comprobante y notificación
        private string estudioAsignadoStr = "";
        private string fechaEstudioStr = "";
        private string horarioEstudioStr = "";
        private string emailPacienteDestino = "melina.lera@email.com"; // Se puede cambiar o cargar dinámicamente

        public Estudios_Clínicos()
        {
            InitializeComponent();

            // Suscribimos todos los RadioButton al evento unificado al iniciar
            Laboratorio.CheckedChanged += Estudio_CheckedChanged;
            Electrocardiograma.CheckedChanged += Estudio_CheckedChanged;
            Ecografía.CheckedChanged += Estudio_CheckedChanged;
            Radiografía.CheckedChanged += Estudio_CheckedChanged;
            Tomografía.CheckedChanged += Estudio_CheckedChanged;
        }

        private void Estudios_Clínicos_Load(object sender, EventArgs e)
        {
            label1.Text = "Seleccione el estudio clínico para ver los horarios disponibles.";
            label2.Text = "Días de atención y requisitos.";
        }

        private void Estudio_CheckedChanged(object? sender, EventArgs e)
        {
            RadioButton? rd = sender as RadioButton;
            if (rd != null && rd.Checked)
            {
                string estudio = rd.Text.Trim();
                MostrarInformaciónEstudio(estudio);
                CargarHorariosDisponibles(estudio);
            }
        }

        // Método para rellenar el ComboBox con los horarios según el estudio clínico
        private void CargarHorariosDisponibles(string estudio)
        {
            comboBox1.Items.Clear(); // Limpiamos horarios anteriores

            switch (estudio)
            {
                case "Laboratorio":
                    comboBox1.Items.Add("07:00 - 08:00 (Ayuno)");
                    comboBox1.Items.Add("08:00 - 09:00 (Ayuno)");
                    break;
                case "Electrocardiograma":
                    comboBox1.Items.Add("09:00 - 10:00");
                    comboBox1.Items.Add("10:00 - 11:00");
                    break;
                case "Ecografía":
                    comboBox1.Items.Add("11:00 - 12:00");
                    comboBox1.Items.Add("12:00 - 13:00");
                    break;
                case "Radiografía":
                    comboBox1.Items.Add("13:00 - 14:00");
                    comboBox1.Items.Add("14:00 - 15:00");
                    break;
                case "Tomografía":
                    comboBox1.Items.Add("15:00 - 16:00");
                    comboBox1.Items.Add("16:00 - 17:00");
                    break;
                default:
                    comboBox1.Items.Add("09:00 - 10:00");
                    break;
            }

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0; // Selecciona el primero por defecto
        }

        private void MostrarInformaciónEstudio(string estudio)
        {
            string diasAtención = ObtenerDiasAtención(estudio);
            label1.Text = $"Estudio seleccionado: {estudio}";
            label2.Text = $"Días habilitados: {diasAtención}";
        }

        private string ObtenerDiasAtención(string estudio)
        {
            switch (estudio)
            {
                case "Laboratorio": return "Lunes a Viernes";
                case "Electrocardiograma": return "Lunes, Miércoles y Viernes";
                case "Ecografía": return "Martes y Jueves";
                case "Radiografía": return "Lunes a Sábado";
                case "Tomografía": return "Miércoles y Viernes";
                default: return "A confirmar";
            }
        }

        // Botón Aceptar: Guarda, Valida, Envía Notificación Email e Imprime/Exporta Comprobante
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string estudioSeleccionado = "";

            if (Laboratorio.Checked) estudioSeleccionado = "Laboratorio";
            else if (Electrocardiograma.Checked) estudioSeleccionado = "Electrocardiograma";
            else if (Ecografía.Checked) estudioSeleccionado = "Ecografía";
            else if (Radiografía.Checked) estudioSeleccionado = "Radiografía";
            else if (Tomografía.Checked) estudioSeleccionado = "Tomografía";
            else
            {
                MessageBox.Show("Por favor, seleccione un estudio clínico.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un horario disponible.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fechaEstudio = monthCalendar1.SelectionStart;
            string horarioSeleccionado = comboBox1.SelectedItem.ToString();

            // Extraemos los primeros 5 caracteres de la hora (ej. "07:00" de "07:00 - 08:00 (Ayuno)") para que encaje en el campo TIME
            string horaInicio = horarioSeleccionado.Substring(0, 5);

            using (MySqlConnection conn = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();

                    // Verificamos si ya existe el turno para ese estudio, fecha y hora exacta
                    string queryVerificar = "SELECT COUNT(*) FROM turnosEstudiosClínicos WHERE estudio = @estudio AND fecha = @fecha AND hora = @hora";

                    using (MySqlCommand cmdVerificar = new MySqlCommand(queryVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@estudio", estudioSeleccionado);
                        cmdVerificar.Parameters.AddWithValue("@fecha", fechaEstudio.ToString("yyyy-MM-dd"));
                        cmdVerificar.Parameters.AddWithValue("@hora", horaInicio);

                        long count = Convert.ToInt64(cmdVerificar.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Ya existe un turno reservado para este estudio en este horario y fecha. Por favor elija otro.", "Turno Ocupado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insertamos el nuevo registro del estudio clínico incluyendo la hora
                    string queryInsert = "INSERT INTO turnosEstudiosClínicos (estudio, fecha, hora, estado) VALUES (@estudio, @fecha, @hora, 'Reservado')";

                    using (MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@estudio", estudioSeleccionado);
                        cmdInsert.Parameters.AddWithValue("@fecha", fechaEstudio.ToString("yyyy-MM-dd"));
                        cmdInsert.Parameters.AddWithValue("@hora", horaInicio);

                        cmdInsert.ExecuteNonQuery();

                        // Guardamos los datos para el comprobante
                        estudioAsignadoStr = estudioSeleccionado;
                        fechaEstudioStr = fechaEstudio.ToString("dd/MM/yyyy");
                        horarioEstudioStr = horarioSeleccionado;

                        // 1. Enviar notificación por correo electrónico
                        EnviarNotificacionEmail();

                        // 2. Notificación en pantalla y oferta de impresión/PDF
                        DialogResult resultado = MessageBox.Show(
                            $"¡Estudio clínico reservado con éxito y notificación enviada por correo!\nEstudio: {estudioSeleccionado}\nFecha: {fechaEstudioStr}\nHorario: {horarioEstudioStr}\n\n¿Desea imprimir o guardar el comprobante del estudio?",
                            "Clínica SePrice - Estudio Confirmado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (resultado == DialogResult.Yes)
                        {
                            ImprimirComprobanteEstudio();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al reservar el estudio clínico: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Método para enviar el correo electrónico de notificación al paciente
        private void EnviarNotificacionEmail()
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("clinica.seprice.notificaciones@gmail.com", "Clínica SePrice");
                correo.To.Add(emailPacienteDestino);
                correo.Subject = "Confirmación de Turno para Estudio Clínico - Clínica SePrice";
                correo.Body = $"Estimado/a paciente,\n\nSu turno para estudio clínico ha sido reservado con éxito.\n\n" +
                              $"--- DETALLES DEL ESTUDIO ---\n" +
                              $"Estudio: {estudioAsignadoStr}\n" +
                              $"Fecha: {fechaEstudioStr}\n" +
                              $"Horario: {horarioEstudioStr}\n" +
                              $"Estado: RESERVADO\n\n" +
                              $"Por favor, preséntese con DNI y la orden médica correspondiente.\n\nAtentamente,\nClínica SePrice.";

                SmtpClient servidorSmtp = new SmtpClient("smtp.gmail.com", 587);
                servidorSmtp.Credentials = new NetworkCredential("tu_correo@gmail.com", "tu_contraseña_de_aplicacion");
                servidorSmtp.EnableSsl = true;

                // Descomentar la siguiente línea si deseas habilitar el envío real por red:
                // servidorSmtp.Send(correo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("El estudio se guardó en la base de datos, pero hubo un fallo al enviar la notificación por email: " + ex.Message, "Aviso de Red", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Método para activar la vista previa de impresión / exportación a PDF
        private void ImprimirComprobanteEstudio()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(DocumentoComprobante_PrintPage);

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = pd;
            preview.ShowDialog();
        }

        // Diseño visual del comprobante que se imprime o se guarda como PDF
        private void DocumentoComprobante_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font fuenteTitulo = new Font("Arial", 16, FontStyle.Bold);
            Font fuenteTexto = new Font("Arial", 12, FontStyle.Regular);

            e.Graphics.DrawString("CLÍNICA SEPRICE - COMPROBANTE DE ESTUDIO CLÍNICO", fuenteTitulo, Brushes.DarkBlue, new Point(100, 80));
            e.Graphics.DrawString("--------------------------------------------------------------------------------", fuenteTexto, Brushes.Black, new Point(100, 110));
            e.Graphics.DrawString($"Estudio: {estudioAsignadoStr}", fuenteTexto, Brushes.Black, new Point(100, 150));
            e.Graphics.DrawString($"Fecha del Estudio: {fechaEstudioStr}", fuenteTexto, Brushes.Black, new Point(100, 190));
            e.Graphics.DrawString($"Horario: {horarioEstudioStr}", fuenteTexto, Brushes.Black, new Point(100, 230));
            e.Graphics.DrawString("Estado: RESERVADO", fuenteTexto, Brushes.Green, new Point(100, 270));

            e.Graphics.DrawString("--------------------------------------------------------------------------------", fuenteTexto, Brushes.Black, new Point(100, 310));
            e.Graphics.DrawString("Por favor, preséntese con su DNI, orden médica y preparación requerida.", new Font("Arial", 10, FontStyle.Italic), Brushes.Gray, new Point(100, 340));
        }

        // Botón Atrás
        private void btnAtrás_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Botón Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Métodos auxiliares de eventos del diseñador (redirigidos o limpios)
        private void Laboratorio_Click(object sender, EventArgs e) { }
        private void Electrocardiograma_Click(object sender, EventArgs e) { }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { }
        private void Ecografía_Click(object sender, EventArgs e) { }
        private void radioButton4_CheckedChanged(object sender, EventArgs e) { }
        private void Radiografía_Click(object sender, EventArgs e) { }
        private void Tomografía_Click(object sender, EventArgs e) { }
    }
}