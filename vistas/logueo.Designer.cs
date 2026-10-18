namespace Clínica_SePrice.vistas
{
    partial class logueo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(logueo));
            usuario = new TextBox();
            contraseña = new TextBox();
            button1 = new Button();
            label1 = new Label();
            btnRegistrarse = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // usuario
            // 
            usuario.Location = new Point(309, 214);
            usuario.Name = "usuario";
            usuario.Size = new Size(100, 23);
            usuario.TabIndex = 0;
            usuario.Text = "Melina ";
            usuario.TextChanged += textBox1_TextChanged;
            // 
            // contraseña
            // 
            contraseña.Location = new Point(309, 269);
            contraseña.Name = "contraseña";
            contraseña.Size = new Size(100, 23);
            contraseña.TabIndex = 1;
            contraseña.Text = "1234";
            contraseña.TextChanged += textBox2_TextChanged;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.MenuHighlight;
            button1.ForeColor = SystemColors.ActiveCaptionText;
            button1.Location = new Point(318, 313);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "Ingresar";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.MenuHighlight;
            label1.Font = new Font("Gloucester MT Extra Condensed", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.AppWorkspace;
            label1.Location = new Point(258, 159);
            label1.Name = "label1";
            label1.Size = new Size(221, 25);
            label1.TabIndex = 3;
            label1.Text = "¡Bienvenidos a clínica SePrice!\r\n";
            label1.Click += label1_Click;
            // 
            // btnRegistrarse
            // 
            btnRegistrarse.Location = new Point(318, 369);
            btnRegistrarse.Name = "btnRegistrarse";
            btnRegistrarse.Size = new Size(75, 23);
            btnRegistrarse.TabIndex = 5;
            btnRegistrarse.Text = "Registrarse\r\n";
            btnRegistrarse.UseVisualStyleBackColor = true;
            btnRegistrarse.Click += btnRegistrarse_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(229, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(294, 136);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // logueo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(btnRegistrarse);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(contraseña);
            Controls.Add(usuario);
            Name = "logueo";
            Text = "Form1";
            Load += logueo_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox usuario;
        private TextBox contraseña;
        private Button button1;
        private Label label1;
        private Button btnRegistrarse;
        private PictureBox pictureBox1;
    }
}