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
            usuario = new TextBox();
            contraseña = new TextBox();
            button1 = new Button();
            label1 = new Label();
            btnRegistrarse = new Button();
            SuspendLayout();
            // 
            // usuario
            // 
            usuario.Location = new Point(309, 174);
            usuario.Name = "usuario";
            usuario.Size = new Size(100, 23);
            usuario.TabIndex = 0;
            usuario.Text = "Melina ";
            usuario.TextChanged += textBox1_TextChanged;
            // 
            // contraseña
            // 
            contraseña.Location = new Point(309, 236);
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
            button1.Location = new Point(318, 286);
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
            label1.Location = new Point(264, 120);
            label1.Name = "label1";
            label1.Size = new Size(221, 25);
            label1.TabIndex = 3;
            label1.Text = "¡Bienvenidos a clínica SePrice!\r\n";
            label1.Click += label1_Click;
            // 
            // btnRegistrarse
            // 
            btnRegistrarse.Location = new Point(318, 330);
            btnRegistrarse.Name = "btnRegistrarse";
            btnRegistrarse.Size = new Size(75, 23);
            btnRegistrarse.TabIndex = 5;
            btnRegistrarse.Text = "Registrarse\r\n";
            btnRegistrarse.UseVisualStyleBackColor = true;
            btnRegistrarse.Click += btnRegistrarse_Click;
            // 
            // logueo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnRegistrarse);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(contraseña);
            Controls.Add(usuario);
            Name = "logueo";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox usuario;
        private TextBox contraseña;
        private Button button1;
        private Label label1;
        private Button btnRegistrarse;
    }
}