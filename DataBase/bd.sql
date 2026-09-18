CREATE DATABASE Inmobiliaria;
USE Inmobiliaria;

-- Creación de la tabla Propietarios

CREATE TABLE Propietarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(30) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE
);

-- Inserciones para Propietarios

INSERT INTO Propietarios (Nombre, Apellido, DNI, Telefono, Correo) 
VALUES  ('Carlos', 'Mendoza', '12345678A', '+34 600 111 222', 'carlos.mendoza@email.com'),
        ('María', 'Rodríguez', '23456789B', '+34 600 333 444', 'maria.rodriguez@email.com'),
        ('Juan', 'García', '34567890C', '+34 600 555 666', 'juan.garcia@email.com'),
        ('Ana', 'Martínez', '45678901D', '+34 600 777 888', 'ana.martinez@email.com'),
        ('Luis', 'Sánchez', '56789012E', '+34 600 999 000', 'luis.sanchez@email.com'),
        ('Laura', 'López', '67890123F', '+34 611 222 333', 'laura.lopez@email.com'),
        ('Jorge', 'Gómez', '78901234G', '+34 622 444 555', 'jorge.gomez@email.com');

-- Creación de la tabla Inquilinos

CREATE TABLE Inquilinos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(30) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE
);

-- Inserciones para Inquilinos

INSERT INTO Inquilinos (Nombre, Apellido, DNI, Telefono, Correo)
VALUES  ('Diego', 'Fernández', '89012345H', '+34 633 111 222', 'diego.fernandez@email.com'),
        ('Sofía', 'Benítez', '90123456I', '+34 644 333 444', 'sofia.benitez@email.com'),
        ('Andrés', 'Castro', '01234567J', '+34 655 555 666', 'andres.castro@email.com'),
        ('Lucía', 'Morales', '11223344K', '+34 666 777 888', 'lucia.morales@email.com'),
        ('Santiago', 'Ortiz', '22334455L', '+34 677 999 000', 'santiago.ortiz@email.com'),
        ('Elena', 'Ruiz', '33445566M', '+34 688 222 333', 'elena.ruiz@email.com'),
        ('Marcos', 'Navarro', '44556677N', '+34 699 444 555', 'marcos.navarro@email.com');

-- Creación de la tabla Tipo_Inmuebles

CREATE TABLE Tipo_Inmuebles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL,
    descripcion VARCHAR(100) NOT NULL
);

-- Inserciones para Tipos de Inmuebles

INSERT INTO Tipo_Inmuebles (nombre, descripcion)
VALUES  ('Departamento / Piso', 'Vivienda en edificio de 1 a 4 ambientes, tamaño de 30-120m2, antigüedad variable de estreno a antiguo.'),
        ('Casa Residencial', 'Propiedad unifamiliar de 2+ ambientes con patio, tamaño de 100-300m2, antigüedad de 0 a 50 años.'),
        ('Monoambiente / Estudio', 'Espacio único integrado con baño, tamaño compacto de 20-40m2, ideal para solteros o estreno.'),
        ('Oficina Comercial', 'Espacio laboral diáfano o privado, tamaño de 40-500m2, infraestructura moderna o remodelada.'),
        ('Local Comercial', 'Ubicado a pie de calle con salón y depósito, tamaño de 50-200m2, alta circulación de personas.'),
        ('Terreno / Lote Urbano', 'Lote sin edificar listo para construir, tamaño de 150-1000m2, sin antigüedad (tierra virgen).'),
        ('Casa de Campo / Quinta', 'Propiedad de descanso con parque y pileta, tamaño de 500+m2, ambientes amplios y estilo rústico.'),
        ('Ph (Propiedad Horizontal)', 'Vivienda tipo casa sin expensas en complejo, 2 a 4 ambientes, 50-100m2, antigüedad de 20+ años.'),
        ('Depósito / Galpón', 'Espacio industrial techado con techos altos, tamaño de 200-2000m2, construcción fuerte y antigua.'),
        ('Cochera / Garaje', 'Espacio exclusivo para estacionar un vehículo, tamaño de 12-15m2, sin ambientes internos.'),
        ('Penthouse', 'Departamento de lujo en último piso con terraza, 4+ ambientes, 150+m2, acabados modernos.'),
        ('Duplex / Triplex', 'Vivienda distribuida en dos o tres plantas, 3 a 5 ambientes, 80-150m2, diseño moderno.'),
        ('Consultorio Médico', 'Espacio adaptado para salud con sala de espera, 2 ambientes, 30-60m2, infraestructura sanitaria.'),
        ('Edificio Completo', 'Estructura corporativa o residencial entera, múltiples ambientes, 1000+m2, antigüedad variable.'),
        ('Apartamento', 'Vivienda independiente en un edificio de varias plantas.'),
        ('Estudio', 'Espacio monoambiente que integra sala, dormitorio y cocina.'),
        ('Chalet', 'Casa de campo o residencial grande, habitualmente con jardín y piscina.');

