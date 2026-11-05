using System.Data;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public sealed class VistaComprasInsumos : Form
    {
        private readonly TextBox txtBuscar = new()
        {
            Location = new Point(20, 105), Width = 440, MaxLength = 100
        };
        private readonly DataGridView grilla = new()
        {
            Location = new Point(20, 150), Size = new Size(1010, 350),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
        };
        private readonly Label aviso = new()
        {
            Location = new Point(20, 520), Size = new Size(1010, 45),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ForeColor = Color.Firebrick
        };

        public VistaComprasInsumos()
        {
            Text = "Compras de insumos";
            ClientSize = new Size(1050, 580);
            MinimumSize = Size;
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
                Text = "Compras de insumos",
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(100, 15), Size = new Size(850, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };
            var buscar = new Button
            {
                Text = "Buscar / Actualizar", Location = new Point(480, 103), Size = new Size(150, 27)
            };
            buscar.Click += (_, _) => CargarCompras();
            var verTodas = new Button
            {
                Text = "Ver todas", Location = new Point(645, 103), Size = new Size(110, 27)
            };
            verTodas.Click += (_, _) =>
            {
                txtBuscar.Clear();
                CargarCompras();
            };
            Controls.AddRange(new Control[]
            {
                volver, titulo,
                new Label
                {
                    Text = "Buscar por proveedor, comprobante, insumo o lote",
                    Location = new Point(20, 78), Size = new Size(500, 23)
                },
                txtBuscar, buscar, verTodas, grilla, aviso
            });
            AcceptButton = buscar;
            grilla.DataBindingComplete += (_, _) =>
            {
                if (grilla.Columns["Recepcion"] is DataGridViewColumn recepcion)
                    recepcion.DefaultCellStyle.Format = "dd/MM/yyyy";
                if (grilla.Columns["Registro"] is DataGridViewColumn registro)
                    registro.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            };
            Load += (_, _) => CargarCompras();
        }

        private void CargarCompras()
        {
            aviso.Text = "";
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT c.id_compra AS Compra, c.fecha_recepcion AS Recepcion,
                           c.proveedor AS Proveedor, c.comprobante AS Comprobante,
                           i.nombre AS Insumo, i.numero_lote AS Lote,
                           m.cantidad AS Cantidad, c.fecha_registro AS Registro
                    FROM compras_insumos c
                    JOIN movimientos_insumos m ON m.id = c.id_movimiento
                    JOIN insumos i ON i.id = m.id_insumo
                    WHERE (@vacio = TRUE
                           OR c.proveedor LIKE @busqueda ESCAPE '!'
                           OR c.comprobante LIKE @busqueda ESCAPE '!'
                           OR i.nombre LIKE @busqueda ESCAPE '!'
                           OR i.numero_lote LIKE @busqueda ESCAPE '!')
                    ORDER BY c.fecha_recepcion DESC, c.id_compra DESC
                    """;
                string texto = txtBuscar.Text.Trim();
                string patron = texto.Replace("!", "!!").Replace("%", "!%").Replace("_", "!_");
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@vacio", texto.Length == 0);
                comando.Parameters.AddWithValue("@busqueda", $"%{patron}%");
                using var adaptador = new MySqlDataAdapter(comando);
                var compras = new DataTable();
                adaptador.Fill(compras);
                grilla.DataSource = compras;
                aviso.Text = compras.Rows.Count == 0
                    ? "No hay compras para la búsqueda realizada."
                    : $"{compras.Rows.Count} recepciones de lotes encontradas.";
            }
            catch (MySqlException ex) when (ex.Number == 1146)
            {
                grilla.DataSource = null;
                aviso.Text = "Falta una tabla. Revisá los scripts de movimientos y compras (01 y 06).";
            }
            catch (MySqlException)
            {
                grilla.DataSource = null;
                aviso.Text = "No se pudieron consultar las compras. Revisá la conexión y volvé a actualizar.";
            }
        }
    }
}
