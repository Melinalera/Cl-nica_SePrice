USE clinica_seprice;

-- Cada registro corresponde a la recepción de un lote comprado.
-- Un comprobante puede incluir varios lotes, cada uno con su movimiento de entrada.
CREATE TABLE IF NOT EXISTS compras_insumos (
    id_compra INT AUTO_INCREMENT PRIMARY KEY,
    id_movimiento INT NOT NULL,
    proveedor VARCHAR(100) NOT NULL,
    comprobante VARCHAR(100) NOT NULL,
    fecha_recepcion DATE NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_compras_insumos_movimiento UNIQUE (id_movimiento),
    CONSTRAINT fk_compras_insumos_movimiento
        FOREIGN KEY (id_movimiento) REFERENCES movimientos_insumos(id),
    INDEX idx_compras_insumos_comprobante (proveedor, comprobante)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
