USE clinica_seprice;

-- 1. Limpieza previa de tablas en orden correcto
DROP TABLE IF EXISTS administrativos;
DROP TABLE IF EXISTS pacientes;
DROP TABLE IF EXISTS medicos;
DROP TABLE IF EXISTS usuarios;

-- Tabla central de usuarios con nombre, apellido, email y teléfono
CREATE TABLE usuarios (
    id_usuario INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    usuario VARCHAR(50) NOT NULL,
    contraseña VARCHAR(100) NOT NULL,
    rol VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    PRIMARY KEY (id_usuario)
);

-- Inserción de datos con nombre y apellido separados
INSERT INTO usuarios (nombre, apellido, usuario, contraseña, rol, email, telefono) 
VALUES ('Melina', 'Lera', 'melina.l', '1234', 'paciente', 'melina.lera@email.com', '11-4567-8901');

INSERT INTO usuarios (nombre, apellido, usuario, contraseña, rol, email, telefono) 
VALUES ('Milagros', 'Bianco', 'milagros.b', '1234', 'medico', 'milagros.bianco@clinica.com', '11-5555-1234');

INSERT INTO usuarios (nombre, apellido, usuario, contraseña, rol, email, telefono) 
VALUES ('Ariel', 'Alfonzo', 'ariel.a', '1234', 'medico', 'ariel.alfonzo@clinica.com', '11-6666-5678');

INSERT INTO usuarios (nombre, apellido, usuario, contraseña, rol, email, telefono) 
VALUES ('Juan', 'Cáceres', 'juan.c', '1234', 'administrativo', 'juan.caceres@clinica.com', '11-7777-9876');

-- 2. Tabla de Médicos
CREATE TABLE medicos (
    id_usuario INT NOT NULL,
    matricula INT NOT NULL,
    especialidad VARCHAR(50),
    PRIMARY KEY (id_usuario),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

INSERT INTO medicos (id_usuario, matricula, especialidad) VALUES (2, 45123, 'Cardiología');
INSERT INTO medicos (id_usuario, matricula, especialidad) VALUES (3, 48910, 'Pediatría');

-- 3. Tabla de Pacientes
CREATE TABLE pacientes (
    id_usuario INT NOT NULL,
    número_de_afiliado INT NOT NULL,
    PRIMARY KEY (id_usuario),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

INSERT INTO pacientes (id_usuario, número_de_afiliado) VALUES (1, 998822);

-- 4. Tabla de Administrativos
CREATE TABLE administrativos (
    id_usuario INT NOT NULL,
    sector VARCHAR(50) NOT NULL,
    PRIMARY KEY (id_usuario),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

INSERT INTO administrativos (id_usuario, sector) VALUES (4, 'Mesa de Entradas y Turnos');

-- 5. Consultar las tablas para verificar
SELECT * FROM usuarios;
SELECT * FROM medicos;
SELECT * FROM pacientes;
SELECT * FROM administrativos;

CREATE TABLE IF NOT EXISTS insumos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    stock_actual INT NOT NULL,
    stock_minimo INT NOT NULL,
    numero_lote VARCHAR(50) NOT NULL,
    fecha_vencimiento DATE NOT NULL
);