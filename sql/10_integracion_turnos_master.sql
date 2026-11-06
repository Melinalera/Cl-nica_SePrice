USE clinica_seprice;

-- Incorporar la hora conservando los turnos anteriores sin horario conocido.
SET @seprice_hora_existe = (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name = 'turnosConsultoriosMédicos' AND column_name = 'hora'
);
SET @seprice_actualizar_hora = IF(@seprice_hora_existe = 0,
    'ALTER TABLE `turnosConsultoriosMédicos` ADD COLUMN hora TIME NULL AFTER fecha',
    'SELECT ''La tabla de turnos ya tiene la columna hora.'' AS informacion');
PREPARE seprice_turnos_hora FROM @seprice_actualizar_hora;
EXECUTE seprice_turnos_hora;
DEALLOCATE PREPARE seprice_turnos_hora;

-- Agregar los nuevos profesionales de ejemplo sin cambiar IDs ni perfiles existentes.
INSERT INTO usuarios (nombre, apellido, usuario, contraseña, rol, email, telefono)
SELECT nuevos.nombre, nuevos.apellido, nuevos.usuario, '1234', 'medico',
       nuevos.email, nuevos.telefono
FROM (
    SELECT 'Cosme' AS nombre, 'Fulanito' AS apellido, 'cosme.f' AS usuario,
           'cosme.fulanito@clinica.com' AS email, '11-5578-1234' AS telefono
    UNION ALL
    SELECT 'Lucas', 'Ledezma', 'lucas.l', 'lucas.ledezma@clinica.com', '11-3455-1234'
    UNION ALL
    SELECT 'Uma', 'Insaurralde', 'uma.i', 'uma.insaurralde@clinica.com', '11-6586-5678'
) nuevos
WHERE NOT EXISTS (SELECT 1 FROM usuarios u WHERE u.usuario = nuevos.usuario);

INSERT INTO medicos (id_usuario, matricula, especialidad)
SELECT u.id_usuario, nuevos.matricula, nuevos.especialidad
FROM usuarios u
JOIN (
    SELECT 'milagros.b' AS usuario, 45123 AS matricula, 'Cardiología' AS especialidad
    UNION ALL SELECT 'cosme.f', 48510, 'Dermatología'
    UNION ALL SELECT 'lucas.l', 76910, 'Gastroenterología'
    UNION ALL SELECT 'ariel.a', 75910, 'Neumonología'
    UNION ALL SELECT 'uma.i', 26210, 'Neurología'
) nuevos ON nuevos.usuario = u.usuario
WHERE u.rol = 'medico'
  AND NOT EXISTS (SELECT 1 FROM medicos m WHERE m.id_usuario = u.id_usuario);
