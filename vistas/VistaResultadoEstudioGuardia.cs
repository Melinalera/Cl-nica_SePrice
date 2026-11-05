using System.Data;
using System.Drawing.Printing;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaResultadoEstudioGuardia : Form
    {
        private readonly int idSolicitud;
        private bool resultadoGuardado;
        private readonly Label detalle = new()
        {
            Location = new Point(25, 65), Size = new Size(800, 55)
        };
        private readonly ComboBox medico = new()
        {
            Location = new Point(180, 130), Size = new Size(620, 23),
            DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false
        };
        private readonly TextBox resultado = new()
        {
            Location = new Point(25, 200), Size = new Size(800, 190),
            Multiline = true, ScrollBars = ScrollBars.Vertical,
            MaxLength = 16000, ReadOnly = true
        };
        private readonly Label fechas = new()
        {
            Location = new Point(25, 405), Size = new Size(800, 45)
        };
        private readonly TextBox receptor = new()
        {
            Location = new Point(180, 462), Width = 620, MaxLength = 100, ReadOnly = true
        };
        private readonly Label aviso = new()
        {
            Location = new Point(25, 505), Size = new Size(800, 50), ForeColor = Color.Firebrick
        };
        private readonly Button guardar = new()
        {
            Text = "Guardar resultado", Location = new Point(25, 570),
            Size = new Size(180, 30), Enabled = false
        };
        private readonly Button entregar = new()
        {
            Text = "Registrar entrega", Location = new Point(620, 570),
            Size = new Size(180, 30), Enabled = false
        };

        public VistaResultadoEstudioGuardia(int idSolicitud)
        {
            this.idSolicitud = idSolicitud;
            Text = $"Resultado de solicitud N.º {idSolicitud}";
            ClientSize = new Size(850, 625);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            var volver = new Button
            {
                Text = "Volver", Location = new Point(12, 12), Size = new Size(75, 23),
                BackColor = Color.Teal, ForeColor = SystemColors.ControlLight,
                Font = new Font("Tahoma", 8.25F, FontStyle.Bold), UseVisualStyleBackColor = false
            };
            volver.Click += (_, _) => Close();
            guardar.Click += (_, _) => GuardarResultado();
            entregar.Click += (_, _) => RegistrarEntrega();
            var vistaPrevia = new Button
            {
                Text = "Comprobante de retiro", Location = new Point(215, 570), Size = new Size(190, 30)
            };
            vistaPrevia.Click += (_, _) => MostrarComprobante(imprimir: false);
            var imprimir = new Button
            {
                Text = "Imprimir / Guardar PDF", Location = new Point(415, 570), Size = new Size(195, 30)
            };
            imprimir.Click += (_, _) => MostrarComprobante(imprimir: true);
            Controls.AddRange(new Control[]
            {
                volver,
                new Label
                {
                    Text = "Resultado del estudio", Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                    Location = new Point(100, 15), Size = new Size(650, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                },
                detalle,
                new Label { Text = "Profesional responsable", Location = new Point(25, 133), AutoSize = true },
                medico,
                new Label { Text = "Resultado informado por el profesional", Location = new Point(25, 173), AutoSize = true },
                resultado, fechas,
                new Label { Text = "Nombre de quien recibe", Location = new Point(25, 465), AutoSize = true },
                receptor, aviso, guardar, entregar, vistaPrevia, imprimir
            });
            Load += (_, _) => CargarResultado();
        }

        private void CargarResultado()
        {
            guardar.Enabled = false;
            entregar.Enabled = false;
            medico.Enabled = false;
            resultado.ReadOnly = true;
            receptor.ReadOnly = true;
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var profesionales = new MySqlDataAdapter("""
                    SELECT m.id_usuario, CONCAT(u.apellido, ', ', u.nombre) AS nombre
                    FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                    ORDER BY u.apellido, u.nombre, m.id_usuario
                    """, conexion);
                var medicos = new DataTable();
                profesionales.Fill(medicos);
                medico.DisplayMember = "nombre";
                medico.ValueMember = "id_usuario";
                medico.DataSource = medicos;
                medico.SelectedIndex = -1;

                using var consulta = new MySqlCommand("""
                    SELECT s.estado, s.nombre_paciente, s.dni_paciente, s.estudio_solicitado,
                           r.id_medico, r.resultado, r.fecha_resultado, r.fecha_entrega, r.receptor
                    FROM solicitudes_estudios_guardia s
                    LEFT JOIN resultados_estudios_guardia r ON r.id_solicitud = s.id_solicitud
                    WHERE s.id_solicitud = @id
                    """, conexion);
                consulta.Parameters.AddWithValue("@id", idSolicitud);
                using var adaptador = new MySqlDataAdapter(consulta);
                using var datos = new DataTable();
                adaptador.Fill(datos);
                if (datos.Rows.Count == 0)
                {
                    aviso.Text = "La solicitud ya no está disponible.";
                    return;
                }
                DataRow fila = datos.Rows[0];
                detalle.Text = $"Solicitud N.º {idSolicitud} — {fila["estudio_solicitado"]}\nPaciente: {fila["nombre_paciente"]} — DNI: {fila["dni_paciente"]}";
                resultadoGuardado = fila["resultado"] != DBNull.Value;
                if (resultadoGuardado)
                {
                    resultado.Text = (string)fila["resultado"];
                    medico.SelectedValue = fila["id_medico"];
                    bool entregado = fila["fecha_entrega"] != DBNull.Value;
                    fechas.Text = $"Resultado registrado: {((DateTime)fila["fecha_resultado"]):dd/MM/yyyy HH:mm}";
                    if (entregado)
                    {
                        receptor.Text = (string)fila["receptor"];
                        fechas.Text += $"\nEntregado: {((DateTime)fila["fecha_entrega"]):dd/MM/yyyy HH:mm}";
                    }
                    else if ((string)fila["estado"] == "Realizado")
                    {
                        receptor.Clear();
                        receptor.ReadOnly = false;
                        entregar.Enabled = true;
                    }
                    return;
                }
                if ((string)fila["estado"] != "Realizado")
                {
                    aviso.Text = "El estudio debe estar Realizado para cargar un resultado.";
                    return;
                }
                fechas.Text = "Resultado pendiente de carga.";
                medico.Enabled = true;
                resultado.ReadOnly = false;
                guardar.Enabled = medicos.Rows.Count > 0;
                if (medicos.Rows.Count == 0)
                    aviso.Text = "No hay profesionales registrados en la tabla medicos.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo cargar el resultado. Revisá la conexión y el script 07.";
            }
        }

        private void GuardarResultado()
        {
            aviso.Text = "";
            if (medico.SelectedValue is not int idMedico || string.IsNullOrWhiteSpace(resultado.Text))
            {
                aviso.Text = "Seleccioná al profesional responsable e ingresá el resultado informado.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                // Registrar una única vez y verificar el estado vigente en la base.
                using var comando = new MySqlCommand("""
                    INSERT INTO resultados_estudios_guardia (id_solicitud, id_medico, resultado)
                    SELECT s.id_solicitud, @medico, @resultado
                    FROM solicitudes_estudios_guardia s
                    WHERE s.id_solicitud = @id AND s.estado = 'Realizado'
                      AND EXISTS (SELECT 1 FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                                  WHERE m.id_usuario = @medico AND u.rol = 'medico')
                    """, conexion);
                comando.Parameters.AddWithValue("@id", idSolicitud);
                comando.Parameters.AddWithValue("@medico", idMedico);
                comando.Parameters.AddWithValue("@resultado", resultado.Text.Trim());
                if (comando.ExecuteNonQuery() != 1)
                {
                    aviso.Text = "Revisá que el estudio esté Realizado y el profesional tenga rol de médico.";
                    return;
                }
                CargarResultado();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                CargarResultado();
                aviso.Text = "Esta solicitud ya tenía un resultado registrado. Se muestra el resultado guardado.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar el guardado. Cerrá y consultá nuevamente antes de reintentar.";
            }
        }

        private void MostrarComprobante(bool imprimir)
        {
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                using var comando = new MySqlCommand("""
                    SELECT s.id_solicitud, s.nombre_paciente, s.dni_paciente,
                           s.estudio_solicitado, s.origen, s.referencia_orden, s.fecha_solicitud,
                           r.fecha_resultado, r.fecha_entrega, r.receptor
                    FROM solicitudes_estudios_guardia s
                    LEFT JOIN resultados_estudios_guardia r ON r.id_solicitud = s.id_solicitud
                    WHERE s.id_solicitud = @id AND s.estado = 'Realizado'
                    """, conexion);
                comando.Parameters.AddWithValue("@id", idSolicitud);
                using var adaptador = new MySqlDataAdapter(comando);
                using var datos = new DataTable();
                adaptador.Fill(datos);
                if (datos.Rows.Count == 0)
                {
                    aviso.Text = "El estudio debe estar Realizado para emitir el comprobante de retiro.";
                    return;
                }
                using var documento = new DocumentoRetiroEstudio(datos.Rows[0]);
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

        private void RegistrarEntrega()
        {
            aviso.Text = "";
            if (!resultadoGuardado || string.IsNullOrWhiteSpace(receptor.Text))
            {
                aviso.Text = "Ingresá el nombre de quien recibe el resultado.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var comando = new MySqlCommand("""
                    UPDATE resultados_estudios_guardia r
                    SET r.fecha_entrega = CURRENT_TIMESTAMP, r.receptor = @receptor
                    WHERE r.id_solicitud = @id AND r.fecha_entrega IS NULL
                      AND EXISTS (SELECT 1 FROM solicitudes_estudios_guardia s
                                  WHERE s.id_solicitud = r.id_solicitud AND s.estado = 'Realizado')
                    """, conexion);
                comando.Parameters.AddWithValue("@id", idSolicitud);
                comando.Parameters.AddWithValue("@receptor", receptor.Text.Trim());
                int cambios = comando.ExecuteNonQuery();
                CargarResultado();
                aviso.Text = cambios == 1
                    ? "Entrega registrada."
                    : "La entrega o el estado ya cambiaron. Revisá los datos actualizados.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar la entrega. Cerrá y consultá nuevamente antes de reintentar.";
            }
        }
    }
}
