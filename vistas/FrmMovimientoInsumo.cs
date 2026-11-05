using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    internal sealed class FrmMovimientoInsumo : Form
    {
        private readonly int idInsumo;
        private readonly bool esSalida;
        private readonly bool esCompra;
        private readonly TextBox proveedor = new() { MaxLength = 100 };
        private readonly TextBox comprobante = new() { MaxLength = 100 };
        private readonly DateTimePicker fechaRecepcion = new()
        {
            Format = DateTimePickerFormat.Short, Value = DateTime.Today
        };
        private readonly TextBox cantidad = new()
        {
            MaxLength = 10,
            Location = new Point(105, 76), Width = 125
        };
        private readonly TextBox motivo = new()
        {
            MaxLength = 150, Location = new Point(105, 110), Width = 290
        };
        private readonly Label aviso = new()
        {
            ForeColor = Color.Firebrick, Location = new Point(15, 150), Size = new Size(380, 45)
        };

        internal FrmMovimientoInsumo(int idInsumo, string nombre, string lote, bool esSalida,
            bool esCompra = false)
        {
            this.idInsumo = idInsumo;
            this.esSalida = esSalida;
            this.esCompra = esCompra;
            Text = esCompra ? "Registrar compra" : esSalida ? "Registrar salida" : "Registrar entrada";
            ClientSize = new Size(410, 242);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var detalle = new Label
            {
                Text = $"{nombre}\nLote: {lote}",
                Location = new Point(15, 15), Size = new Size(380, 50)
            };
            var guardar = new Button
            {
                Text = "Guardar", Location = new Point(210, 200), Size = new Size(85, 27)
            };
            var cancelar = new Button
            {
                Text = "Cancelar", Location = new Point(310, 200), Size = new Size(85, 27),
                DialogResult = DialogResult.Cancel
            };
            guardar.Click += Guardar_Click;
            AcceptButton = guardar;
            CancelButton = cancelar;
            Controls.AddRange(new Control[]
            {
                detalle,
                new Label { Text = "Cantidad:", Location = new Point(15, 79), AutoSize = true },
                cantidad,
                aviso, guardar, cancelar
            });
            if (esCompra)
            {
                ClientSize = new Size(510, 380);
                detalle.Size = new Size(480, 50);
                proveedor.Location = new Point(150, 110);
                proveedor.Width = 340;
                comprobante.Location = new Point(150, 150);
                comprobante.Width = 340;
                fechaRecepcion.Location = new Point(150, 190);
                fechaRecepcion.Width = 160;
                aviso.Location = new Point(15, 240);
                aviso.Size = new Size(480, 60);
                guardar.Location = new Point(305, 330);
                cancelar.Location = new Point(405, 330);
                cantidad.TabIndex = 0;
                proveedor.TabIndex = 1;
                comprobante.TabIndex = 2;
                fechaRecepcion.TabIndex = 3;
                guardar.TabIndex = 4;
                cancelar.TabIndex = 5;
                Controls.AddRange(new Control[]
                {
                    new Label { Text = "Proveedor:", Location = new Point(15, 113), AutoSize = true },
                    proveedor,
                    new Label { Text = "Comprobante:", Location = new Point(15, 153), AutoSize = true },
                    comprobante,
                    new Label { Text = "Fecha de recepción:", Location = new Point(15, 193), AutoSize = true },
                    fechaRecepcion
                });
            }
            else
            {
                Controls.Add(new Label { Text = "Motivo:", Location = new Point(15, 113), AutoSize = true });
                Controls.Add(motivo);
            }
        }

        private void Guardar_Click(object? sender, EventArgs e)
        {
            // 1. Validar la cantidad ingresada y el motivo del movimiento.
            if (!int.TryParse(cantidad.Text, out int unidades) || unidades <= 0)
            {
                aviso.Text = "Ingresá una cantidad entera mayor que cero.";
                cantidad.Focus();
                return;
            }

            aviso.Text = "";
            if (esCompra && (string.IsNullOrWhiteSpace(proveedor.Text) ||
                             string.IsNullOrWhiteSpace(comprobante.Text)))
            {
                aviso.Text = "Completá el proveedor y el número de comprobante.";
                return;
            }
            if (esCompra && fechaRecepcion.Value.Date > DateTime.Today)
            {
                aviso.Text = "La recepción no puede tener una fecha futura.";
                return;
            }
            string motivoMovimiento = esCompra
                ? $"Compra de insumos — Comprobante {comprobante.Text.Trim()}"
                : motivo.Text.Trim();
            if (motivoMovimiento.Length == 0)
            {
                aviso.Text = "Ingresá el motivo del movimiento.";
                motivo.Focus();
                return;
            }

            try
            {
                // 2. Actualizar el stock vigente y guardar el movimiento en la misma transacción.
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                conexion.Open();
                using var transaccion = conexion.BeginTransaction();

                string actualizar = esSalida ? """
                    UPDATE insumos
                    SET stock_actual = stock_actual - @cantidad
                    WHERE id = @id AND stock_actual >= @limite
                    """ : """
                    UPDATE insumos
                    SET stock_actual = stock_actual + @cantidad
                    WHERE id = @id AND stock_actual <= @limite
                    """;
                using var cambio = new MySqlCommand(actualizar, conexion, transaccion);
                cambio.Parameters.AddWithValue("@id", idInsumo);
                cambio.Parameters.AddWithValue("@cantidad", unidades);
                cambio.Parameters.AddWithValue("@limite", esSalida ? unidades : int.MaxValue - unidades);

                if (cambio.ExecuteNonQuery() != 1)
                {
                    aviso.Text = esSalida
                        ? "No hay stock suficiente o el lote ya no existe."
                        : "El lote ya no existe o la entrada supera el máximo de stock.";
                    return;
                }

                const string insertar = """
                    INSERT INTO movimientos_insumos (id_insumo, tipo, cantidad, motivo)
                    VALUES (@id, @tipo, @cantidad, @motivo)
                    """;
                using var movimiento = new MySqlCommand(insertar, conexion, transaccion);
                movimiento.Parameters.AddWithValue("@id", idInsumo);
                movimiento.Parameters.AddWithValue("@cantidad", unidades);
                movimiento.Parameters.AddWithValue("@tipo", esSalida ? "salida" : "entrada");
                movimiento.Parameters.AddWithValue("@motivo", motivoMovimiento);
                movimiento.ExecuteNonQuery();

                // 3. Vincular la compra a esta misma entrada, sin volver a sumar stock.
                if (esCompra)
                {
                    using var compra = new MySqlCommand("""
                        INSERT INTO compras_insumos
                            (id_movimiento, proveedor, comprobante, fecha_recepcion)
                        VALUES (@movimiento, @proveedor, @comprobante, @fecha)
                        """, conexion, transaccion);
                    compra.Parameters.AddWithValue("@movimiento", movimiento.LastInsertedId);
                    compra.Parameters.AddWithValue("@proveedor", proveedor.Text.Trim());
                    compra.Parameters.AddWithValue("@comprobante", comprobante.Text.Trim());
                    compra.Parameters.AddWithValue("@fecha", fechaRecepcion.Value.Date);
                    compra.ExecuteNonQuery();
                }

                transaccion.Commit();
                DialogResult = DialogResult.OK;
            }
            catch (MySqlException ex) when (esCompra && ex.Number == 1146)
            {
                aviso.Text = "Falta una tabla. Revisá los scripts de movimientos y compras (01 y 06).";
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar el movimiento. Revisá el stock antes de reintentar.";
            }
        }
    }
}
