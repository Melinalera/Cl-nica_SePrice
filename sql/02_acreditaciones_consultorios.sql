USE clinica_seprice;

-- Una acreditación por turno de consultorios externos.
CREATE TABLE IF NOT EXISTS acreditaciones_consultorios (
    id_acreditacion INT AUTO_INCREMENT PRIMARY KEY,
    id_turno INT NOT NULL,
    dni_verificado VARCHAR(20) NOT NULL,
    modalidad_pago ENUM('particular', 'obra_social') NOT NULL,
    obra_social VARCHAR(100),
    numero_afiliado VARCHAR(50),
    fecha_acreditacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_acreditaciones_consultorios_turno UNIQUE (id_turno),
    CONSTRAINT fk_acreditaciones_consultorios_turno
        FOREIGN KEY (id_turno) REFERENCES `turnosConsultoriosMédicos`(id_turno)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
