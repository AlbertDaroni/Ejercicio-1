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
    fecha_anulacion DATETIME NOT NULL,
    importe DECIMAL(10,2) NOT NULL,
    estado VARCHAR(1) NOT NULL,
    id_reserva INT,
    id_usuario_creador INT,
    id_usuario_finalizador INT,

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