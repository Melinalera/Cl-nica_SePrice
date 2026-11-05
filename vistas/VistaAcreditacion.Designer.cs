namespace Clínica_SePrice.vistas
{
    partial class VistaAcreditacion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnVolver = new Button();
            lblTitulo = new Label();
            lblDni = new Label();
            txtDni = new TextBox();
            btnBuscar = new Button();
            dgvTurnos = new DataGridView();
            lblAviso = new Label();
            chkIdentidad = new CheckBox();
            lblModalidad = new Label();
            cmbModalidad = new ComboBox();
            lblObraSocial = new Label();
            txtObraSocial = new TextBox();
            lblAfiliado = new Label();
            txtAfiliado = new TextBox();
            btnConfirmar = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvTurnos).BeginInit();
            SuspendLayout();
            // 
            // btnVolver
            // 
            btnVolver.BackColor = Color.Teal;
            btnVolver.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVolver.ForeColor = SystemColors.ControlLight;
            btnVolver.Location = new Point(12, 12);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(75, 23);
            btnVolver.TabIndex = 0;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Palatino Linotype", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(12, 46);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(776, 30);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Acreditación de pacientes";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Click += lblTitulo_Click;
            // 
            // lblDni
            // 
            lblDni.Location = new Point(30, 100);
            lblDni.Name = "lblDni";
            lblDni.Size = new Size(110, 23);
            lblDni.TabIndex = 2;
            lblDni.Text = "DNI del paciente";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(150, 97);
            txtDni.MaxLength = 20;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(180, 23);
            txtDni.TabIndex = 4;
            // 
            // btnBuscar
            // 
            btnBuscar.Location = new Point(350, 96);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(110, 27);
            btnBuscar.TabIndex = 5;
            btnBuscar.Text = "Buscar turno";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // dgvTurnos
            // 
            dgvTurnos.AllowUserToAddRows = false;
            dgvTurnos.AllowUserToDeleteRows = false;
            dgvTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTurnos.Location = new Point(30, 140);
            dgvTurnos.MultiSelect = false;
            dgvTurnos.Name = "dgvTurnos";
            dgvTurnos.ReadOnly = true;
            dgvTurnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTurnos.Size = new Size(740, 130);
            dgvTurnos.TabIndex = 6;
            // 
            // lblAviso
            // 
            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Location = new Point(30, 280);
            lblAviso.Name = "lblAviso";
            lblAviso.Size = new Size(740, 40);
            lblAviso.TabIndex = 7;
            lblAviso.Text = "label1";
            // 
            // chkIdentidad
            // 
            chkIdentidad.Location = new Point(30, 325);
            chkIdentidad.Name = "chkIdentidad";
            chkIdentidad.Size = new Size(500, 25);
            chkIdentidad.TabIndex = 8;
            chkIdentidad.Text = "Verifiqué la identidad con el documento presentado";
            chkIdentidad.UseVisualStyleBackColor = true;
            chkIdentidad.CheckedChanged += chkIdentidad_CheckedChanged;
            // 
            // lblModalidad
            // 
            lblModalidad.Location = new Point(30, 365);
            lblModalidad.Name = "lblModalidad";
            lblModalidad.Size = new Size(130, 23);
            lblModalidad.TabIndex = 9;
            lblModalidad.Text = "Modalidad de pago";
            // 
            // cmbModalidad
            // 
            cmbModalidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbModalidad.FormattingEnabled = true;
            cmbModalidad.Items.AddRange(new object[] { "Particular", "Obra social" });
            cmbModalidad.Location = new Point(170, 362);
            cmbModalidad.Name = "cmbModalidad";
            cmbModalidad.Size = new Size(200, 23);
            cmbModalidad.TabIndex = 10;
            cmbModalidad.SelectedIndexChanged += cmbModalidad_SelectedIndexChanged;
            // 
            // lblObraSocial
            // 
            lblObraSocial.Location = new Point(30, 400);
            lblObraSocial.Name = "lblObraSocial";
            lblObraSocial.Size = new Size(130, 23);
            lblObraSocial.TabIndex = 11;
            lblObraSocial.Text = "Obra social";
            // 
            // txtObraSocial
            // 
            txtObraSocial.Enabled = false;
            txtObraSocial.Location = new Point(170, 397);
            txtObraSocial.MaxLength = 100;
            txtObraSocial.Name = "txtObraSocial";
            txtObraSocial.Size = new Size(200, 23);
            txtObraSocial.TabIndex = 12;
            // 
            // lblAfiliado
            // 
            lblAfiliado.Location = new Point(390, 400);
            lblAfiliado.Name = "lblAfiliado";
            lblAfiliado.Size = new Size(140, 23);
            lblAfiliado.TabIndex = 13;
            lblAfiliado.Text = "Número de afiliado";
            // 
            // txtAfiliado
            // 
            txtAfiliado.Enabled = false;
            txtAfiliado.Location = new Point(540, 397);
            txtAfiliado.MaxLength = 50;
            txtAfiliado.Name = "txtAfiliado";
            txtAfiliado.Size = new Size(200, 23);
            txtAfiliado.TabIndex = 14;
            // 
            // btnConfirmar
            // 
            btnConfirmar.Location = new Point(540, 455);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(200, 30);
            btnConfirmar.TabIndex = 15;
            btnConfirmar.Text = "Confirmar acreditación";
            btnConfirmar.UseVisualStyleBackColor = true;
            btnConfirmar.Click += btnConfirmar_Click;
            // 
            // VistaAcreditacion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 511);
            Controls.Add(btnConfirmar);
            Controls.Add(txtAfiliado);
            Controls.Add(lblAfiliado);
            Controls.Add(txtObraSocial);
            Controls.Add(lblObraSocial);
            Controls.Add(cmbModalidad);
            Controls.Add(lblModalidad);
            Controls.Add(chkIdentidad);
            Controls.Add(lblAviso);
            Controls.Add(dgvTurnos);
            Controls.Add(btnBuscar);
            Controls.Add(txtDni);
            Controls.Add(lblDni);
            Controls.Add(lblTitulo);
            Controls.Add(btnVolver);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "VistaAcreditacion";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Acreditación de pacientes";
            ((System.ComponentModel.ISupportInitialize)dgvTurnos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnVolver;
        private Label lblTitulo;
        private Label lblDni;
        private TextBox txtDni;
        private Button btnBuscar;
        private DataGridView dgvTurnos;
        private Label lblAviso;
        private CheckBox chkIdentidad;
        private Label lblModalidad;
        private ComboBox cmbModalidad;
        private Label lblObraSocial;
        private TextBox txtObraSocial;
        private Label lblAfiliado;
        private TextBox txtAfiliado;
        private Button btnConfirmar;
    }
}