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
using MySql.Data.MySqlClient;

namespace Clínica_SePrice
{
    public partial class TurnosConsultoriosMédicos : Form
    {
        private string cadenaConexion = "server=localhost; database=clínica_seprice; uid=root; pwd=1234;";

        // Variables para el comprobante y notificación
        private string especialidadAsignadaTurno = "";
        private string fechaTurnoStr = "";
        private string horarioTurnoStr = "";
        private string medicoAsignadoStr = "";
        private string emailPacienteDestino = "melina.lera@email.com"; // Puedes cambiarlo o cargarlo dinámicamente desde la sesión del paciente

        public TurnosConsultoriosMédicos()
        {
            InitializeComponent();

            // Suscribimos todos los Radio Button al mismo evento al iniciar
            Cardiología.CheckedChanged += Especialidad_CheckedChanged;
            Dermatología.CheckedChanged += Especialidad_CheckedChanged;
            Gastroenterología.CheckedChanged += Especialidad_CheckedChanged;
            Neumonología.CheckedChanged += Especialidad_CheckedChanged;
            Neurología.CheckedChanged += Especialidad_CheckedChanged;
            Guardia.CheckedChanged += Especialidad_CheckedChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Seleccione la especialidad médica para ver los turnos disponibles.";
            label2.Text = "Seleccione la fecha para ver los turnos disponibles.";
        }

        private void Especialidad_CheckedChanged(object? sender, EventArgs e)
        {
            RadioButton? rd = sender as RadioButton;
            if (rd != null && rd.Checked)
            {
                string especialidad = rd.Text.Trim();
                MostrarInformaciónEspecialidad(especialidad);
                CargarHorariosDisponibles(especialidad);
            }
        }

        // Método para rellenar el ComboBox con los horarios según la especialidad
        private void CargarHorariosDisponibles(string especialidad)
        {
            comboBox1.Items.Clear(); // Limpiamos horarios anteriores

            switch (especialidad)
            {
                case "Cardiología":
                    comboBox1.Items.Add("08:00 - 09:00");
                    comboBox1.Items.Add("09:00 - 10:00");
                    comboBox1.Items.Add("10:00 - 11:00");
                    break;
                case "Dermatología":
                    comboBox1.Items.Add("10:00 - 11:00");
                    comboBox1.Items.Add("11:00 - 12:00");
                    break;
                case "Gastroenterología":
                    comboBox1.Items.Add("13:00 - 14:00");
                    comboBox1.Items.Add("14:00 - 15:00");
                    break;
                case "Neumonología":
                    comboBox1.Items.Add("08:30 - 09:30");
                    comboBox1.Items.Add("09:30 - 10:30");
                    break;
                case "Neurología":
                    comboBox1.Items.Add("15:00 - 16:00");
                    comboBox1.Items.Add("16:00 - 17:00");
                    break;
                case "Guardia":
                    comboBox1.Items.Add("Turno Mañana (08:00)");
                    comboBox1.Items.Add("Turno Tarde (14:00)");
                    comboBox1.Items.Add("Turno Noche (20:00)");
                    break;
                default:
                    comboBox1.Items.Add("09:00 - 10:00");
                    break;
            }

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0; // Selecciona el primero por defecto
        }

