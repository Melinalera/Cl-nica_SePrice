using System.Globalization;
using System.Text;

namespace Clínica_SePrice
{
    internal static class ReglasAgendaConsultorios
    {
        internal static int DuracionMinima(string especialidad)
        {
            string texto = string.Concat(especialidad.Normalize(NormalizationForm.FormD)
                .Where(caracter => CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark))
                .ToLowerInvariant();
            if (texto.Contains("kines") || texto.Contains("fisio"))
                return 25;
            if (texto.Contains("salud mental") || texto.Contains("psicolog") || texto.Contains("psiquiatr"))
                return 30;
            return 15;
        }
    }
}