-- Creación de la tabla Usuarios

CREATE TABLE Usuarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Contraseña VARCHAR(255) NOT NULL,
    Avatar VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL,
    Estado VARCHAR(1) NOT NULL DEFAULT '1'
);

-- Inserciones para Usuarios

INSERT INTO Usuarios (Nombre, Apellido, Correo, Contraseña, Avatar, Rol, Estado)
VALUES  ('Alejandro', 'Silva', 'ale.silva@sistema.com', '$2y$10$e0myL5u..vsnDGL.61iYGe1V6.2Lp3P76pA7h4fR5G2vB5fU3u', 'avatar_ale.png', 'Administrador', '1'),
        ('Gabriela', 'Pérez', 'gaby.perez@sistema.com', '$2y$10$r9xM3a9JkLZ7qP1oY8z2e.V9wX4mB7tK3vL5nP8qR1sT3uV5w', 'avatar_gaby.png', 'Recepcionista', '1'),
        ('Ricardo', 'Torres', 'ricardo.torres@sistema.com', '$2y$10$w4mB7tK3vL5nP8qR1sT3uV5wX4mB7tK3vL5nP8qR1sT3uV5w', 'avatar_ricardo.png', 'Agente Inmobiliario', '1'),
        ('Valeria', 'Ríos', 'valeria.rios@sistema.com', '$2y$10$z2e.V9wX4mB7tK3vL5nP8qR1sT3uV5wX4mB7tK3vL5nP8qR1sT', 'avatar_valeria.png', 'Recepcionista', '1'),
        ('Manuel', 'Vargas', 'manuel.vargas@sistema.com', '$2y$10$P8qR1sT3uV5wX4mB7tK3vL5nP8qR1sT3uV5wX4mB7tK3vL5nP', 'avatar_manuel.png', 'Soporte', '1'),
        ('Beatriz', 'Luna', 'beatriz.luna@sistema.com', '$2y$10$T3uV5wX4mB7tK3vL5nP8qR1sT3uV5wX4mB7tK3vL5nP8qR1s', 'avatar_beatriz.png', 'Supervisor', '0');

-- Usuarios iniciales para autenticación y roles

INSERT INTO Usuarios (Nombre, Apellido, Correo, Contraseña, Avatar, Rol, Estado) VALUES
('Admin', 'Sistema', 'admin@inmobiliaria.com', '1234', 'avatar.png', 'Administrador', '1'),
('Empleado', 'Prueba', 'empleado@inmobiliaria.com', '1234', 'avatar.png', 'Empleado', '1');

-- Creación de la tabla Inmuebles

CREATE TABLE Inmuebles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    direccion VARCHAR(50) NOT NULL,
    cupo INT NOT NULL,
    latitud DECIMAL(10,8) NOT NULL,
    longitud DECIMAL(11,8) NOT NULL,
    precio_dia DECIMAL(10,2) NOT NULL,
    porcentaje_seña DECIMAL(10,2) NOT NULL DEFAULT '0',
    estado VARCHAR(1) NOT NULL DEFAULT '1',
    id_propietario INT,
    id_tipo INT,

    CONSTRAINT fk_inmueble_propietario
    FOREIGN KEY (id_propietario)
    REFERENCES Propietarios(id),

    CONSTRAINT fk_inmueble_tipo
    FOREIGN KEY (id_tipo)
    REFERENCES Tipo_Inmuebles(id)
);

-- Inserciones para Inmuebles

INSERT INTO Inmuebles (direccion, cupo, latitud, longitud, precio_dia, porcentaje_seña, estado, id_propietario, id_tipo)
VALUES  ('Av. de la Constitución 12', 4, 40.41677500, -3.70379000, 85.00, 20.00, '1', 1, 1),
        ('Calle Mayor 45', 6, 40.41536300, -3.70739800, 150.00, 15.00, '1', 2, 2),
        ('Paseo de la Castellana 100', 2, 40.44111200, -3.69166700, 60.00, 10.00, '1', 3, 3),
        ('Gran Vía 28', 20, 40.42011900, -3.70158400, 450.00, 30.00, '1', 4, 4),
        ('Calle de Alcalá 85', 10, 40.42000000, -3.68800000, 250.00, 25.00, '1', 5, 5),
        ('Camino del Río 7', 8, 40.45000000, -3.75000000, 320.00, 20.00, '1', 6, 6),
        ('Calle Atocha 14', 3, 40.41200000, -3.70200000, 75.00, 0.00, '0', 7, 1);

-- Creación de la tabla Imagen_Inmuebles

CREATE TABLE Imagen_Inmuebles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    url VARCHAR(255) NOT NULL,
    id_inmueble INT,

    CONSTRAINT fk_imagen_inmuebles_inmueble
    FOREIGN KEY (id_inmueble)
    REFERENCES Inmuebles(id)
);

-- Inserciones para Imagen_Inmuebles

