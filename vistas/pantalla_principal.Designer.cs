namespace Clínica_SePrice.vistas
{
    partial class pantalla_principal
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
            btnTurnos = new Button();
            btnEstudios = new Button();
            btnInsumos = new Button();
            btnHistorial = new Button();
            SuspendLayout();
            // 
            // btnTurnos
            // 
            btnTurnos.Location = new Point(318, 95);
            btnTurnos.Name = "btnTurnos";
            btnTurnos.Size = new Size(133, 23);
            btnTurnos.TabIndex = 0;
            btnTurnos.Text = "Gestionar Turnos";
            btnTurnos.UseVisualStyleBackColor = true;
            // 
            // btnEstudios
            // 
            btnEstudios.Location = new Point(318, 135);
            btnEstudios.Name = "btnEstudios";
            btnEstudios.Size = new Size(133, 23);
            btnEstudios.TabIndex = 1;
            btnEstudios.Text = "Estudios Médicos";
            btnEstudios.UseVisualStyleBackColor = true;
            // 
            // btnInsumos
            // 
            btnInsumos.Location = new Point(318, 214);
            btnInsumos.Name = "btnInsumos";
            btnInsumos.Size = new Size(133, 23);
            btnInsumos.TabIndex = 3;
            btnInsumos.Text = "Gestión de Insumos";
            btnInsumos.UseVisualStyleBackColor = true;
            // 
            // btnHistorial
            // 
            btnHistorial.Location = new Point(318, 174);
            btnHistorial.Name = "btnHistorial";
            btnHistorial.Size = new Size(133, 23);
            btnHistorial.TabIndex = 2;
            btnHistorial.Text = "Historial Clínico";
            btnHistorial.UseVisualStyleBackColor = true;
            // 
            // pantalla_principal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnInsumos);
            Controls.Add(btnHistorial);
            Controls.Add(btnEstudios);
            Controls.Add(btnTurnos);
            Name = "pantalla_principal";
            Text = "pantalla_principal";
            ResumeLayout(false);
        }

        #endregion

        private Button btnTurnos;
        private Button btnEstudios;
        private Button btnInsumos;
        private Button btnHistorial;
    }
}