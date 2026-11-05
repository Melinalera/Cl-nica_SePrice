using System.Data;
using System.Globalization;
using System.Drawing.Printing;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaHistorialLiquidaciones : Form
    {
        private readonly int? idMedico;
        private readonly DataGridView liquidaciones = new()
        {
            Location = new Point(20, 100), Size = new Size(910, 190),
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        };
        private readonly DataGridView detalle = new()
        {
            Location = new Point(20, 350), Size = new Size(910, 190),
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        private readonly Label tituloDetalle = new()
        {
            Text = "Seleccioná una liquidación y presioná Ver detalle.",
            Location = new Point(20, 310), Size = new Size(700, 30)
        };
        private readonly Label aviso = new()
        {
            ForeColor = Color.Firebrick,
            Location = new Point(20, 560), Size = new Size(910, 50)
        };

        public VistaHistorialLiquidaciones(int? idMedico = null)
        {
            this.idMedico = idMedico;
            Text = "Historial de liquidaciones";
            ClientSize = new Size(950, 630);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            var volver = new Button
            {
                Text = "Volver", Location = new Point(12, 12), Size = new Size(75, 23),
                BackColor = Color.Teal, ForeColor = SystemColors.ControlLight,
                Font = new Font("Tahoma", 8.25F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            volver.Click += (_, _) => Close();
            var titulo = new Label
            {
                Text = "Historial de liquidaciones",
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(100, 15), Size = new Size(730, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var alcance = new Label
            {
                Text = idMedico.HasValue
                    ? "Liquidaciones confirmadas del médico seleccionado."
                    : "Liquidaciones confirmadas de todos los médicos.",
                Location = new Point(20, 65), Size = new Size(650, 25)
            };
            var actualizar = new Button
            {
                Text = "Actualizar", Location = new Point(810, 60), Size = new Size(120, 27)
            };
            actualizar.Click += (_, _) => CargarLiquidaciones();
            var verDetalle = new Button
            {
                Text = "Ver detalle", Location = new Point(810, 305), Size = new Size(120, 27)
            };
            verDetalle.Click += (_, _) => CargarDetalle();
            var comprobante = new Button
            {
                Text = "Vista previa del comprobante",
                Location = new Point(20, 590), Size = new Size(230, 27)
            };
            comprobante.Click += (_, _) => MostrarComprobante(imprimir: false);
            var imprimir = new Button
            {
                Text = "Imprimir / Guardar PDF",
                Location = new Point(700, 590), Size = new Size(230, 27)
            };
            imprimir.Click += (_, _) => MostrarComprobante(imprimir: true);
            aviso.Height = 28;
            liquidaciones.DataBindingComplete += (_, _) =>
            {
                if (liquidaciones.Columns["Total"] is DataGridViewColumn total)
                {
                    total.DefaultCellStyle.Format = "C2";
                    total.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-AR");
                }
            };
            detalle.DataBindingComplete += (_, _) =>
            {
                if (detalle.Columns["Importe"] is DataGridViewColumn importe)
                {
                    importe.DefaultCellStyle.Format = "C2";
                    importe.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-AR");
                }
            };
            Controls.AddRange(new Control[]
            {
                volver, titulo, alcance, actualizar, liquidaciones,
                tituloDetalle, verDetalle, detalle, aviso, comprobante, imprimir
            });
            Load += (_, _) => CargarLiquidaciones();
        }

        private void CargarLiquidaciones()
        {
            aviso.Text = "";
            detalle.DataSource = null;
            tituloDetalle.Text = "Seleccioná una liquidación y presioná Ver detalle.";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT l.id_liquidacion AS Numero,
                           CONCAT(u.apellido, ', ', u.nombre) AS Medico,
                           l.fecha_desde AS Desde, l.fecha_hasta AS Hasta,
                           l.fecha_emision AS Emision, l.importe_total AS Total
                    FROM liquidaciones_honorarios l
                    JOIN usuarios u ON u.id_usuario = l.id_medico
                    WHERE (@medico IS NULL OR l.id_medico = @medico)
                    ORDER BY l.fecha_emision DESC, l.id_liquidacion DESC
                    """;
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@medico", idMedico.HasValue ? (object)idMedico.Value : DBNull.Value);
                using var adaptador = new MySqlDataAdapter(comando);
                var datos = new DataTable();
                adaptador.Fill(datos);
                liquidaciones.DataSource = datos;
                if (datos.Rows.Count == 0)
                    aviso.Text = "No hay liquidaciones registradas para esta consulta.";
            }
            catch (MySqlException)
            {
                liquidaciones.DataSource = null;
                aviso.Text = "No se pudo cargar el historial. Revisá la conexión y las tablas de honorarios.";
            }
        }

        private void MostrarComprobante(bool imprimir)
        {
            aviso.Text = "";
            if (liquidaciones.CurrentRow?.DataBoundItem is not DataRowView seleccion)
            {
                aviso.Text = "Seleccioná una liquidación.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);
                using var cabeceraComando = new MySqlCommand("""
                    SELECT l.*, CONCAT(u.apellido, ', ', u.nombre) AS medico,
                           m.matricula, m.especialidad
                    FROM liquidaciones_honorarios l
                    JOIN medicos m ON m.id_usuario = l.id_medico
                    JOIN usuarios u ON u.id_usuario = l.id_medico
                    WHERE l.id_liquidacion = @id
                    """, conexion, transaccion);
                cabeceraComando.Parameters.AddWithValue("@id", seleccion["Numero"]);
                using var cabeceraAdaptador = new MySqlDataAdapter(cabeceraComando);
                using var cabecera = new DataTable();
                cabeceraAdaptador.Fill(cabecera);
                if (cabecera.Rows.Count == 0)
                {
                    aviso.Text = "La liquidación ya no está disponible. Actualizá el historial.";
                    return;
                }
                using var detalleComando = new MySqlCommand("""
                    SELECT a.id_atencion, a.fecha_atencion, t.dni_paciente,
                           a.practica, d.importe
                    FROM detalle_liquidaciones_honorarios d
                    JOIN atenciones_consultorios a ON a.id_atencion = d.id_atencion
                    JOIN `turnosConsultoriosMédicos` t ON t.id_turno = a.id_turno
                    WHERE d.id_liquidacion = @id
                    ORDER BY a.fecha_atencion, a.id_atencion
                    """, conexion, transaccion);
                detalleComando.Parameters.AddWithValue("@id", seleccion["Numero"]);
                using var detalleAdaptador = new MySqlDataAdapter(detalleComando);
                using var datos = new DataTable();
                detalleAdaptador.Fill(datos);
                transaccion.Commit();

                using var documento = new DocumentoLiquidacion(cabecera.Rows[0], datos);
                if (!documento.PrinterSettings.IsValid)
                {
                    aviso.Text = "Configurá una impresora o Microsoft Print to PDF en Windows.";
                    return;
                }
                if (imprimir)
                {
                    using var dialogo = new PrintDialog { Document = documento, UseEXDialog = true };
                    if (dialogo.ShowDialog(this) == DialogResult.OK)
                        documento.Print();
                }
                else
                {
                    using var vistaPrevia = new PrintPreviewDialog
                    {
                        Document = documento, Width = 1000, Height = 750,
                        StartPosition = FormStartPosition.CenterParent
                    };
                    vistaPrevia.ShowDialog(this);
                }
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudieron consultar los datos del comprobante.";
            }
            catch (InvalidPrinterException)
            {
                aviso.Text = "La impresora seleccionada no está disponible.";
            }
            catch (System.ComponentModel.Win32Exception)
            {
                aviso.Text = "No se pudo abrir la impresión. Revisá las impresoras de Windows.";
            }
        }

        private void CargarDetalle()
        {
            aviso.Text = "";
            detalle.DataSource = null;
            tituloDetalle.Text = "Seleccioná una liquidación y presioná Ver detalle.";
            if (liquidaciones.CurrentRow?.DataBoundItem is not DataRowView seleccion)
            {
                aviso.Text = "Seleccioná una liquidación.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT a.id_atencion AS Atencion, a.id_turno AS Turno,
                           t.dni_paciente AS DNI, a.fecha_atencion AS Fecha,
                           a.practica AS Practica, d.importe AS Importe
                    FROM detalle_liquidaciones_honorarios d
                    JOIN atenciones_consultorios a ON a.id_atencion = d.id_atencion
                    JOIN `turnosConsultoriosMédicos` t ON t.id_turno = a.id_turno
                    WHERE d.id_liquidacion = @liquidacion
                    ORDER BY a.fecha_atencion, a.id_atencion
                    """;
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@liquidacion", seleccion["Numero"]);
                using var adaptador = new MySqlDataAdapter(comando);
                var datos = new DataTable();
                adaptador.Fill(datos);
                detalle.DataSource = datos;
                tituloDetalle.Text = $"Liquidación N.º {seleccion["Numero"]} — {seleccion["Medico"]} — {datos.Rows.Count} atenciones";
                if (datos.Rows.Count == 0)
                    aviso.Text = "Esta liquidación no tiene atenciones registradas en su detalle.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo cargar el detalle. Volvé a intentar la consulta.";
            }
        }
    }
}
