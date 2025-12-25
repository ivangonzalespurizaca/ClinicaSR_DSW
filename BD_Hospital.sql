CREATE DATABASE BDHospital
GO

USE BDHospital
GO

CREATE TABLE Especialidad (
    ID_Especialidad BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE
)
GO

CREATE TABLE Medico (
    ID_Medico BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    DNI VARCHAR(8) NOT NULL UNIQUE,
    Nro_Colegiatura VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(15),
    ID_Especialidad BIGINT NOT NULL,
    FOREIGN KEY (ID_Especialidad) REFERENCES Especialidad(ID_Especialidad)
)
GO

CREATE TABLE Paciente (
    ID_Paciente BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(50) NOT NULL,
    DNI VARCHAR(8) NOT NULL UNIQUE,
    Fecha_Nacimiento DATE,
    Telefono VARCHAR(15)
)
GO

CREATE TABLE Cita (
    ID_Cita BIGINT IDENTITY(1,1) PRIMARY KEY,
    ID_Medico BIGINT NOT NULL,
    ID_Paciente BIGINT NOT NULL,
    ID_Usuario BIGINT NOT NULL,
    Fecha_Cita DATE NOT NULL,
	Hora_Cita TIME NOT NULL,
    Motivo VARCHAR(255),
    Estado VARCHAR(20) NOT NULL 
		DEFAULT 'PENDIENTE'
		CHECK (Estado IN ('PENDIENTE', 'CONFIRMADO', 'CANCELADO', 'VENCIDO')),
    FOREIGN KEY (ID_MEDICO) REFERENCES Medico(ID_MEDICO),
    FOREIGN KEY (ID_Paciente) REFERENCES Paciente(ID_PACIENTE)
)
GO

CREATE TABLE Horarios_Atencion (
    ID_Horario BIGINT IDENTITY(1,1) PRIMARY KEY,
    ID_Medico BIGINT NOT NULL,
    Dia_Semana VARCHAR(9) CHECK (Dia_Semana IN ('DOMINGO', 'LUNES', 'MARTES', 'MIERCOLES', 'JUEVES', 'VIERNES', 'SABADO')) NOT NULL,
    Horario_Entrada TIME NOT NULL,
    Horario_Salida TIME NOT NULL,
    FOREIGN KEY (ID_MEDICO) REFERENCES Medico(ID_Medico),
    CONSTRAINT UC_Horario UNIQUE (ID_Medico, Dia_Semana, Horario_Entrada, Horario_Salida)
)
GO

CREATE TABLE Comprobante_Pago (
    ID_Comprobante BIGINT IDENTITY(1,1) PRIMARY KEY,
    ID_Cita BIGINT NOT NULL UNIQUE,
	ID_Usuario BIGINT NOT NULL,
    Nombre_Pagador VARCHAR(100) NOT NULL,
    Apellidos_Pagador VARCHAR(100) NOT NULL,
    DNI_Pagador VARCHAR(8),
    Contacto_Pagador VARCHAR(15),
    Fecha_Emision DATETIME NOT NULL DEFAULT GETDATE(),
    Monto DECIMAL(10,2) NOT NULL,
    Metodo_Pago VARCHAR(20) CHECK (Metodo_Pago IN ('EFECTIVO', 'TARJETA', 'TRANSFERENCIA')) NOT NULL,
    Estado VARCHAR(20) NOT NULL
		DEFAULT 'EMITIDO'
		CHECK (Estado IN ('EMITIDO', 'ANULADO'))
    FOREIGN KEY (ID_Cita) REFERENCES Cita(ID_Cita)
)
GO

-------------------------------------------------------

INSERT INTO Especialidad (Nombre)
VALUES 
	('Medicina General'),
	('Pediatr a'),
	('Cardiolog a'),
	('Dermatolog a')
GO

INSERT INTO Medico (Nombres, Apellidos, DNI, Nro_Colegiatura, Telefono, ID_Especialidad)
VALUES 
	('Juan', 'Perez', '12345678', '1234-5678', '987654321', 1),  
	('Ana', 'Martinez', '23456789', '2345-6789', '987654322', 2),  
	('Carlos', 'Gomez', '34567890', '3456-7890', '987654323', 3)
GO

INSERT INTO Paciente (Nombres, Apellidos, DNI, Fecha_Nacimiento, Telefono)
VALUES 
	('Luis', 'Ram rez', '45678901', '1990-05-10', '999999999'),
	('Mar a', 'S nchez', '56789012', '1985-08-15', '988888888'),
	('Pedro', 'L pez', '67890123', '2000-02-20', '977777777')
GO

INSERT INTO Horarios_Atencion (ID_MEDICO, Dia_Semana, Horario_Entrada, Horario_Salida)
VALUES 
	(1, 'LUNES', '09:00:00', '13:00:00'),
	(1, 'MIERCOLES', '09:00:00', '13:00:00'),
	(1, 'VIERNES', '09:00:00', '13:00:00'),
	(2, 'MARTES', '08:00:00', '12:00:00'),
	(2, 'JUEVES', '08:00:00', '12:00:00'),
	(3, 'LUNES', '14:00:00', '18:00:00'),
	(3, 'MIERCOLES', '14:00:00', '18:00:00')
GO

INSERT INTO Cita (ID_Medico, ID_Paciente, ID_Usuario, Fecha_Cita, Hora_Cita, Motivo, Estado)
VALUES 
	(1, 1, 2, '2025-12-29', '10:00:00', 'Consulta general', 'CONFIRMADO'),
	(2, 2, 2, '2025-12-30', '09:00:00', 'Chequeo pedi trico', 'CONFIRMADO'),
	(3, 3, 2, '2025-12-31', '15:00:00', 'Consulta cardiol gica', 'CONFIRMADO')
GO

INSERT INTO Comprobante_Pago (ID_Cita, ID_Usuario, Nombre_Pagador, Apellidos_Pagador, DNI_Pagador, Contacto_Pagador, Monto, Metodo_Pago, Estado)
VALUES 
	(1, 3, 'Carlos', 'Ram rez', '45678901', '999999999', 100.00, 'EFECTIVO', 'EMITIDO'),
	(2, 3, 'Luisa', 'S nchez', '56789012', '988888888', 120.00, 'TARJETA', 'EMITIDO'),
	(3, 3, 'Mar a', 'L pez', '67890123', '977777777', 150.00, 'TRANSFERENCIA', 'EMITIDO')
GO

DROP DATABASE BDHospital
GO

ALTER DATABASE BDHospital set single_user with rollback immediate
go

USE MASTER
GO

