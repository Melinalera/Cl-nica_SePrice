using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{

    public partial class VistaInsumos : Form
    {
        private Form ventanaAnterior;


        public VistaInsumos(Form anterior)
        {
            InitializeComponent();
            ventanaAnterior = anterior;

            this.Load += FrmControlInsumos_Load;
            this.btnNuevo.Click += btnNuevo_Click;
            this.btnAtras.Click += btnAtras_Click;
            this.txtBuscar.TextChanged += txtBuscar_TextChanged;
            this.dgvInsumos.CellFormatting += dgvInsumos_CellFormatting;
        }

        private void FrmControlInsumos_Load(object sender, EventArgs e)
        {
            CargarGrillaInsumos();
        }

        private void CargarGrillaInsumos()
        {
            lblAviso.Text = "";
            using (MySqlConnection conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion))
            {
                try
                {
                    conexion.Open();
                    string query = "SELECT id, nombre, descripcion, stock_actual, stock_minimo, numero_lote, fecha_vencimiento FROM insumos";

                    using (MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion))
                    {
                        DataTable dt = new DataTable();
                        adaptador.Fill(dt);
                        dt.DefaultView.RowFilter = CrearFiltroBusqueda(txtBuscar.Text);
                        dgvInsumos.DataSource = dt;
                        dgvInsumos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvInsumos.AllowUserToAddRows = false;
                    }
                }
                catch (Exception ex)
                {
                    lblAviso.Text = "No se pudo actualizar la grilla: " + ex.Message;
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmNuevoInsumo frm = new FrmNuevoInsumo();
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                CargarGrillaInsumos();
            }
        }

        private void RegistrarMovimiento(bool esSalida, bool esCompra = false)
        {
            if (dgvInsumos.CurrentRow?.DataBoundItem is not DataRowView lote)
            {
                lblAviso.Text = "Seleccioná un lote para registrar el movimiento.";
                return;
            }

            using var formulario = new FrmMovimientoInsumo(
                (int)lote["id"], (string)lote["nombre"], (string)lote["numero_lote"], esSalida, esCompra);

            if (formulario.ShowDialog(this) == DialogResult.OK)
                CargarGrillaInsumos();
        }

        private void btnHistorial_Click(object? sender, EventArgs e)
        {
            lblAviso.Text = "";

            // 1. Identificar el lote seleccionado en la grilla.
            if (dgvInsumos.CurrentRow?.DataBoundItem is not DataRowView lote)
            {
                lblAviso.Text = "Seleccioná un lote para ver su historial.";
                return;
            }

            // 2. Consultar los movimientos del lote, del más reciente al más antiguo.
            using var movimientos = new DataTable();
            try
            {
                using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
                const string consulta = """
                    SELECT fecha AS Fecha, tipo AS Tipo,
                           cantidad AS Cantidad, motivo AS Motivo
                    FROM movimientos_insumos
                    WHERE id_insumo = @id
                    ORDER BY fecha DESC, id DESC
                    """;
                using var comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@id", lote["id"]);
                using var adaptador = new MySqlDataAdapter(comando);
                adaptador.Fill(movimientos);
            }
            catch (MySqlException)
            {
                lblAviso.Text = "No se pudo cargar el historial del lote.";
                return;
            }

            if (movimientos.Rows.Count == 0)
            {
                lblAviso.Text = "El lote seleccionado todavía no tiene movimientos registrados.";
                return;
            }

            // 3. Mostrar los movimientos en una grilla de solo lectura.
            using var ventana = new Form
            {
                Text = $"Historial de {lote["nombre"]} — Lote {lote["numero_lote"]}",
                ClientSize = new Size(660, 340),
                StartPosition = FormStartPosition.CenterParent
            };

            var grilla = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grilla.DataBindingComplete += (_, _) =>
            {
                if (grilla.Columns["Fecha"] is DataGridViewColumn fecha)
                    fecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                if (grilla.Columns["Motivo"] is DataGridViewColumn motivo)
                    motivo.FillWeight = 190;
            };

            ventana.Controls.Add(grilla);
            grilla.DataSource = movimientos;
            ventana.ShowDialog(this);
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (dgvInsumos.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = CrearFiltroBusqueda(txtBuscar.Text);
            }
        }

        private static string CrearFiltroBusqueda(string texto)
        {
            // Tratar las comillas y los comodines como parte literal de la búsqueda.
            var busqueda = new StringBuilder();
            foreach (char caracter in texto.Trim())
            {
                busqueda.Append(caracter switch
                {
                    '\'' => "''",
                    '[' => "[[]",
                    ']' => "[]]",
                    '*' => "[*]",
                    '%' => "[%]",
                    _ => caracter.ToString()
                });
            }
            return $"nombre LIKE '%{busqueda}%' OR numero_lote LIKE '%{busqueda}%'";
        }

        // Botón atrás adaptado para volver al menú
        private void btnAtras_Click(object sender, EventArgs e)
        {
            if (ventanaAnterior != null)
            {
                ventanaAnterior.Show();
            }
            this.Close();
        }

        private void dgvInsumos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvInsumos.Rows.Count || dgvInsumos.Rows[e.RowIndex].IsNewRow)
                return;

            DataGridViewRow fila = dgvInsumos.Rows[e.RowIndex];
            fila.DefaultCellStyle.BackColor = Color.White;

            if (fila.Cells["stock_actual"].Value != DBNull.Value && fila.Cells["stock_minimo"].Value != DBNull.Value)
            {
                int stockActual = Convert.ToInt32(fila.Cells["stock_actual"].Value);
                int stockMinimo = Convert.ToInt32(fila.Cells["stock_minimo"].Value);

                if (stockActual <= 0)
                {
                    fila.DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else if (stockActual <= stockMinimo)
                {
                    fila.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }

            if (fila.Cells["fecha_vencimiento"].Value != DBNull.Value)
            {
                DateTime fechaVenc = Convert.ToDateTime(fila.Cells["fecha_vencimiento"].Value).Date;
                DateTime hoy = DateTime.Now.Date;

                if (fechaVenc < hoy)
                {
                    fila.DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else if (fechaVenc <= hoy.AddDays(30) && fila.DefaultCellStyle.BackColor != Color.LightCoral)
                {
                    fila.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
        }

        private void InitializeComponent()
        {
            Label lblTitulo = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Palatino Linotype", 14F, FontStyle.Bold),
                Location = new Point(12, 46),
                Size = new Size(760, 26),
                Text = "Gestión de Insumos",
                TextAlign = ContentAlignment.MiddleCenter
            };
            dgvInsumos = new DataGridView();
            btnNuevo = new Button();
            btnEntrada = new Button();
            txtBuscar = new TextBox();
            btnAtras = new Button();
            lblAviso = new Label
            {
                ForeColor = Color.Firebrick,
                Location = new Point(12, 432),
                Size = new Size(760, 28)
            };
            ((System.ComponentModel.ISupportInitialize)dgvInsumos).BeginInit();
            SuspendLayout();

            dgvInsumos.AllowUserToAddRows = false;
            dgvInsumos.ReadOnly = true;
            dgvInsumos.MultiSelect = false;
            dgvInsumos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInsumos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInsumos.BackgroundColor = Color.White;
            dgvInsumos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInsumos.Location = new Point(12, 112);
            dgvInsumos.Name = "dgvInsumos";
            dgvInsumos.Size = new Size(760, 318);
            dgvInsumos.TabIndex = 0;

            btnNuevo.Location = new Point(12, 80);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(110, 25);
            btnNuevo.TabIndex = 1;
            btnNuevo.Text = "Nuevo insumo";
            btnNuevo.UseVisualStyleBackColor = true;

            btnEntrada.Location = new Point(132, 80);
            btnEntrada.Size = new Size(130, 25);
            btnEntrada.Text = "Registrar entrada";
            btnEntrada.UseVisualStyleBackColor = true;
            btnEntrada.Click += (_, _) => RegistrarMovimiento(esSalida: false);

            var btnSalida = new Button
            {
                Location = new Point(272, 80),
                Size = new Size(130, 25),
                Text = "Registrar salida",
                UseVisualStyleBackColor = true
            };
            btnSalida.Click += (_, _) => RegistrarMovimiento(esSalida: true);

            txtBuscar.Location = new Point(412, 81);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(200, 23);
            txtBuscar.TabIndex = 2;

            var btnHistorial = new Button
            {
                Location = new Point(622, 80),
                Size = new Size(130, 25),
                Text = "Ver historial",
                UseVisualStyleBackColor = true
            };
            btnHistorial.Click += btnHistorial_Click;

            var btnCompra = new Button
            {
                Location = new Point(622, 12),
                Size = new Size(130, 27),
                Text = "Registrar compra",
                UseVisualStyleBackColor = true
            };
            btnCompra.Click += (_, _) => RegistrarMovimiento(esSalida: false, esCompra: true);
            var btnVerCompras = new Button
            {
                Location = new Point(480, 12),
                Size = new Size(130, 27),
                Text = "Ver compras",
                UseVisualStyleBackColor = true
            };
            btnVerCompras.Click += (_, _) =>
            {
                using var ventana = new VistaComprasInsumos();
                ventana.ShowDialog(this);
            };

            btnAtras.BackColor = Color.Teal;
            btnAtras.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAtras.ForeColor = SystemColors.ControlLight;
            btnAtras.Location = new Point(12, 12);
            btnAtras.Name = "btnAtras";
            btnAtras.Size = new Size(75, 23);
            btnAtras.TabIndex = 3;
            btnAtras.Text = "Volver";
            btnAtras.UseVisualStyleBackColor = false;

            ClientSize = new Size(784, 461);
            Controls.Add(lblTitulo);
            Controls.Add(lblAviso);
            Controls.Add(btnAtras);
            Controls.Add(txtBuscar);
            Controls.Add(btnNuevo);
            Controls.Add(btnEntrada);
            Controls.Add(btnSalida);
            Controls.Add(btnHistorial);
            Controls.Add(btnCompra);
            Controls.Add(btnVerCompras);
            Controls.Add(dgvInsumos);
            Name = "VistaInsumos";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Control y Gestión de Insumos y Stock";
            ((System.ComponentModel.ISupportInitialize)dgvInsumos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridView dgvInsumos;
        private Button btnNuevo;
        private Button btnEntrada;
        private Label lblAviso;
        private TextBox txtBuscar;
        private Button btnAtras;
    }
}
