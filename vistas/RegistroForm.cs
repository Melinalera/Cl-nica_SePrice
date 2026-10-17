using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice.vistas
{
    public partial class RegistroForm : Form
    {
        public RegistroForm()
        {
            InitializeComponent();
        }

        private void RegistroForm_Load(object sender, EventArgs e)
        {
            string cadenaConexion = "server=localhost; database=clínica_seprice; uid=root; pwd=1234;";

            using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
            {
                try
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conexion))

                    



                }
            }

        }
    }
}