CREATE DATABASE Inmobiliaria;
USE Inmobiliaria;

CREATE TABLE Propietarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(30) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Inquilinos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    DNI VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(30) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Tipo_Inmuebles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL,
    descripcion VARCHAR(100) NOT NULL
);

INSERT INTO Tipo_Inmuebles (nombre, descripcion) VALUES
('Departamento / Piso', 'Vivienda en edificio de 1 a 4 ambientes, tamaño de 30-120m2, antigüedad variable de estreno a antiguo.'),
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
('Edificio Completo', 'Estructura corporativa o residencial entera, múltiples ambientes, 1000+m2, antigüedad variable.');

CREATE TABLE Usuarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Contraseña VARCHAR(255) NOT NULL,
    Avatar VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL,
    Estado VARCHAR(1) NOT NULL
);

CREATE TABLE Inmuebles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    direccion VARCHAR(50) NOT NULL,
    cupo INT NOT NULL,
    latitud DECIMAL(10,8) NOT NULL,
    longitud DECIMAL(11,8) NOT NULL,
    precio_dia DECIMAL(10,2) NOT NULL,
    porcentaje_seña DECIMAL(10,2) NOT NULL,
    estado VARCHAR(1) NOT NULL,
    id_propietario INT,
    id_tipo INT,

    CONSTRAINT fk_inmueble_propietario
    FOREIGN KEY (id_propietario)
    REFERENCES Propietarios(id),

    CONSTRAINT fk_inmueble_tipo
    FOREIGN KEY (id_tipo)
    REFERENCES Tipo_Inmuebles(id)
);

CREATE TABLE Imagen_Inmuebles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    url VARCHAR(255) NOT NULL,
    esPortada BOOLEAN NOT NULL,
    orden INT NOT NULL,
    id_inmueble INT,

    CONSTRAINT fk_imagen_inmuebles_inmueble
    FOREIGN KEY (id_inmueble)
    REFERENCES Inmuebles(id)
);

CREATE TABLE Reservas (
    id INT PRIMARY KEY AUTO_INCREMENT,
    fecha_creacion DATE NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin_original DATE NOT NULL,
    fecha_fin_efectiva DATE NOT NULL,
    monto_dia DECIMAL(10,2) NOT NULL,
    multa DECIMAL(10,2) NOT NULL,
    estado VARCHAR(1) NOT NULL,
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

CREATE TABLE Pagos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    concepto VARCHAR(50) NOT NULL,
    fecha_pago DATE NOT NULL,
    fecha_anulacion DATETIME NULL,
    importe DECIMAL(10,2) NOT NULL,
    estado VARCHAR(1) NOT NULL,
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