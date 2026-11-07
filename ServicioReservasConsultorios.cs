using System.Data;
using MySql.Data.MySqlClient;

namespace Clínica_SePrice
{
    internal static class ServicioReservasConsultorios
    {
        internal static DataTable ConsultarHorarios(MySqlConnection conexion, int idAgenda,
            bool sobreturno, MySqlTransaction? transaccion = null)
        {
            using var comando = new MySqlCommand("""
                SELECT a.*, m.especialidad
                FROM agendas_consultorios a
                JOIN medicos m ON m.id_usuario = a.id_medico
                JOIN usuarios u ON u.id_usuario = m.id_usuario
                JOIN consultorios c ON c.id_consultorio = a.id_consultorio
                WHERE a.id_agenda = @id AND c.activo = TRUE AND u.rol = 'medico'
                """, conexion, transaccion);
            comando.Parameters.AddWithValue("@id", idAgenda);
            using var adaptador = new MySqlDataAdapter(comando);
            using var agendas = new DataTable();
            adaptador.Fill(agendas);
            if (agendas.Rows.Count == 0)
                throw new InvalidOperationException("La agenda ya no está disponible.");
            DataRow agenda = agendas.Rows[0];
            int minutos = Convert.ToInt32(agenda["duracion_minutos"]);
            if (minutos < ReglasAgendaConsultorios.DuracionMinima((string)agenda["especialidad"]))
                throw new InvalidOperationException("Revisá la duración de la agenda y la especialidad del profesional.");

            using var ocupadosComando = new MySqlCommand("""
                SELECT r.id_agenda, r.hora, r.es_sobreturno
                FROM reservas_agenda_consultorios r
                JOIN agendas_consultorios a ON a.id_agenda = r.id_agenda
                WHERE a.id_medico = @medico AND a.fecha = @fecha
                """, conexion, transaccion);
            ocupadosComando.Parameters.AddWithValue("@medico", agenda["id_medico"]);
            ocupadosComando.Parameters.AddWithValue("@fecha", agenda["fecha"]);
            using var ocupadosAdaptador = new MySqlDataAdapter(ocupadosComando);
            using var ocupados = new DataTable();
            ocupadosAdaptador.Fill(ocupados);

            var horarios = new DataTable();
            horarios.Columns.Add("Hora", typeof(TimeSpan));
            horarios.Columns.Add("Descripcion", typeof(string));
            var reservas = ocupados.AsEnumerable().Select(fila => (
                Agenda: (int)fila["id_agenda"],
                Hora: (TimeSpan)fila["hora"],
                Sobreturno: Convert.ToBoolean(fila["es_sobreturno"]))).ToArray();
            foreach (TimeSpan hora in GenerarHorarios((DateTime)agenda["fecha"],
                         (TimeSpan)agenda["hora_inicio"], (TimeSpan)agenda["hora_fin"],
                         minutos, idAgenda, sobreturno, reservas, DateTime.Now))
            {
                TimeSpan fin = hora.Add(TimeSpan.FromMinutes(minutos));
                horarios.Rows.Add(hora, $"{hora.ToString("hh':'mm")} - {fin.ToString("hh':'mm")}" + (sobreturno ? " (sobreturno)" : ""));
            }
            return horarios;
        }

        internal static IEnumerable<TimeSpan> GenerarHorarios(DateTime fecha, TimeSpan inicio,
            TimeSpan fin, int minutos, int idAgenda, bool sobreturno,
            (int Agenda, TimeSpan Hora, bool Sobreturno)[] reservas, DateTime ahora)
        {
            if (minutos <= 0)
                yield break;
            for (TimeSpan hora = inicio; hora.Add(TimeSpan.FromMinutes(minutos)) <= fin;
                 hora = hora.Add(TimeSpan.FromMinutes(minutos)))
            {
                if (fecha.Date.Add(hora) <= ahora)
                    continue;
                bool ocupado = reservas.Any(reserva => reserva.Agenda == idAgenda &&
                    reserva.Hora == hora && !reserva.Sobreturno);
                if (!sobreturno && !ocupado)
                    yield return hora;
                else if (sobreturno && ocupado &&
                         !reservas.Any(reserva => reserva.Sobreturno && reserva.Hora.Hours == hora.Hours))
                    yield return hora;
            }
        }

