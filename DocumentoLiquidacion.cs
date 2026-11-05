using System.Data;
using System.Drawing.Printing;
using System.Globalization;

namespace Clínica_SePrice
{
    internal sealed class DocumentoLiquidacion : PrintDocument
    {
        private readonly DataRow cabecera;
        private readonly DataTable atenciones;
        private readonly CultureInfo cultura = CultureInfo.GetCultureInfo("es-AR");
        private int siguienteFila;
        private int pagina;

        internal DocumentoLiquidacion(DataRow cabecera, DataTable atenciones)
        {
            this.cabecera = cabecera;
            this.atenciones = atenciones;
            DocumentName = $"Liquidacion_{cabecera["id_liquidacion"]}";
            DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            siguienteFila = 0;
            pagina = 1;
            base.OnBeginPrint(e);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            if (e.Graphics is not Graphics dibujo)
                return;
            e.HasMorePages = false;

            using var titulo = new Font("Palatino Linotype", 18F, FontStyle.Bold);
            using var subtitulo = new Font("Tahoma", 11F, FontStyle.Bold);
            using var normal = new Font("Tahoma", 9F);
            using var negrita = new Font("Tahoma", 9F, FontStyle.Bold);
            using var derecha = new StringFormat { Alignment = StringAlignment.Far };
            float x = e.MarginBounds.Left;
            float ancho = e.MarginBounds.Width;
            float y = e.MarginBounds.Top;

            y = Escribir(dibujo, "Clínica SePrice", titulo, x, y, ancho);
            y = Escribir(dibujo, $"Liquidación de honorarios N.º {cabecera["id_liquidacion"]}", subtitulo, x, y, ancho);
            y += 10;
            y = Escribir(dibujo, $"Profesional: {cabecera["medico"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Matrícula: {cabecera["matricula"]}    Especialidad: {cabecera["especialidad"]}", normal, x, y, ancho);
            y = Escribir(dibujo,
                $"Período: {((DateTime)cabecera["fecha_desde"]):dd/MM/yyyy} al {((DateTime)cabecera["fecha_hasta"]):dd/MM/yyyy}",
                normal, x, y, ancho);
            y = Escribir(dibujo, $"Emisión: {((DateTime)cabecera["fecha_emision"]):dd/MM/yyyy HH:mm}", normal, x, y, ancho);
            y += 15;

            float[] anchos = { ancho * .10f, ancho * .17f, ancho * .17f, ancho * .38f, ancho * .18f };
            string[] encabezados = { "Atención", "Fecha", "DNI", "Práctica", "Importe" };
            float columna = x;
            for (int i = 0; i < encabezados.Length; i++)
            {
                dibujo.FillRectangle(Brushes.LightGray, columna, y, anchos[i], 28);
                dibujo.DrawRectangle(Pens.Gray, columna, y, anchos[i], 28);
                dibujo.DrawString(encabezados[i], negrita, Brushes.Black,
                    new RectangleF(columna + 4, y + 5, anchos[i] - 8, 23));
                columna += anchos[i];
            }
            y += 28;

            while (siguienteFila < atenciones.Rows.Count)
            {
                DataRow fila = atenciones.Rows[siguienteFila];
                string[] valores =
                {
                    fila["id_atencion"].ToString() ?? "",
                    ((DateTime)fila["fecha_atencion"]).ToString("dd/MM/yyyy"),
                    fila["dni_paciente"].ToString() ?? "",
                    fila["practica"].ToString() ?? "",
                    ((decimal)fila["importe"]).ToString("C2", cultura)
                };
                float alto = 28;
                for (int i = 0; i < valores.Length; i++)
                    alto = Math.Max(alto, dibujo.MeasureString(valores[i], normal,
                        new SizeF(anchos[i] - 8, 1000)).Height + 10);

                if (y + alto > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    break;
                }
                columna = x;
                for (int i = 0; i < valores.Length; i++)
                {
                    dibujo.DrawRectangle(Pens.LightGray, columna, y, anchos[i], alto);
                    dibujo.DrawString(valores[i], normal, Brushes.Black,
                        new RectangleF(columna + 4, y + 5, anchos[i] - 8, alto - 8),
                        i == valores.Length - 1 ? derecha : StringFormat.GenericDefault);
                    columna += anchos[i];
                }
                y += alto;
                siguienteFila++;
            }

            if (!e.HasMorePages)
            {
                y += 15;
                y = Escribir(dibujo, $"Atenciones incluidas: {atenciones.Rows.Count}", normal, x, y, ancho);
                Escribir(dibujo, $"Total liquidado: {((decimal)cabecera["importe_total"]).ToString("C2", cultura)}",
                    subtitulo, x, y, ancho);
            }
            dibujo.DrawString($"Página {pagina}", normal, Brushes.Black,
                new RectangleF(x, e.MarginBounds.Bottom + 10, ancho, 25), derecha);
            pagina++;
        }

        private static float Escribir(Graphics dibujo, string texto, Font fuente,
            float x, float y, float ancho)
        {
            float alto = dibujo.MeasureString(texto, fuente, new SizeF(ancho, 1000)).Height;
            dibujo.DrawString(texto, fuente, Brushes.Black, new RectangleF(x, y, ancho, alto + 2));
            return y + alto + 4;
        }
    }
}
