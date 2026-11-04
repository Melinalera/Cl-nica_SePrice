using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    internal sealed class FrmMovimientoInsumo : Form
    {
        private readonly int idInsumo;
        private readonly bool esSalida;
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

        internal FrmMovimientoInsumo(int idInsumo, string nombre, string lote, bool esSalida)
        {
            this.idInsumo = idInsumo;
            this.esSalida = esSalida;
            Text = esSalida ? "Registrar salida" : "Registrar entrada";
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
                new Label { Text = "Motivo:", Location = new Point(15, 113), AutoSize = true },
                motivo, aviso, guardar, cancelar
            });
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

            string motivoMovimiento = motivo.Text.Trim();
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

                transaccion.Commit();
                DialogResult = DialogResult.OK;
            }
            catch (MySqlException)
            {
                aviso.Text = "No se pudo confirmar el movimiento. Revisá el stock antes de reintentar.";
            }
        }
    }
}
