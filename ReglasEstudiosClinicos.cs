namespace Clínica_SePrice
{
    internal static class ReglasEstudiosClinicos
    {
        // Franjas de una hora y días definidos en la pantalla incorporada desde master.
        internal static DayOfWeek[] DiasAtencion(string estudio) => estudio switch
        {
            "Electrocardiograma" => new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
            "Ecografía" => new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday },
            "Radiografía" => new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday },
            "Tomografía" => new[] { DayOfWeek.Wednesday, DayOfWeek.Friday },
            _ => Array.Empty<DayOfWeek>()
        };

        internal static TimeSpan[] HorariosBase(string estudio) => estudio switch
        {
            "Electrocardiograma" => new[] { new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0) },
            "Ecografía" => new[] { new TimeSpan(11, 0, 0), new TimeSpan(12, 0, 0) },
            "Radiografía" => new[] { new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0) },
            "Tomografía" => new[] { new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0) },
            _ => Array.Empty<TimeSpan>()
        };

        internal static bool HorarioValido(string estudio, DateTime fecha, TimeSpan hora, DateTime ahora)
        {
            return DiasAtencion(estudio).Contains(fecha.DayOfWeek) &&
                   HorariosBase(estudio).Contains(hora) && fecha.Date.Add(hora) > ahora;
        }
    }
}
