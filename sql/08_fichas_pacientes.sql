USE clinica_seprice;

-- Ejecutar sobre una copia respaldada de la base.
-- Conservar las cuentas vinculadas y permitir pacientes sin cuenta de acceso.
-- La estructura original utiliza id_usuario como clave primaria de pacientes.
SET @seprice_fichas_creadas = (
    SELECT COUNT(*)
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name = 'pacientes'
      AND column_name = 'id_paciente'
);

SET @seprice_migracion = IF(
    @seprice_fichas_creadas = 0,
    'ALTER TABLE pacientes
        ADD COLUMN id_paciente INT NOT NULL AUTO_INCREMENT FIRST,
        DROP PRIMARY KEY,
        MODIFY COLUMN id_usuario INT NULL,
        MODIFY COLUMN número_de_afiliado VARCHAR(50) NULL,
        ADD COLUMN dni VARCHAR(20) NULL,
        ADD COLUMN nombre VARCHAR(50) NULL,
        ADD COLUMN apellido VARCHAR(50) NULL,
        ADD COLUMN email VARCHAR(100) NULL,
        ADD COLUMN telefono VARCHAR(20) NULL,
        ADD PRIMARY KEY (id_paciente),
        ADD CONSTRAINT uq_pacientes_usuario UNIQUE (id_usuario),
        ADD CONSTRAINT uq_pacientes_dni UNIQUE (dni)',
    'SELECT ''La tabla pacientes ya tiene la ficha independiente de la cuenta.'' AS informacion'
);

PREPARE seprice_actualizar_pacientes FROM @seprice_migracion;
EXECUTE seprice_actualizar_pacientes;
DEALLOCATE PREPARE seprice_actualizar_pacientes;

-- Recuperar los datos de las cuentas existentes sin sustituir datos ya cargados.
UPDATE pacientes p
JOIN usuarios u ON u.id_usuario = p.id_usuario
SET p.nombre = COALESCE(p.nombre, u.nombre),
    p.apellido = COALESCE(p.apellido, u.apellido),
    p.email = COALESCE(p.email, u.email),
    p.telefono = COALESCE(p.telefono, u.telefono);

-- Los DNI de pacientes anteriores se completan al verificar sus datos.
SELECT id_paciente, id_usuario, nombre, apellido, dni
FROM pacientes
WHERE dni IS NULL OR TRIM(dni) = '';
