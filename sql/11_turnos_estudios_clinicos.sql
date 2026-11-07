USE clinica_seprice;

-- Incorporar la tabla nueva sin ejecutar el reinicio de mysql.sql.
CREATE TABLE IF NOT EXISTS `turnosEstudiosClínicos` (
    id_turno INT AUTO_INCREMENT PRIMARY KEY,
    estudio VARCHAR(50) NOT NULL,
    fecha DATE NOT NULL,
    hora TIME NOT NULL,
    dni_paciente VARCHAR(20),
    estado VARCHAR(20) DEFAULT 'Reservado',
    CONSTRAINT uq_turnos_estudios_horario UNIQUE (estudio, fecha, hora)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Si la tabla se creó con el script original del compañero, agregar la protección
-- contra reservas simultáneas del mismo horario. Si hay duplicados previos, MySQL
-- rechazará el índice; se deben revisar esos registros antes de continuar.
SET @seprice_indice_estudios = (
    SELECT COUNT(*) FROM information_schema.statistics
    WHERE table_schema = DATABASE() AND table_name = 'turnosEstudiosClínicos'
      AND index_name = 'uq_turnos_estudios_horario'
);
SET @seprice_actualizar_estudios = IF(@seprice_indice_estudios = 0,
    'ALTER TABLE `turnosEstudiosClínicos` ADD CONSTRAINT uq_turnos_estudios_horario UNIQUE (estudio, fecha, hora)',
    'SELECT ''Los turnos de estudios ya tienen protección de horarios duplicados.'' AS informacion');
PREPARE seprice_estudios_indice FROM @seprice_actualizar_estudios;
EXECUTE seprice_estudios_indice;
DEALLOCATE PREPARE seprice_estudios_indice;
