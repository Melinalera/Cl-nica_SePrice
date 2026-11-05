using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Clínica_SePrice
{
    public partial class TurnosConsultoriosMédicos : Form
    {
        public TurnosConsultoriosMédicos()
        {
            InitializeComponent();
            // Suscribimos todos los Radio Button al mismo evento al iniciar
            Cardiología.CheckedChanged += Especialidad_CheckedChanged;
            Dermatología.CheckedChanged += Especialidad_CheckedChanged;
            Gastroenterología.CheckedChanged+= Especialidad_CheckedChanged;
            Neumonología.CheckedChanged += Especialidad_CheckedChanged;


        }

        private void Especialidad_CheckedChanged(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
