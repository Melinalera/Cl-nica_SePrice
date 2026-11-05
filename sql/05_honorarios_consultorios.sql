USE clinica_seprice;

-- Atenciones efectivamente realizadas y el honorario correspondiente a cada una.
CREATE TABLE IF NOT EXISTS atenciones_consultorios (
    id_atencion INT AUTO_INCREMENT PRIMARY KEY,
    id_turno INT NOT NULL,
    id_medico INT NOT NULL,
    fecha_atencion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    practica VARCHAR(100) NOT NULL,
    importe DECIMAL(12, 2) NOT NULL,
    CONSTRAINT uq_atenciones_consultorios_turno UNIQUE (id_turno),
    CONSTRAINT chk_atenciones_consultorios_importe CHECK (importe >= 0),
    CONSTRAINT fk_atenciones_consultorios_turno
        FOREIGN KEY (id_turno) REFERENCES `turnosConsultoriosMédicos`(id_turno),
    CONSTRAINT fk_atenciones_consultorios_medico
        FOREIGN KEY (id_medico) REFERENCES medicos(id_usuario),
    INDEX idx_atenciones_medico_fecha (id_medico, fecha_atencion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Cabecera de cada liquidación confirmada para un profesional y un período.
CREATE TABLE IF NOT EXISTS liquidaciones_honorarios (
    id_liquidacion INT AUTO_INCREMENT PRIMARY KEY,
    id_medico INT NOT NULL,
    fecha_desde DATE NOT NULL,
    fecha_hasta DATE NOT NULL,
    fecha_emision DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    importe_total DECIMAL(14, 2) NOT NULL,
    CONSTRAINT chk_liquidaciones_honorarios_periodo CHECK (fecha_desde <= fecha_hasta),
    CONSTRAINT chk_liquidaciones_honorarios_importe CHECK (importe_total >= 0),
    CONSTRAINT fk_liquidaciones_honorarios_medico
        FOREIGN KEY (id_medico) REFERENCES medicos(id_usuario),
    INDEX idx_liquidaciones_medico_periodo (id_medico, fecha_desde, fecha_hasta)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Cada atención puede incluirse una sola vez en una liquidación.
CREATE TABLE IF NOT EXISTS detalle_liquidaciones_honorarios (
    id_liquidacion INT NOT NULL,
    id_atencion INT NOT NULL,
    importe DECIMAL(12, 2) NOT NULL,
    PRIMARY KEY (id_liquidacion, id_atencion),
    CONSTRAINT uq_detalle_honorarios_atencion UNIQUE (id_atencion),
    CONSTRAINT chk_detalle_honorarios_importe CHECK (importe >= 0),
    CONSTRAINT fk_detalle_honorarios_liquidacion
        FOREIGN KEY (id_liquidacion) REFERENCES liquidaciones_honorarios(id_liquidacion),
    CONSTRAINT fk_detalle_honorarios_atencion
        FOREIGN KEY (id_atencion) REFERENCES atenciones_consultorios(id_atencion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
