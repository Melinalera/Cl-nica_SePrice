using System;
using System.Windows.Forms;
using Clínica_SePrice.vistas; // <--- Agregado para encontrar la clase logueo

namespace Clínica_SePrice
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            // Inicia la aplicación cargando directamente tu pantalla de logueo
            Application.Run(new FrmControlInsumos()); 
        }
    }
}