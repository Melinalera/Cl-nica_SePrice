using System.Data;
using System.Drawing.Printing;

namespace Clínica_SePrice
{
    internal sealed class DocumentoRetiroEstudio : PrintDocument
    {
        private readonly DataRow solicitud;
        private readonly DateTime emision = DateTime.Now;

        internal DocumentoRetiroEstudio(DataRow solicitud)
        {
            this.solicitud = solicitud;
            DocumentName = $"Retiro_estudio_{solicitud["id_solicitud"]}";
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
            y = Escribir(dibujo, "Comprobante de retiro del estudio", subtitulo, x, y, ancho);
            y += 20;
            y = Escribir(dibujo, $"Solicitud N.º {solicitud["id_solicitud"]}", subtitulo, x, y, ancho);
            y = Escribir(dibujo, $"Fecha de emisión: {emision:dd/MM/yyyy HH:mm}", normal, x, y, ancho);
            y += 15;
            y = Escribir(dibujo, $"Paciente: {solicitud["nombre_paciente"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"DNI: {solicitud["dni_paciente"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Estudio: {solicitud["estudio_solicitado"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Origen: {solicitud["origen"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Orden médica: {solicitud["referencia_orden"]}", normal, x, y, ancho);
            y = Escribir(dibujo, $"Fecha de solicitud: {((DateTime)solicitud["fecha_solicitud"]):dd/MM/yyyy HH:mm}",
                normal, x, y, ancho);
            y += 20;

            if (solicitud["fecha_entrega"] != DBNull.Value)
            {
                y = Escribir(dibujo, "Entrega registrada", subtitulo, x, y, ancho);
                y = Escribir(dibujo, $"Fecha: {((DateTime)solicitud["fecha_entrega"]):dd/MM/yyyy HH:mm}",
                    normal, x, y, ancho);
                Escribir(dibujo, $"Recibió: {solicitud["receptor"]}", normal, x, y, ancho);
            }
            else
            {
                string disponibilidad = solicitud["fecha_resultado"] == DBNull.Value
                    ? "Resultado pendiente de registro. Consulte disponibilidad en el sector de estudios."
                    : "Resultado registrado y pendiente de entrega.";
                y = Escribir(dibujo, disponibilidad, normal, x, y, ancho);
                y += 15;
                Escribir(dibujo, "Para retirar el resultado, presente este comprobante y el documento de identidad.",
                    normal, x, y, ancho);
            }
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
