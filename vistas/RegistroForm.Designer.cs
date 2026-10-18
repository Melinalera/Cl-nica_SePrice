namespace Clínica_SePrice.vistas
{
    partial class RegistroForm // <-- Cambiado de Form1 a RegistroForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistroForm));
            label1 = new Label();
            Ap = new Label();
            us = new Label();
            em = new Label();
            tel = new Label();
            paciente = new RadioButton();
            Médico = new RadioButton();
            administrativo = new RadioButton();
            nombre = new MaskedTextBox();
            apellido = new MaskedTextBox();
            usuario = new MaskedTextBox();
            email = new MaskedTextBox();
            label6 = new Label();
            telefono = new MaskedTextBox();
            rol = new GroupBox();
            Enviar = new Button();
            atras = new Button();
            pictureBox1 = new PictureBox();
            rol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 153);
            label1.Name = "label1";
            label1.Size = new Size(107, 15);
            label1.TabIndex = 0;
            label1.Text = "Ingrese su Nombre";
            // 
            // Ap
            // 
            Ap.AutoSize = true;
            Ap.Location = new Point(37, 207);
            Ap.Name = "Ap";
            Ap.Size = new Size(51, 15);
            Ap.TabIndex = 1;
            Ap.Text = "Apellido";
            // 
            // us
            // 
            us.AutoSize = true;
            us.Location = new Point(37, 251);
            us.Name = "us";
            us.Size = new Size(46, 15);
            us.TabIndex = 2;
            us.Text = "usuario";
            // 
            // em
            // 
            em.AutoSize = true;
            em.Location = new Point(37, 295);
            em.Name = "em";
            em.Size = new Size(36, 15);
            em.TabIndex = 3;
            em.Text = "email";
            // 
            // tel
            // 
            tel.AutoSize = true;
            tel.Location = new Point(37, 348);
            tel.Name = "tel";
            tel.Size = new Size(53, 15);
            tel.TabIndex = 4;
            tel.Text = "Telefonó";
            // 
            // paciente
            // 
            paciente.AutoSize = true;
            paciente.Location = new Point(18, 22);
            paciente.Name = "paciente";
            paciente.Size = new Size(70, 19);
            paciente.TabIndex = 5;
            paciente.TabStop = true;
            paciente.Text = "Paciente";
            paciente.UseVisualStyleBackColor = true;
            // 
            // Médico
            // 
            Médico.AutoSize = true;
            Médico.Location = new Point(18, 47);
            Médico.Name = "Médico";
            Médico.Size = new Size(65, 19);
            Médico.TabIndex = 6;
            Médico.TabStop = true;
            Médico.Text = "Médico";
            Médico.UseVisualStyleBackColor = true;
            // 
            // administrativo
            // 
            administrativo.AutoSize = true;
            administrativo.Location = new Point(18, 72);
            administrativo.Name = "administrativo";
            administrativo.Size = new Size(103, 19);
            administrativo.TabIndex = 7;
            administrativo.TabStop = true;
            administrativo.Text = "Administrativo";
            administrativo.UseVisualStyleBackColor = true;
            // 
            // nombre
            // 
            nombre.Location = new Point(37, 181);
            nombre.Name = "nombre";
            nombre.Size = new Size(100, 23);
            nombre.TabIndex = 8;
            // 
            // apellido
            // 
            apellido.Location = new Point(37, 225);
            apellido.Name = "apellido";
            apellido.Size = new Size(100, 23);
            apellido.TabIndex = 9;
            // 
            // usuario
            // 
            usuario.Location = new Point(37, 269);
            usuario.Name = "usuario";
            usuario.Size = new Size(100, 23);
            usuario.TabIndex = 10;
            // 
            // email
            // 
            email.Location = new Point(37, 313);
            email.Name = "email";
            email.Size = new Size(100, 23);
            email.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = SystemColors.MenuHighlight;
            label6.Font = new Font("Gloucester MT Extra Condensed", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.AppWorkspace;
            label6.Location = new Point(254, 96);
            label6.Name = "label6";
            label6.Size = new Size(0, 25);
            label6.TabIndex = 12;
            // 
            // telefono
            // 
            telefono.Location = new Point(37, 366);
            telefono.Name = "telefono";
            telefono.Size = new Size(100, 23);
            telefono.TabIndex = 13;
            // 
            // rol
            // 
            rol.Controls.Add(paciente);
            rol.Controls.Add(Médico);
            rol.Controls.Add(administrativo);
            rol.Location = new Point(306, 210);
            rol.Name = "rol";
            rol.Size = new Size(200, 100);
            rol.TabIndex = 14;
            rol.TabStop = false;
            rol.Text = "Rol";
            // 
            // Enviar
            // 
            Enviar.Location = new Point(571, 362);
            Enviar.Name = "Enviar";
            Enviar.Size = new Size(75, 23);
            Enviar.TabIndex = 15;
            Enviar.Text = "Enviar";
            Enviar.UseVisualStyleBackColor = true;
            Enviar.Click += Enviar_Click;
            // 
            // atras
            // 
            atras.Location = new Point(687, 362);
            atras.Name = "atras";
            atras.Size = new Size(75, 23);
            atras.TabIndex = 16;
            atras.Text = "Atrás";
            atras.UseVisualStyleBackColor = true;
            atras.Click += atras_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(226, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(294, 136);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            // 
            // RegistroForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(atras);
            Controls.Add(Enviar);
            Controls.Add(rol);
            Controls.Add(telefono);
            Controls.Add(label6);
            Controls.Add(email);
            Controls.Add(usuario);
            Controls.Add(apellido);
            Controls.Add(nombre);
            Controls.Add(tel);
            Controls.Add(em);
            Controls.Add(us);
            Controls.Add(Ap);
            Controls.Add(label1);
            Name = "RegistroForm";
            Text = "RegistroForm";
            Load += RegistroForm_Load;
            rol.ResumeLayout(false);
            rol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private MaskedTextBox maskedTextBox1;
        private MaskedTextBox maskedTextBox2;
        private MaskedTextBox maskedTextBox3;
        private MaskedTextBox maskedTextBox4;
        private Label label6;
        private MaskedTextBox nombre;
        private Label usario;
        private MaskedTextBox usuario;
        private Label Apellido;
        private MaskedTextBox apellido;
        private MaskedTextBox email;
        private Label Ap;
        private Label us;
        private Label em;
        private Label tel;
        private MaskedTextBox telefono;
        private GroupBox groupBox1;
        private Button Enviar;
        private RadioButton paciente;
        private RadioButton Médico;
        private RadioButton administrativo;
        private GroupBox rol;
        private Button atras;
        private PictureBox pictureBox1;
    }
}