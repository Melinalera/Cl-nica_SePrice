USE clinica_seprice;

-- Historial de entradas y salidas por lote de insumo.
CREATE TABLE IF NOT EXISTS movimientos_insumos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_insumo INT NOT NULL,
    tipo ENUM('entrada', 'salida') NOT NULL,
    cantidad INT UNSIGNED NOT NULL,
    fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    motivo VARCHAR(150) NOT NULL,
    CONSTRAINT chk_movimientos_insumos_cantidad CHECK (cantidad > 0),
    CONSTRAINT fk_movimientos_insumos_insumo
        FOREIGN KEY (id_insumo) REFERENCES insumos(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
