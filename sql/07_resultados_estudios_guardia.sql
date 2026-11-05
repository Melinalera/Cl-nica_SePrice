USE clinica_seprice;

-- Un resultado por solicitud de estudio de guardia o internación.
CREATE TABLE IF NOT EXISTS resultados_estudios_guardia (
    id_solicitud INT NOT NULL PRIMARY KEY,
    id_medico INT NOT NULL,
    resultado TEXT NOT NULL,
    fecha_resultado DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_entrega DATETIME NULL,
    receptor VARCHAR(100) NULL,
    CONSTRAINT fk_resultados_guardia_solicitud
        FOREIGN KEY (id_solicitud) REFERENCES solicitudes_estudios_guardia(id_solicitud),
    CONSTRAINT fk_resultados_guardia_medico
        FOREIGN KEY (id_medico) REFERENCES medicos(id_usuario),
    CONSTRAINT chk_resultados_guardia_entrega CHECK (
        (fecha_entrega IS NULL AND receptor IS NULL)
        OR (fecha_entrega IS NOT NULL AND receptor IS NOT NULL)
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
