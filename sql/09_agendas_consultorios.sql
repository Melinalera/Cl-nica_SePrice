USE clinica_seprice;

-- Espacios físicos disponibles para la atención.
CREATE TABLE IF NOT EXISTS consultorios (
    id_consultorio INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_consultorios_nombre UNIQUE (nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Cada agenda corresponde a un profesional, un consultorio y una fecha.
CREATE TABLE IF NOT EXISTS agendas_consultorios (
    id_agenda INT AUTO_INCREMENT PRIMARY KEY,
    id_medico INT NOT NULL,
    id_consultorio INT NOT NULL,
    fecha DATE NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fin TIME NOT NULL,
    duracion_minutos SMALLINT UNSIGNED NOT NULL,
    CONSTRAINT fk_agendas_consultorios_medico
        FOREIGN KEY (id_medico) REFERENCES medicos(id_usuario),
    CONSTRAINT fk_agendas_consultorios_consultorio
        FOREIGN KEY (id_consultorio) REFERENCES consultorios(id_consultorio),
    CONSTRAINT chk_agendas_consultorios_horas CHECK (
        hora_inicio >= '00:00:00' AND hora_fin < '24:00:00'
        AND hora_fin > hora_inicio
    ),
    CONSTRAINT chk_agendas_consultorios_duracion CHECK (
        duracion_minutos >= 15
        AND TIME_TO_SEC(hora_fin) - TIME_TO_SEC(hora_inicio) >= duracion_minutos * 60
    ),
    INDEX idx_agendas_medico_fecha (id_medico, fecha),
    INDEX idx_agendas_consultorio_fecha (id_consultorio, fecha)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Asignación del horario a los nuevos turnos. Los anteriores se conservan.
CREATE TABLE IF NOT EXISTS reservas_agenda_consultorios (
    id_turno INT NOT NULL PRIMARY KEY,
    id_agenda INT NOT NULL,
    hora TIME NOT NULL,
    es_sobreturno BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_reservas_agenda_turno
        FOREIGN KEY (id_turno) REFERENCES `turnosConsultoriosMédicos`(id_turno),
    CONSTRAINT fk_reservas_agenda_agenda
        FOREIGN KEY (id_agenda) REFERENCES agendas_consultorios(id_agenda),
    CONSTRAINT chk_reservas_agenda_hora CHECK (hora >= '00:00:00' AND hora < '24:00:00'),
    CONSTRAINT chk_reservas_agenda_sobreturno CHECK (es_sobreturno IN (0, 1)),
    CONSTRAINT uq_reservas_agenda_horario UNIQUE (id_agenda, hora, es_sobreturno)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
