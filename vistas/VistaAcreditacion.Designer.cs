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
            // VistaAcreditacion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 411);
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
    }
}