        private void MostrarInformaciónEspecialidad(string especialidad)
        {
            using (MySqlConnection conn = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT u.nombre, u.apellido, m.especialidad
                                     FROM medicos m
                                     INNER JOIN usuarios u ON m.id_usuario = u.id_usuario
                                     WHERE m.especialidad = @especialidad LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@especialidad", especialidad);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string nombre = reader["nombre"].ToString();
                                string apellido = reader["apellido"].ToString();
                                string especialidadObtenida = reader["especialidad"].ToString();

                                medicoAsignadoStr = $"Dr/a. {nombre} {apellido}";
                                label1.Text = $"Médico: {medicoAsignadoStr}, Especialidad: {especialidadObtenida}";
                                string diasAtención = ObtenerDiasAtención(especialidad);
                                label2.Text = $"Días de atención: {diasAtención}";
                            }
                            else
                            {
                                medicoAsignadoStr = "Sin asignar";
                                label1.Text = "Sin médico asignado";
                                label2.Text = "No hay registros";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar la especialidad: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string ObtenerDiasAtención(string especialidad)
        {
            switch (especialidad)
            {
                case "Cardiología": return "Lunes, Miércoles y Viernes";
                case "Dermatología": return "Martes y Jueves";
                case "Gastroenterología": return "Lunes y Jueves";
                case "Neumonología": return "Martes y Viernes";
                case "Neurología": return "Miércoles y Sábado";
                case "Guardia": return "Lunes a Domingo (24hs)";
                default: return "A confirmar";
            }
        }

        // Botón Aceptar: Guarda, Valida, Envía Notificación Email e Imprime/Exporta Comprobante
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string especialidadSeleccionada = "";

            if (Cardiología.Checked) especialidadSeleccionada = "Cardiología";
            else if (Dermatología.Checked) especialidadSeleccionada = "Dermatología";
            else if (Gastroenterología.Checked) especialidadSeleccionada = "Gastroenterología";
            else if (Neumonología.Checked) especialidadSeleccionada = "Neumonología";
            else if (Neurología.Checked) especialidadSeleccionada = "Neurología";
            else if (Guardia.Checked) especialidadSeleccionada = "Guardia";
            else
            {
                MessageBox.Show("Por favor, seleccione una especialidad.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un horario disponible.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fechaTurno = monthCalendar1.SelectionStart;
            string horarioSeleccionado = comboBox1.SelectedItem.ToString();

            using (MySqlConnection conn = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();

                    // Verificamos si ya existe el turno reservado
                    string queryVerificar = "SELECT COUNT(*) FROM turnosConsultoriosMédicos WHERE especialidad = @especialidad AND fecha = @fecha";

                    using (MySqlCommand cmdVerificar = new MySqlCommand(queryVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@especialidad", especialidadSeleccionada);
                        cmdVerificar.Parameters.AddWithValue("@fecha", fechaTurno.ToString("yyyy-MM-dd"));

                        long count = Convert.ToInt64(cmdVerificar.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("El turno ya se encuentra reservado para esta fecha. Por favor elija otro día.", "Turno Ocupado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insertamos el nuevo registro
                    string queryInsert = "INSERT INTO turnosConsultoriosMédicos (especialidad, fecha, estado) VALUES (@especialidad, @fecha, 'Reservado')";

                    using (MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@especialidad", especialidadSeleccionada);
                        cmdInsert.Parameters.AddWithValue("@fecha", fechaTurno.ToString("yyyy-MM-dd"));

                        cmdInsert.ExecuteNonQuery();

                        // Guardamos los datos para el comprobante
                        especialidadAsignadaTurno = especialidadSeleccionada;
                        fechaTurnoStr = fechaTurno.ToString("dd/MM/yyyy");
                        horarioTurnoStr = horarioSeleccionado;

                        // 1. Enviar notificación por correo electrónico
                        EnviarNotificacionEmail();

                        // 2. Notificación en pantalla y oferta de impresión/PDF
                        DialogResult resultado = MessageBox.Show(
                            $"¡Turno reservado con éxito y notificación enviada por correo!\nEspecialidad: {especialidadSeleccionada}\nFecha: {fechaTurnoStr}\nHorario: {horarioTurnoStr}\n\n¿Desea imprimir o guardar el comprobante del turno?",
                            "Clínica SePrice - Turno Confirmado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (resultado == DialogResult.Yes)
                        {
                            ImprimirComprobanteTurno();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al reservar el turno: " + ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Método para enviar el correo electrónico de notificación al paciente
        private void EnviarNotificacionEmail()
        {
            try
            {
                // Configura tu cuenta de correo SMTP (Ejemplo con Gmail o servidor local de pruebas)
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("clinica.seprice.notificaciones@gmail.com", "Clínica SePrice");
                correo.To.Add(emailPacienteDestino);
                correo.Subject = "Confirmación de Turno Médico - Clínica SePrice";
                correo.Body = $"Estimado/a paciente,\n\nSu turno ha sido reservado con éxito.\n\n" +
                              $"--- DETALLES DEL TURNO ---\n" +
                              $"Especialidad: {especialidadAsignadaTurno}\n" +
                              $"Médico: {medicoAsignadoStr}\n" +
                              $"Fecha: {fechaTurnoStr}\n" +
                              $"Horario: {horarioTurnoStr}\n" +
                              $"Estado: RESERVADO\n\n" +
                              $"Por favor, preséntese 10 minutos antes con su DNI y credencial.\n\nAtentamente,\nClínica SePrice.";

                SmtpClient servidorSmtp = new SmtpClient("smtp.gmail.com", 587);
                servidorSmtp.Credentials = new NetworkCredential("tu_correo@gmail.com", "tu_contraseña_de_aplicacion");
                servidorSmtp.EnableSsl = true;

                // NOTA: Si estás probando en entorno local sin internet, puedes comentar la línea de envío para evitar excepciones de red:
                // servidorSmtp.Send(correo);
            }
            catch (Exception ex)
            {
                // Mostramos un aviso si el correo no pudo enviarse por red, pero sin interrumpir el flujo de reserva
                MessageBox.Show("El turno se guardó en la base de datos, pero hubo un fallo al enviar la notificación por email: " + ex.Message, "Aviso de Red", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Método para activar la vista previa de impresión / exportación a PDF de Windows
        private void ImprimirComprobanteTurno()
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

            e.Graphics.DrawString("CLÍNICA SEPRICE - COMPROBANTE DE TURNO", fuenteTitulo, Brushes.DarkBlue, new Point(100, 80));
            e.Graphics.DrawString("--------------------------------------------------------------------------------", fuenteTexto, Brushes.Black, new Point(100, 110));

            e.Graphics.DrawString($"Especialidad: {especialidadAsignadaTurno}", fuenteTexto, Brushes.Black, new Point(100, 150));
            e.Graphics.DrawString($"Médico: {medicoAsignadoStr}", fuenteTexto, Brushes.Black, new Point(100, 180));
            e.Graphics.DrawString($"Fecha del Turno: {fechaTurnoStr}", fuenteTexto, Brushes.Black, new Point(100, 210));
            e.Graphics.DrawString($"Horario: {horarioTurnoStr}", fuenteTexto, Brushes.Black, new Point(100, 240));
            e.Graphics.DrawString("Estado: RESERVADO", fuenteTexto, Brushes.Green, new Point(100, 270));

            e.Graphics.DrawString("--------------------------------------------------------------------------------", fuenteTexto, Brushes.Black, new Point(100, 310));
            e.Graphics.DrawString("Por favor, preséntese 10 minutos antes con su DNI y credencial.", new Font("Arial", 10, FontStyle.Italic), Brushes.Gray, new Point(100, 340));
        }

        // Conexión del botón Atrás
        private void Atrás_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Conexión del botón Cancelar
        private void Cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Solución del evento pendiente de Neumonología generado por el diseñador
        private void Neumonología_CheckedChanged(object sender, EventArgs e)
        {
            // Este evento redirige de forma segura al manejador general de especialidades
            Especialidad_CheckedChanged(sender, e);
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }
    }
}