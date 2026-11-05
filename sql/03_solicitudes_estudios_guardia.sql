USE clinica_seprice;

-- Solicitudes de estudios sin turno previo provenientes de guardia o internación.
CREATE TABLE IF NOT EXISTS solicitudes_estudios_guardia (
    id_solicitud INT AUTO_INCREMENT PRIMARY KEY,
    dni_paciente VARCHAR(20) NOT NULL,
    nombre_paciente VARCHAR(100) NOT NULL,
    estudio_solicitado VARCHAR(100) NOT NULL,
    origen ENUM('Guardia', 'Internación') NOT NULL,
    prioridad ENUM('Normal', 'Urgente') NOT NULL,
    referencia_orden VARCHAR(100) NOT NULL,
    orden_verificada BOOLEAN NOT NULL,
    estado ENUM('Pendiente', 'En realización', 'Realizado', 'Cancelado')
        NOT NULL DEFAULT 'Pendiente',
    fecha_solicitud DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_solicitudes_guardia_dni (dni_paciente),
    INDEX idx_solicitudes_guardia_estado_prioridad (estado, prioridad)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
