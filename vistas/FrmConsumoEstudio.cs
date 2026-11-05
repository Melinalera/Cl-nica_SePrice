using System.Data;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    internal sealed class FrmConsumoEstudio : Form
    {
        private readonly int idSolicitud;
        private readonly DataGridView grilla = new()
        {
            Location = new Point(20, 115), Size = new Size(860, 290),
            AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        private readonly CheckBox sinConsumo = new()
        {
            Text = "Confirmo que este estudio no utilizó insumos.",
            Location = new Point(20, 420), Size = new Size(500, 26)
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 457), Size = new Size(860, 45),
            ForeColor = Color.Firebrick
        };
        private readonly Button confirmar = new()
        {
            Text = "Confirmar y finalizar estudio",
            Location = new Point(650, 510), Size = new Size(230, 30),
            Enabled = false
        };
        private bool consultaCompleta;

        internal FrmConsumoEstudio(int idSolicitud, string estudio)
        {
            this.idSolicitud = idSolicitud;
            Text = $"Finalizar solicitud N.º {idSolicitud}";
            ClientSize = new Size(900, 560);
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
                Text = "Consumo de insumos",
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(100, 15), Size = new Size(680, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var detalle = new Label
            {
                Text = $"Solicitud N.º {idSolicitud} — {estudio}\nIngresá las unidades usadas por lote. Se muestran lotes con stock y sin vencer.",
                Location = new Point(20, 60), Size = new Size(860, 48)
            };
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "nombre", HeaderText = "Insumo", ReadOnly = true, FillWeight = 160
            });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "numero_lote", HeaderText = "Lote", ReadOnly = true
            });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "fecha_vencimiento", HeaderText = "Vencimiento", ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "stock_actual", HeaderText = "Disponible", ReadOnly = true
            });
            grilla.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "cantidad", HeaderText = "Cantidad usada",
                DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.LightYellow }
            });
            grilla.DataError += (_, e) =>
            {
                e.ThrowException = false;
                aviso.Text = "Revisá la cantidad ingresada.";
            };
            sinConsumo.CheckedChanged += (_, _) => grilla.Enabled = !sinConsumo.Checked;
            confirmar.Click += Confirmar_Click;
            Controls.AddRange(new Control[] { volver, titulo, detalle, grilla, sinConsumo, aviso, confirmar });
            Load += (_, _) => CargarLotes();
        }

        private void CargarLotes()
        {
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT id, nombre, numero_lote, fecha_vencimiento, stock_actual
                    FROM insumos
                    WHERE stock_actual > 0 AND fecha_vencimiento >= CURDATE()
                    ORDER BY fecha_vencimiento, id
                    """;
                using var adaptador = new MySqlDataAdapter(consulta, conexion);
                var lotes = new DataTable();
                adaptador.Fill(lotes);
                lotes.Columns.Add("cantidad", typeof(string)).DefaultValue = "0";
                foreach (DataRow lote in lotes.Rows)
                    lote["cantidad"] = "0";
                grilla.DataSource = lotes;
                consultaCompleta = true;
                confirmar.Enabled = true;
                if (lotes.Rows.Count == 0)
                    aviso.Text = "No hay lotes disponibles. Si el estudio utilizó insumos, registrá su disponibilidad antes de finalizar.";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudieron consultar los lotes. Cerrá y volvé a abrir para reintentar.";
            }
        }

        private void Confirmar_Click(object? sender, EventArgs e)
        {
            aviso.Text = "";
            if (!consultaCompleta || !grilla.EndEdit())
                return;
            BindingContext?[grilla.DataSource!]?.EndCurrentEdit();

            // 1. Validar cantidades enteras y seleccionar los lotes usados.
            var consumos = new List<(int Id, int Cantidad)>();
            foreach (DataGridViewRow fila in grilla.Rows)
            {
                if (fila.DataBoundItem is not DataRowView lote)
                    continue;
                string texto = lote["cantidad"].ToString()?.Trim() ?? "";
                int cantidad = 0;
                if (texto.Length > 0 && (!int.TryParse(texto, out cantidad) || cantidad < 0))
                {
                    aviso.Text = "Las cantidades deben ser números enteros iguales o mayores que cero.";
                    return;
                }
                if (cantidad > 0)
                    consumos.Add(((int)lote["id"], cantidad));
            }
            if (sinConsumo.Checked && consumos.Count > 0)
            {
                aviso.Text = "Para confirmar sin insumos, quitá la marca y dejá las cantidades en cero.";
                return;
            }
            if (!sinConsumo.Checked && consumos.Count == 0)
            {
                aviso.Text = "Ingresá el consumo o confirmá que el estudio no utilizó insumos.";
                return;
            }

            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();

                // 2. Bloquear la finalización si la solicitud ya cambió de estado.
                using var finalizar = new MySqlCommand("""
                    UPDATE solicitudes_estudios_guardia
                    SET estado = 'Realizado'
                    WHERE id_solicitud = @id AND estado = 'En realización'
                    """, conexion, transaccion);
                finalizar.Parameters.AddWithValue("@id", idSolicitud);
                if (finalizar.ExecuteNonQuery() != 1)
                {
                    aviso.Text = "La solicitud ya cambió de estado. Cerrá y actualizá el listado.";
                    return;
                }

                // 3. Descontar el stock vigente y registrar cada consumo y su salida.
                foreach (var consumo in consumos.OrderBy(item => item.Id))
                {
                    using var descontar = new MySqlCommand("""
                        UPDATE insumos SET stock_actual = stock_actual - @cantidad
                        WHERE id = @id AND stock_actual >= @cantidad
                          AND fecha_vencimiento >= CURDATE()
                        """, conexion, transaccion);
                    descontar.Parameters.AddWithValue("@cantidad", consumo.Cantidad);
                    descontar.Parameters.AddWithValue("@id", consumo.Id);
                    if (descontar.ExecuteNonQuery() != 1)
                    {
                        aviso.Text = $"El lote con ID {consumo.Id} no tiene stock suficiente o está vencido. No se guardó el consumo.";
                        return;
                    }

                    using var registrar = new MySqlCommand("""
                        INSERT INTO consumos_estudios_guardia (id_solicitud, id_insumo, cantidad)
                        VALUES (@solicitud, @insumo, @cantidad)
                        """, conexion, transaccion);
                    registrar.Parameters.AddWithValue("@solicitud", idSolicitud);
                    registrar.Parameters.AddWithValue("@insumo", consumo.Id);
                    registrar.Parameters.AddWithValue("@cantidad", consumo.Cantidad);
                    registrar.ExecuteNonQuery();

                    using var movimiento = new MySqlCommand("""
                        INSERT INTO movimientos_insumos (id_insumo, tipo, cantidad, motivo)
                        VALUES (@insumo, 'salida', @cantidad, @motivo)
                        """, conexion, transaccion);
                    movimiento.Parameters.AddWithValue("@insumo", consumo.Id);
                    movimiento.Parameters.AddWithValue("@cantidad", consumo.Cantidad);
                    movimiento.Parameters.AddWithValue("@motivo", $"Consumo de estudio de guardia/internación — Solicitud N.º {idSolicitud}");
                    movimiento.ExecuteNonQuery();
                }

                // 4. Confirmar todos los cambios juntos.
                transaccion.Commit();
                DialogResult = DialogResult.OK;
            }
            catch (MySqlException ex) when (ex.Number == 1146)
            {
                aviso.Text = "Falta una tabla. Revisá los scripts de movimientos, solicitudes y consumos (01, 03 y 04).";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar la finalización. Cerrá y actualizá el listado antes de reintentar.";
            }
        }
    }
}
