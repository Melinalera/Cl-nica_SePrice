namespace Clínica_SePrice
{
    partial class Estudios_Clínicos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Estudios_Clínicos));
            pictureBox1 = new PictureBox();
            Laboratorio = new RadioButton();
            Electrocardiograma = new RadioButton();
            Ecografía = new RadioButton();
            Radiografía = new RadioButton();
            monthCalendar1 = new MonthCalendar();
            label1 = new Label();
            Aceptar = new Button();
            Atrás = new Button();
            Cancelar = new Button();
            comboBox1 = new ComboBox();
            label2 = new Label();
            Tomografía = new RadioButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(241, 25);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(294, 125);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // Laboratorio
            // 
            Laboratorio.AutoSize = true;
            Laboratorio.Location = new Point(12, 245);
            Laboratorio.Name = "Laboratorio";
            Laboratorio.Size = new Size(86, 19);
            Laboratorio.TabIndex = 10;
            Laboratorio.TabStop = true;
            Laboratorio.Text = "Laboratorio";
            Laboratorio.UseVisualStyleBackColor = true;
            Laboratorio.Click += this.Laboratorio_Click;
            // 
            // Electrocardiograma
            // 
            Electrocardiograma.AutoSize = true;
            Electrocardiograma.Location = new Point(12, 270);
            Electrocardiograma.Name = "Electrocardiograma";
            Electrocardiograma.Size = new Size(128, 19);
            Electrocardiograma.TabIndex = 11;
            Electrocardiograma.TabStop = true;
            Electrocardiograma.Text = "Electrocardiograma";
            Electrocardiograma.UseVisualStyleBackColor = true;
            Electrocardiograma.Click += Electrocardiograma_Click;
            // 
            // Ecografía
            // 
            Ecografía.AutoSize = true;
            Ecografía.Location = new Point(12, 295);
            Ecografía.Name = "Ecografía";
            Ecografía.Size = new Size(74, 19);
            Ecografía.TabIndex = 12;
            Ecografía.TabStop = true;
            Ecografía.Text = "Ecografía";
            Ecografía.UseVisualStyleBackColor = true;
            Ecografía.CheckedChanged += radioButton3_CheckedChanged;
            Ecografía.Click += Ecografía_Click;
            // 
            // Radiografía
            // 
            Radiografía.AutoSize = true;
            Radiografía.Location = new Point(12, 320);
            Radiografía.Name = "Radiografía";
            Radiografía.Size = new Size(85, 19);
            Radiografía.TabIndex = 13;
            Radiografía.TabStop = true;
            Radiografía.Text = "Radiografía";
            Radiografía.UseVisualStyleBackColor = true;
            Radiografía.CheckedChanged += radioButton4_CheckedChanged;
            Radiografía.Click += Radiografía_Click;
            // 
            // monthCalendar1
            // 
            monthCalendar1.Location = new Point(241, 215);
            monthCalendar1.Name = "monthCalendar1";
            monthCalendar1.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(353, 164);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 15;
            label1.Text = "label1";
            // 
            // Aceptar
            // 
            Aceptar.Location = new Point(533, 354);
            Aceptar.Name = "Aceptar";
            Aceptar.Size = new Size(75, 23);
            Aceptar.TabIndex = 16;
            Aceptar.Text = "Aceptar";
            Aceptar.UseVisualStyleBackColor = true;
            Aceptar.Click += btnAceptar_Click;
            // 
            // Atrás
            // 
            Atrás.Location = new Point(614, 354);
            Atrás.Name = "Atrás";
            Atrás.Size = new Size(75, 23);
            Atrás.TabIndex = 17;
            Atrás.Text = "Atrás";
            Atrás.UseVisualStyleBackColor = true;
            Atrás.Click += btnAtrás_Click;
            // 
            // Cancelar
            // 
            Cancelar.Location = new Point(695, 354);
            Cancelar.Name = "Cancelar";
            Cancelar.Size = new Size(75, 23);
            Cancelar.TabIndex = 18;
            Cancelar.Text = "Cancelar";
            Cancelar.UseVisualStyleBackColor = true;
            Cancelar.Click += btnCancelar_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(487, 241);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 19;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 201);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 20;
            label2.Text = "label2";
            // 
            // Tomografía
            // 
            Tomografía.AutoSize = true;
            Tomografía.Location = new Point(13, 346);
            Tomografía.Name = "Tomografía";
            Tomografía.Size = new Size(86, 19);
            Tomografía.TabIndex = 21;
            Tomografía.TabStop = true;
            Tomografía.Text = "Tomografía";
            Tomografía.UseVisualStyleBackColor = true;
            Tomografía.Click += Tomografía_Click;
            // 
            // Estudios_Clínicos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Tomografía);
            Controls.Add(label2);
            Controls.Add(comboBox1);
            Controls.Add(Cancelar);
            Controls.Add(Atrás);
            Controls.Add(Aceptar);
            Controls.Add(label1);
            Controls.Add(monthCalendar1);
            Controls.Add(Radiografía);
            Controls.Add(Ecografía);
            Controls.Add(Electrocardiograma);
            Controls.Add(Laboratorio);
            Controls.Add(pictureBox1);
            Name = "Estudios_Clínicos";
            Text = "Estudios_Clínicos";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private RadioButton Electrocardiograma;
        private RadioButton Ecografía;
        private RadioButton Radiografía;
        private MonthCalendar monthCalendar1;
        private Label label1;
        private Button Aceptar;
        private Button Atrás;
        private Button Cancelar;
        private ComboBox comboBox1;
        private Label label2;
        private RadioButton Laboratorio;
        private RadioButton Tomografía;
    }
}