INSERT INTO Imagen_Inmuebles (url, id_inmueble)
VALUES  ('https://unsplash.com', 1),
        ('https://apartmenttherapy.com', 1),
        ('https://istockphoto.com', 2),
        ('https://backsplash.com', 3),
        ('https://istockphoto.com', 4),
        ('https://adobe.com', 5),
        ('https://oxfordski.com', 6),
        ('https://vecteezy.com', 7);

-- Creación de la tabla Reservas

CREATE TABLE Reservas (
    id INT PRIMARY KEY AUTO_INCREMENT,
    fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_inicio DATETIME NOT NULL,
    fecha_fin_original DATETIME NOT NULL,
    fecha_fin_efectiva DATETIME NOT NULL,
    monto_dia DECIMAL(10,2) NOT NULL,
    multa DECIMAL(10,2) NOT NULL DEFAULT '0',
    estado VARCHAR(1) NOT NULL DEFAULT '1',
    id_inquilino INT,
    id_inmueble INT,
    id_usuario_creador INT,
    id_usuario_finalizador INT,

    CONSTRAINT fk_reserva_inquilinos
    FOREIGN KEY (id_inquilino)
    REFERENCES Inquilinos(id),

    CONSTRAINT fk_reserva_inmuebles
    FOREIGN KEY (id_inmueble)
    REFERENCES Inmuebles(id)
    ON UPDATE CASCADE ON DELETE CASCADE,

    CONSTRAINT fk_reserva_usuarios_creadores
    FOREIGN KEY (id_usuario_creador)
    REFERENCES Usuarios(id),

    CONSTRAINT fk_reserva_usuarios_finalizadores
    FOREIGN KEY (id_usuario_finalizador)
    REFERENCES Usuarios(id)
);

-- Inserciones para Reservas

INSERT INTO Reservas (fecha_inicio, fecha_fin_original, fecha_fin_efectiva, monto_dia, multa, estado, id_inquilino, id_inmueble, id_usuario_creador, id_usuario_finalizador)
VALUES  ('2026-10-01 14:00:00', '2026-10-05 10:00:00', '2026-10-05 09:30:00', 85.00, 0.00, '1', 1, 1, 1, 1),
        ('2026-11-15 15:00:00', '2026-11-20 11:00:00', '2026-11-20 11:00:00', 150.00, 0.00, '1', 2, 2, 1, NULL),
        ('2026-09-01 12:00:00', '2026-09-05 12:00:00', '2026-09-06 17:00:00', 60.00, 45.50, '1', 3, 3, 2, 3),
        ('2026-08-10 16:00:00', '2026-08-20 12:00:00', '2026-08-15 10:00:00', 450.00, 0.00, '0', 4, 4, 3, 2),
        ('2026-12-04 14:00:00', '2026-12-06 12:00:00', '2026-12-06 11:45:00', 85.00, 0.00, '1', 1, 1, 1, 1),
        ('2026-10-20 14:00:00', '2026-10-25 11:00:00', '2026-10-25 11:00:00', 60.00, 0.00, '1', 3, 3, 2, NULL);

-- Creación de la tabla Pagos

CREATE TABLE Pagos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    concepto VARCHAR(50) NOT NULL,
    fecha_pago DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_anulacion DATETIME NULL,
    importe DECIMAL(10,2) NOT NULL,
    estado VARCHAR(1) NOT NULL DEFAULT '1',
    id_reserva INT,
    id_usuario_creador INT,
    id_usuario_finalizador INT NULL,

    CONSTRAINT fk_pago_reserva
    FOREIGN KEY (id_reserva)
    REFERENCES Reservas(id),

    CONSTRAINT fk_pago_usuario_creador
    FOREIGN KEY (id_usuario_creador)
    REFERENCES Usuarios(id),

    CONSTRAINT fk_pago_usuario_finalizador
    FOREIGN KEY (id_usuario_finalizador)
    REFERENCES Usuarios(id)
);

-- Inserciones para Pagos

INSERT INTO Pagos (concepto, fecha_pago, fecha_anulacion, importe, estado, id_reserva, id_usuario_creador, id_usuario_finalizador)
VALUES  ('Pago total de reserva - Tarjeta', '2026-10-01 14:15:00', NULL, 340.00, '1', 1, 1, 1),
        ('Seña de reserva 15% - Transferencia', '2026-09-17 10:00:00', NULL, 112.50, '1', 2, 1, NULL),
        ('Abono inicial de estadía', '2026-09-01 12:30:00', NULL, 240.00, '1', 3, 2, 2),
        ('Cobro de multa por entrega tardía', '2026-09-06 17:15:00', NULL, 45.50, '1', 3, 3, 3),
        ('Cobro duplicado - Efectivo', '2026-12-04 14:05:00', '2026-12-04 14:20:00', 170.00, '0', 5, 1, 1),
        ('Pago de estadía - Tarjeta de Débito', '2026-12-04 14:22:00', NULL, 170.00, '1', 5, 1, 1);