        // Ambos circuitos bloquean primero la ficha del paciente antes de comprobar cruces.
        internal static void VerificarDisponibilidadPaciente(MySqlConnection conexion,
            MySqlTransaction transaccion, string dni, DateTime fecha, TimeSpan inicio, TimeSpan fin)
        {
            using var consulta = new MySqlCommand("""
                SELECT r.id_turno FROM reservas_agenda_consultorios r
                JOIN agendas_consultorios a ON a.id_agenda = r.id_agenda
                JOIN `turnosConsultoriosMédicos` t ON t.id_turno = r.id_turno
                WHERE t.dni_paciente = @dni AND a.fecha = @fecha
                  AND r.hora < @fin
                  AND ADDTIME(r.hora, SEC_TO_TIME(a.duracion_minutos * 60)) > @inicio
                UNION ALL
                SELECT id_turno FROM `turnosEstudiosClínicos`
                WHERE dni_paciente = @dni AND fecha = @fecha
                  AND hora < @fin AND ADDTIME(hora, '01:00:00') > @inicio
                LIMIT 1
                """, conexion, transaccion);
            consulta.Parameters.AddWithValue("@dni", dni);
            consulta.Parameters.AddWithValue("@fecha", fecha);
            consulta.Parameters.AddWithValue("@inicio", inicio);
            consulta.Parameters.AddWithValue("@fin", fin);
            if (consulta.ExecuteScalar() != null)
                throw new InvalidOperationException("El paciente ya tiene un turno de consulta o estudio que se superpone con ese horario.");
        }
        internal static long Reservar(int idPaciente, string dni, int idAgenda, TimeSpan hora, bool sobreturno)
        {
            using var conexion = new MySqlConnection(ConexionBaseDatos.CadenaConexion);
            conexion.Open();
            using var transaccion = conexion.BeginTransaction();

            // Serializar las reservas del profesional, incluso entre agendas de la misma hora.
            using var bloquear = new MySqlCommand("""
                SELECT m.id_usuario
                FROM medicos m JOIN agendas_consultorios a ON a.id_medico = m.id_usuario
                WHERE a.id_agenda = @agenda FOR UPDATE
                """, conexion, transaccion);
            bloquear.Parameters.AddWithValue("@agenda", idAgenda);
            if (bloquear.ExecuteScalar() == null)
                throw new InvalidOperationException("La agenda ya no está disponible.");

            // Serializar también las reservas del paciente para evitar turnos simultáneos.
            using var paciente = new MySqlCommand(
                "SELECT id_paciente FROM pacientes WHERE id_paciente = @paciente AND dni = @dni FOR UPDATE",
                conexion, transaccion);
            paciente.Parameters.AddWithValue("@paciente", idPaciente);
            paciente.Parameters.AddWithValue("@dni", dni);
            if (paciente.ExecuteScalar() == null)
                throw new InvalidOperationException("La ficha del paciente cambió. Volvé a buscar el DNI.");

            using var disponibles = ConsultarHorarios(conexion, idAgenda, sobreturno, transaccion);
            if (!disponibles.AsEnumerable().Any(fila => fila.Field<TimeSpan>("Hora") == hora))
                throw new InvalidOperationException("El horario ya no está disponible. Actualizá los horarios.");

            using var agendaComando = new MySqlCommand(
                "SELECT fecha, duracion_minutos FROM agendas_consultorios WHERE id_agenda = @id",
                conexion, transaccion);
            agendaComando.Parameters.AddWithValue("@id", idAgenda);
            using var agendaAdaptador = new MySqlDataAdapter(agendaComando);
            using var datosAgenda = new DataTable();
            agendaAdaptador.Fill(datosAgenda);
            DataRow agenda = datosAgenda.Rows[0];
            TimeSpan fin = hora.Add(TimeSpan.FromMinutes(Convert.ToInt32(agenda["duracion_minutos"])));
            VerificarDisponibilidadPaciente(conexion, transaccion, dni, (DateTime)agenda["fecha"], hora, fin);

            using var insertar = new MySqlCommand("""
                INSERT INTO `turnosConsultoriosMédicos` (especialidad, fecha, hora, dni_paciente, estado)
                SELECT m.especialidad, a.fecha, @hora, @dni, 'Reservado'
                FROM agendas_consultorios a JOIN medicos m ON m.id_usuario = a.id_medico
                WHERE a.id_agenda = @agenda
                """, conexion, transaccion);
            insertar.Parameters.AddWithValue("@hora", hora);
            insertar.Parameters.AddWithValue("@dni", dni);
            insertar.Parameters.AddWithValue("@agenda", idAgenda);
            if (insertar.ExecuteNonQuery() != 1)
                throw new InvalidOperationException("La agenda ya no está disponible.");
            long idTurno = insertar.LastInsertedId;
            using var asignar = new MySqlCommand("""
                INSERT INTO reservas_agenda_consultorios (id_turno, id_agenda, hora, es_sobreturno)
                VALUES (@turno, @agenda, @hora, @sobreturno)
                """, conexion, transaccion);
            asignar.Parameters.AddWithValue("@turno", idTurno);
            asignar.Parameters.AddWithValue("@agenda", idAgenda);
            asignar.Parameters.AddWithValue("@hora", hora);
            asignar.Parameters.AddWithValue("@sobreturno", sobreturno);
            asignar.ExecuteNonQuery();
            transaccion.Commit();
            return idTurno;
        }
    }
}
