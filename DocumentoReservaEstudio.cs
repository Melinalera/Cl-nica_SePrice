using System.Data;
using System.Drawing.Printing;

namespace Clínica_SePrice
{
    internal sealed class DocumentoReservaEstudio : PrintDocument
    {
        private readonly DataRow turno;

        internal DocumentoReservaEstudio(DataRow turno)
        {
            this.turno = turno;
            DocumentName = $"Reserva_estudio_{turno["id_turno"]}";
            DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            DefaultPageSettings.Margins = new Margins(60, 60, 60, 60);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            e.HasMorePages = false;
            if (e.Graphics is not Graphics dibujo)
                return;
            using var titulo = new Font("Palatino Linotype", 18F, FontStyle.Bold);
            using var subtitulo = new Font("Tahoma", 12F, FontStyle.Bold);
            using var normal = new Font("Tahoma", 11F);
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            float ancho = e.MarginBounds.Width;
            y = Escribir(dibujo, "Clínica SePrice", titulo, x, y, ancho);
            y = Escribir(dibujo, $"Comprobante de turno para estudio N.º {turno["id_turno"]}", subtitulo, x, y, ancho);
            y += 20;
            y = Escribir(dibujo, $"Paciente: {turno["paciente"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"DNI: {turno["dni_paciente"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Estudio: {turno["estudio"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Fecha: {((DateTime)turno["fecha"]):dd/MM/yyyy}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Hora: {((TimeSpan)turno["hora"]).ToString("hh':'mm")}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Estado: {turno["estado"]}", normal, x, y, ancho);
            y += 20;
            Escribir(dibujo, "Preséntese con su documento de identidad, la orden médica y la preparación indicada para el estudio.",
                normal, x, y, ancho);
        }

        private static float Escribir(Graphics dibujo, string texto, Font fuente,
            float x, float y, float ancho)
        {
            float alto = dibujo.MeasureString(texto, fuente, new SizeF(ancho, 1000)).Height;
            dibujo.DrawString(texto, fuente, Brushes.Black, new RectangleF(x, y, ancho, alto + 2));
            return y + alto + 8;
        }
    }
}
