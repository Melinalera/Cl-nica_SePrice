using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaLiquidacionHonorarios : Form
    {
        private readonly ComboBox cmbMedico = new()
        {
            Location = new Point(90, 90), Width = 470,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly DateTimePicker desde = new()
        {
            Location = new Point(90, 130), Width = 140, Format = DateTimePickerFormat.Short,
            Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };
        private readonly DateTimePicker hasta = new()
        {
            Location = new Point(320, 130), Width = 140, Format = DateTimePickerFormat.Short,
            Value = DateTime.Today
        };
        private readonly DataGridView grilla = new()
        {
            Location = new Point(20, 180), Size = new Size(910, 270),
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        private readonly Label resumen = new()
        {
            Location = new Point(20, 465), Size = new Size(910, 30),
            Font = new Font("Tahoma", 10F, FontStyle.Bold)
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 510), Size = new Size(910, 45),
            ForeColor = Color.Firebrick
        };
        private DataTable? vistaPrevia;

        public VistaLiquidacionHonorarios()
        {
            Text = "Liquidación de honorarios";
            ClientSize = new Size(950, 620);
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
                Text = "Liquidación de honorarios",
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(100, 15), Size = new Size(730, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var calcular = new Button
            {
                Text = "Consultar atenciones", Location = new Point(590, 128), Size = new Size(180, 27)
            };
            calcular.Click += (_, _) => Consultar();
            var confirmar = new Button
            {
                Text = "Confirmar liquidación", Location = new Point(710, 570), Size = new Size(220, 30)
            };
            confirmar.Click += (_, _) => Confirmar();
            var historial = new Button
            {
                Text = "Ver liquidaciones", Location = new Point(20, 570), Size = new Size(180, 30)
            };
            historial.Click += (_, _) =>
            {
                int? medico = cmbMedico.SelectedValue is int id ? id : null;
                using var ventana = new VistaHistorialLiquidaciones(medico);
                ventana.ShowDialog(this);
            };
            Controls.AddRange(new Control[]
            {
                volver, titulo,
                new Label { Text = "Médico", Location = new Point(20, 93), AutoSize = true }, cmbMedico,
                new Label { Text = "Desde", Location = new Point(20, 133), AutoSize = true }, desde,
                new Label { Text = "Hasta", Location = new Point(260, 133), AutoSize = true }, hasta,
                calcular, grilla, resumen, aviso, confirmar, historial
            });
            cmbMedico.SelectedIndexChanged += (_, _) => LimpiarConsulta();
            desde.ValueChanged += (_, _) => LimpiarConsulta();
            hasta.ValueChanged += (_, _) => LimpiarConsulta();
            Load += (_, _) => CargarMedicos();
        }

        private void LimpiarConsulta()
        {
            grilla.DataSource = null;
            vistaPrevia?.Dispose();
            vistaPrevia = null;
            resumen.Text = "";
            aviso.Text = "";
        }

        private void CargarMedicos()
        {
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT m.id_usuario,
                           CONCAT(u.apellido, ', ', u.nombre, ' — ', COALESCE(m.especialidad, 'Sin especialidad')) AS medico
                    FROM medicos m JOIN usuarios u ON u.id_usuario = m.id_usuario
                    WHERE u.rol = 'medico'
                    ORDER BY u.apellido, u.nombre, m.id_usuario
                    """;
                using var adaptador = new MySqlDataAdapter(consulta, conexion);
                var medicos = new DataTable();
                adaptador.Fill(medicos);
                cmbMedico.DisplayMember = "medico";
                cmbMedico.ValueMember = "id_usuario";
                cmbMedico.DataSource = medicos;
                cmbMedico.SelectedIndex = -1;
                if (medicos.Rows.Count == 0)
                    aviso.Text = "No hay médicos registrados para liquidar honorarios.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudieron cargar los médicos. Cerrá y volvé a abrir para reintentar.";
            }
        }

        private DataTable ConsultarAtenciones(MySqlConnection conexion, MySqlTransaction? transaccion = null)
        {
            const string consulta = """
                SELECT a.id_atencion, a.id_turno, t.dni_paciente,
                       a.fecha_atencion, a.practica, a.importe
                FROM atenciones_consultorios a
                JOIN `turnosConsultoriosMédicos` t ON t.id_turno = a.id_turno
                LEFT JOIN detalle_liquidaciones_honorarios d ON d.id_atencion = a.id_atencion
                WHERE a.id_medico = @medico
                  AND a.fecha_atencion >= @desde AND a.fecha_atencion < @fin
                  AND d.id_atencion IS NULL
                ORDER BY a.id_atencion
                """;
            using var comando = new MySqlCommand(
                consulta + (transaccion == null ? "" : " FOR UPDATE"), conexion, transaccion);
            comando.Parameters.AddWithValue("@medico", cmbMedico.SelectedValue);
            comando.Parameters.AddWithValue("@desde", desde.Value.Date);
            comando.Parameters.AddWithValue("@fin", hasta.Value.Date.AddDays(1));
            using var adaptador = new MySqlDataAdapter(comando);
            var atenciones = new DataTable();
            adaptador.Fill(atenciones);
            return atenciones;
        }

        private void Consultar()
        {
            LimpiarConsulta();
            if (cmbMedico.SelectedIndex < 0 || desde.Value.Date > hasta.Value.Date)
            {
                aviso.Text = "Seleccioná un médico y un período válido.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                vistaPrevia = ConsultarAtenciones(conexion);
                grilla.DataSource = vistaPrevia;
                decimal total = vistaPrevia.AsEnumerable().Sum(fila => fila.Field<decimal>("importe"));
                resumen.Text = $"{vistaPrevia.Rows.Count} atenciones sin liquidar — Total: {total.ToString("C2", CultureInfo.GetCultureInfo("es-AR"))}";
                if (vistaPrevia.Rows.Count == 0)
                    aviso.Text = "No hay atenciones pendientes de liquidar para ese profesional y período.";
            }
            catch (MySqlException)
            {
                LimpiarConsulta();
                aviso.Text = "No se pudieron consultar las atenciones. Revisá la conexión y el script 05 de honorarios.";
            }
        }

        private void Confirmar()
        {
            aviso.Text = "";
            if (vistaPrevia == null || vistaPrevia.Rows.Count == 0)
            {
                aviso.Text = "Consultá las atenciones pendientes antes de confirmar.";
                return;
            }
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();

                // 1. Serializar las liquidaciones del mismo profesional.
                using var bloquear = new MySqlCommand(
                    "SELECT id_usuario FROM medicos WHERE id_usuario = @medico FOR UPDATE",
                    conexion, transaccion);
                bloquear.Parameters.AddWithValue("@medico", cmbMedico.SelectedValue);
                if (bloquear.ExecuteScalar() == null)
                {
                    aviso.Text = "El médico ya no está disponible. Cerrá y actualizá los datos.";
                    return;
                }

                // 2. Verificar que las atenciones y sus importes sigan siendo los revisados.
                using var actuales = ConsultarAtenciones(conexion, transaccion);
                var revisadas = vistaPrevia.AsEnumerable()
                    .Select(fila => (fila.Field<int>("id_atencion"), fila.Field<decimal>("importe")));
                var disponibles = actuales.AsEnumerable()
                    .Select(fila => (fila.Field<int>("id_atencion"), fila.Field<decimal>("importe")));
                if (!revisadas.SequenceEqual(disponibles))
                {
                    aviso.Text = "Las atenciones cambiaron desde la consulta. Volvé a consultar antes de confirmar.";
                    return;
                }
                decimal total = actuales.AsEnumerable().Sum(fila => fila.Field<decimal>("importe"));
                if (total > 999999999999.99m)
                {
                    aviso.Text = "El total supera el máximo admitido. Seleccioná un período más corto.";
                    return;
                }

                // 3. Guardar la cabecera y el detalle en una sola operación.
                using var cabecera = new MySqlCommand("""
                    INSERT INTO liquidaciones_honorarios
                        (id_medico, fecha_desde, fecha_hasta, importe_total)
                    VALUES (@medico, @desde, @hasta, @total)
                    """, conexion, transaccion);
                cabecera.Parameters.AddWithValue("@medico", cmbMedico.SelectedValue);
                cabecera.Parameters.AddWithValue("@desde", desde.Value.Date);
                cabecera.Parameters.AddWithValue("@hasta", hasta.Value.Date);
                cabecera.Parameters.AddWithValue("@total", total);
                cabecera.ExecuteNonQuery();
                long idLiquidacion = cabecera.LastInsertedId;

                foreach (DataRow atencion in actuales.Rows)
                {
                    using var detalle = new MySqlCommand("""
                        INSERT INTO detalle_liquidaciones_honorarios (id_liquidacion, id_atencion, importe)
                        VALUES (@liquidacion, @atencion, @importe)
                        """, conexion, transaccion);
                    detalle.Parameters.AddWithValue("@liquidacion", idLiquidacion);
                    detalle.Parameters.AddWithValue("@atencion", atencion["id_atencion"]);
                    detalle.Parameters.AddWithValue("@importe", atencion["importe"]);
                    detalle.ExecuteNonQuery();
                }
                transaccion.Commit();
                LimpiarConsulta();
                aviso.Text = $"Liquidación N.º {idLiquidacion} registrada. Total: {total.ToString("C2", CultureInfo.GetCultureInfo("es-AR"))}.";
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                aviso.Text = "Una atención ya fue liquidada. Volvé a consultar el período.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar la liquidación. Volvé a consultar antes de reintentar.";
            }
        }
    }
}
