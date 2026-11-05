using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public partial class FrmNuevoInsumo : Form
    {
        // Conexión directa y estable a MySQL

        // Controles de la interfaz declarados explícitamente (cero conflictos de diseñador)
        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private TextBox txtStockActual;
        private TextBox txtStockMinimo;
        private TextBox txtLote;
        private DateTimePicker dtpFechaVencimiento;
        private Button btnGuardar;
        private Button btnCancelar;
        private readonly ErrorProvider errores  = new ErrorProvider();

        public FrmNuevoInsumo()
        {
            InitializeComponentCustom();
            errores.ContainerControl = this;
            errores.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Validar campos obligatorios: marcar los vacíos y enfocar el primero.

            errores.Clear();

            TextBox[] obligatorios =
            {
                txtNombre,
                txtStockActual,
                txtStockMinimo,
                txtLote
            };

            TextBox? primerIncompleto = null;

            foreach (TextBox campo in obligatorios)
            {
                if (string.IsNullOrWhiteSpace(campo.Text))
                {
                    errores.SetError(campo, "Complete este campo.");
                    primerIncompleto ??= campo;
                }
            }

            if (primerIncompleto != null)
            {
                primerIncompleto.Focus();
                return;
            }

            // 2. Validar que ambos stocks sean enteros iguales o mayores que cero.
            bool stockActualValido = int.TryParse(txtStockActual.Text.Trim(), out int stockActual)
                && stockActual >= 0;
            bool stockMinimoValido = int.TryParse(txtStockMinimo.Text.Trim(), out int stockMinimo)
                && stockMinimo >= 0;

            if (!stockActualValido)
                errores.SetError(txtStockActual, "Ingresá un número entero igual o mayor que cero.");
            if (!stockMinimoValido)
                errores.SetError(txtStockMinimo, "Ingresá un número entero igual o mayor que cero.");

            if (!stockActualValido || !stockMinimoValido)
            {
                (stockActualValido ? txtStockMinimo : txtStockActual).Focus();
                return;
            }


            // 3. Inserción síncrona limpia a MySQL
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion))
                {
                    conexion.Open();
                    string query = "INSERT INTO insumos (nombre, descripcion, stock_actual, stock_minimo, numero_lote, fecha_vencimiento) " +
                                   "VALUES (@nombre, @desc, @stockActual, @stockMin, @lote, @vencimiento);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@desc", txtDescripcion.Text.Trim());
                        cmd.Parameters.AddWithValue("@stockActual", stockActual);
                        cmd.Parameters.AddWithValue("@stockMin", stockMinimo);
                        cmd.Parameters.AddWithValue("@lote", txtLote.Text.Trim());
                        cmd.Parameters.AddWithValue("@vencimiento", dtpFechaVencimiento.Value.Date);

                        int resultado = cmd.ExecuteNonQuery();

                        if (resultado > 0)
                        {
                            MessageBox.Show("¡Insumo registrado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK; // Devuelve la señal para actualizar la grilla
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Construcción limpia de la interfaz por código (sin errores de diseñador)
        private void InitializeComponentCustom()
        {
            this.Text = "Registrar Nuevo Insumo";
            this.Size = new Size(420, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Labels
            Label lbl1 = new Label() { Text = "Nombre:", Left = 30, Top = 25, Width = 110 };
            Label lbl2 = new Label() { Text = "Descripción:", Left = 30, Top = 65, Width = 110 };
            Label lbl3 = new Label() { Text = "Stock Actual:", Left = 30, Top = 130, Width = 110 };
            Label lbl4 = new Label() { Text = "Stock Mínimo:", Left = 30, Top = 170, Width = 110 };
            Label lbl5 = new Label() { Text = "Nº de Lote:", Left = 30, Top = 210, Width = 110 };
            Label lbl6 = new Label() { Text = "Vencimiento:", Left = 30, Top = 250, Width = 110 };

            // Inputs
            txtNombre = new TextBox() { Left = 150, Top = 22, Width = 210 };
            txtDescripcion = new TextBox() { Left = 150, Top = 62, Width = 210, Height = 50, Multiline = true };
            txtStockActual = new TextBox() { Left = 150, Top = 127, Width = 100 };
            txtStockMinimo = new TextBox() { Left = 150, Top = 167, Width = 100 };
            txtLote = new TextBox() { Left = 150, Top = 207, Width = 150 };

            dtpFechaVencimiento = new DateTimePicker() { Left = 150, Top = 247, Width = 150, Format = DateTimePickerFormat.Short };

            // Botones
            btnGuardar = new Button() { Text = "Guardar", Left = 150, Top = 300, Width = 95, Height = 30 };
            btnCancelar = new Button() { Text = "Cancelar", Left = 255, Top = 300, Width = 95, Height = 30 };

            // Eventos
            btnGuardar.Click += btnGuardar_Click;
            btnCancelar.Click += btnCancelar_Click;

            // Agregar controles al formulario
            this.Controls.Add(lbl1); this.Controls.Add(txtNombre);
            this.Controls.Add(lbl2); this.Controls.Add(txtDescripcion);
            this.Controls.Add(lbl3); this.Controls.Add(txtStockActual);
            this.Controls.Add(lbl4); this.Controls.Add(txtStockMinimo);
            this.Controls.Add(lbl5); this.Controls.Add(txtLote);
            this.Controls.Add(lbl6); this.Controls.Add(dtpFechaVencimiento);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }
    }
}
