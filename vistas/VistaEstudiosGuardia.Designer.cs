namespace Clínica_SePrice.vistas
{
    partial class VistaEstudiosGuardia
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            btnVolver = new Button();
            lblTitulo = new Label();
            lblDni = new Label();
            txtDni = new TextBox();
            lblPaciente = new Label();
            txtPaciente = new TextBox();
            lblEstudio = new Label();
            txtEstudio = new TextBox();
            lblOrigen = new Label();
            cmbOrigen = new ComboBox();
            lblPrioridad = new Label();
            cmbPrioridad = new ComboBox();
            lblOrden = new Label();
            txtOrden = new TextBox();
            chkOrdenValida = new CheckBox();
            lblAviso = new Label();
            btnRegistrar = new Button();
            SuspendLayout();

            btnVolver.BackColor = Color.Teal;
            btnVolver.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            btnVolver.ForeColor = SystemColors.ControlLight;
            btnVolver.Location = new Point(12, 12);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(75, 23);
            btnVolver.TabIndex = 0;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;

            lblTitulo.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            lblTitulo.Location = new Point(12, 46);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(776, 30);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Estudios de guardia e internación";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            lblDni.Location = new Point(30, 100);
            lblDni.Name = "lblDni";
            lblDni.Size = new Size(220, 23);
            lblDni.Text = "DNI del paciente";

            txtDni.Location = new Point(30, 126);
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(220, 23);
            txtDni.TabIndex = 2;
            txtDni.MaxLength = 20;
            lblPaciente.Location = new Point(290, 100);
            lblPaciente.Name = "lblPaciente";
            lblPaciente.Size = new Size(470, 23);
            lblPaciente.Text = "Nombre y apellido";

            txtPaciente.Location = new Point(290, 126);
            txtPaciente.Name = "txtPaciente";
            txtPaciente.Size = new Size(470, 23);
            txtPaciente.TabIndex = 3;
            txtPaciente.MaxLength = 100;
            lblEstudio.Location = new Point(30, 180);
            lblEstudio.Name = "lblEstudio";
            lblEstudio.Size = new Size(730, 23);
            lblEstudio.Text = "Estudio solicitado";

            txtEstudio.Location = new Point(30, 206);
            txtEstudio.Name = "txtEstudio";
            txtEstudio.Size = new Size(730, 23);
            txtEstudio.TabIndex = 4;
            txtEstudio.MaxLength = 100;
            lblOrigen.Location = new Point(30, 260);
            lblOrigen.Name = "lblOrigen";
            lblOrigen.Size = new Size(220, 23);
            lblOrigen.Text = "Origen de la solicitud";

            cmbOrigen.Location = new Point(30, 286);
            cmbOrigen.Name = "cmbOrigen";
            cmbOrigen.Size = new Size(220, 23);
            cmbOrigen.TabIndex = 5;
            cmbOrigen.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrigen.Items.AddRange(new object[] { "Guardia", "Internación" });
            lblPrioridad.Location = new Point(290, 260);
            lblPrioridad.Name = "lblPrioridad";
            lblPrioridad.Size = new Size(180, 23);
            lblPrioridad.Text = "Prioridad";

            cmbPrioridad.Location = new Point(290, 286);
            cmbPrioridad.Name = "cmbPrioridad";
            cmbPrioridad.Size = new Size(180, 23);
            cmbPrioridad.TabIndex = 6;
            cmbPrioridad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPrioridad.Items.AddRange(new object[] { "Normal", "Urgente" });
            lblOrden.Location = new Point(30, 340);
            lblOrden.Name = "lblOrden";
            lblOrden.Size = new Size(440, 23);
            lblOrden.Text = "Número o referencia de orden médica";

            txtOrden.Location = new Point(30, 366);
            txtOrden.Name = "txtOrden";
            txtOrden.Size = new Size(440, 23);
            txtOrden.TabIndex = 7;
            txtOrden.MaxLength = 100;
            chkOrdenValida.Location = new Point(30, 412);
            chkOrdenValida.Name = "chkOrdenValida";
            chkOrdenValida.Size = new Size(730, 27);
            chkOrdenValida.TabIndex = 8;
            chkOrdenValida.Text = "Verifiqué que la orden médica corresponde al paciente y al estudio solicitado.";
            chkOrdenValida.UseVisualStyleBackColor = true;

            lblAviso.ForeColor = Color.Firebrick;
            lblAviso.Location = new Point(30, 450);
            lblAviso.Name = "lblAviso";
            lblAviso.Size = new Size(730, 40);

            btnRegistrar.Location = new Point(560, 500);
            btnRegistrar.Name = "btnRegistrar";
            btnRegistrar.Size = new Size(200, 30);
            btnRegistrar.TabIndex = 9;
            btnRegistrar.Text = "Registrar solicitud";
            btnRegistrar.UseVisualStyleBackColor = true;
            btnRegistrar.Click += btnRegistrar_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 550);
            Controls.Add(lblDni);
            Controls.Add(txtDni);
            Controls.Add(lblPaciente);
            Controls.Add(txtPaciente);
            Controls.Add(lblEstudio);
            Controls.Add(txtEstudio);
            Controls.Add(lblOrigen);
            Controls.Add(cmbOrigen);
            Controls.Add(lblPrioridad);
            Controls.Add(cmbPrioridad);
            Controls.Add(lblOrden);
            Controls.Add(txtOrden);
            Controls.Add(chkOrdenValida);
            Controls.Add(lblAviso);
            Controls.Add(lblTitulo);
            Controls.Add(btnVolver);
            Controls.Add(btnRegistrar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "VistaEstudiosGuardia";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Estudios de guardia e internación";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnVolver;
        private Label lblTitulo;
        private Label lblDni;
        private TextBox txtDni;
        private Label lblPaciente;
        private TextBox txtPaciente;
        private Label lblEstudio;
        private TextBox txtEstudio;
        private Label lblOrigen;
        private ComboBox cmbOrigen;
        private Label lblPrioridad;
        private ComboBox cmbPrioridad;
        private Label lblOrden;
        private TextBox txtOrden;
        private CheckBox chkOrdenValida;
        private Label lblAviso;
        private Button btnRegistrar;
    }
}
