using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    // Cambiamos el nombre de la clase para que se llame VistaInsumos
    public partial class VistaInsumos : Form
    {
        private Form ventanaAnterior; // Recibimos el menú principal
        private string cadenaConexion = "server=localhost; database=clinica_seprice; uid=root; pwd=MYSQL;";

        // Modificamos el constructor para aceptar la ventana anterior
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
            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conexion.Open();
                    string query = "SELECT id, nombre, descripcion, stock_actual, stock_minimo, numero_lote, fecha_vencimiento FROM insumos";

                    using (MySqlDataAdapter adaptador = new MySqlDataAdapter(query, conexion))
                    {
                        DataTable dt = new DataTable();
                        adaptador.Fill(dt);
                        dgvInsumos.DataSource = dt;
                        dgvInsumos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvInsumos.AllowUserToAddRows = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los insumos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmNuevoInsumo frm = new FrmNuevoInsumo();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarGrillaInsumos();
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (dgvInsumos.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = $"nombre LIKE '%{txtBuscar.Text.Trim()}%' OR numero_lote LIKE '%{txtBuscar.Text.Trim()}%'";
            }
        }

        // Botón atrás adaptado para volver al menú de tu compañero
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
            dgvInsumos = new DataGridView();
            btnNuevo = new Button();
            txtBuscar = new TextBox();
            btnAtras = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvInsumos).BeginInit();
            SuspendLayout();

            dgvInsumos.AllowUserToAddRows = false;
            dgvInsumos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInsumos.BackgroundColor = Color.White;
            dgvInsumos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInsumos.Location = new Point(12, 60);
            dgvInsumos.Name = "dgvInsumos";
            dgvInsumos.Size = new Size(760, 370);
            dgvInsumos.TabIndex = 0;

            btnNuevo.Location = new Point(12, 20);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(110, 25);
            btnNuevo.TabIndex = 1;
            btnNuevo.Text = "Nuevo insumo";
            btnNuevo.UseVisualStyleBackColor = true;

            txtBuscar.Location = new Point(140, 21);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(200, 23);
            txtBuscar.TabIndex = 2;

            btnAtras.Location = new Point(662, 20);
            btnAtras.Name = "btnAtras";
            btnAtras.Size = new Size(110, 25);
            btnAtras.TabIndex = 3;
            btnAtras.Text = "Atrás";
            btnAtras.UseVisualStyleBackColor = true;

            ClientSize = new Size(784, 461);
            Controls.Add(btnAtras);
            Controls.Add(txtBuscar);
            Controls.Add(btnNuevo);
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
        private TextBox txtBuscar;
        private Button btnAtras;
    }
}