USE clinica_seprice;

-- Relacionar cada estudio de guardia o internación con los lotes consumidos.
CREATE TABLE IF NOT EXISTS consumos_estudios_guardia (
    id_solicitud INT NOT NULL,
    id_insumo INT NOT NULL,
    cantidad INT UNSIGNED NOT NULL,
    fecha_consumo DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_solicitud, id_insumo),
    CONSTRAINT chk_consumos_guardia_cantidad CHECK (cantidad > 0),
    CONSTRAINT fk_consumos_guardia_solicitud
        FOREIGN KEY (id_solicitud) REFERENCES solicitudes_estudios_guardia(id_solicitud),
    CONSTRAINT fk_consumos_guardia_insumo
        FOREIGN KEY (id_insumo) REFERENCES insumos(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
