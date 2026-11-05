namespace Clínica_SePrice.vistas
{
    partial class VistaTurnos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VistaTurnos));
            label1 = new Label();
            btnVolver = new Button();
            ConsultoriosExternos = new Button();
            EstudiosClínicos = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Palatino Linotype", 14F, FontStyle.Bold);
            label1.Location = new Point(317, 161);
            label1.Name = "label1";
            label1.Size = new Size(154, 26);
            label1.TabIndex = 1;
            label1.Text = "Turnos Medicos";
            label1.Click += label1_Click;
            // 
            // btnVolver
            // 
            btnVolver.BackColor = Color.Teal;
            btnVolver.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnVolver.ForeColor = SystemColors.ControlLight;
            btnVolver.Location = new Point(12, 12);
            btnVolver.Name = "btnVolver";
            btnVolver.Size = new Size(75, 23);
            btnVolver.TabIndex = 4;
            btnVolver.Text = "Volver";
            btnVolver.UseVisualStyleBackColor = false;
            btnVolver.Click += btnVolver_Click;
            // 
            // ConsultoriosExternos
            // 
            ConsultoriosExternos.Location = new Point(286, 235);
            ConsultoriosExternos.Name = "ConsultoriosExternos";
            ConsultoriosExternos.Size = new Size(239, 23);
            ConsultoriosExternos.TabIndex = 5;
            ConsultoriosExternos.Text = "Consultorios Externos";
            ConsultoriosExternos.UseVisualStyleBackColor = true;
            ConsultoriosExternos.Click += ConsultoriosExternos_click;
            // 
            // EstudiosClínicos
            // 
            EstudiosClínicos.Location = new Point(286, 312);
            EstudiosClínicos.Name = "EstudiosClínicos";
            EstudiosClínicos.Size = new Size(239, 23);
            EstudiosClínicos.TabIndex = 6;
            EstudiosClínicos.Text = "Estudios Clínicos";
            EstudiosClínicos.UseVisualStyleBackColor = true;
            EstudiosClínicos.Click += EstudiosClínicos_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(253, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(294, 125);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // VistaTurnos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(EstudiosClínicos);
            Controls.Add(ConsultoriosExternos);
            Controls.Add(btnVolver);
            Controls.Add(label1);
            Name = "VistaTurnos";
            Text = "VistaTurnos";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnVolver;
        private Button ConsultoriosExternos;
        private Button EstudiosClínicos;
        private PictureBox pictureBox1;
    }
}