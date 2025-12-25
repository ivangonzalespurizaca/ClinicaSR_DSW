CREATE DATABASE BDSeg
GO

USE BDSeg
GO

CREATE TABLE Usuario (
    ID_Usuario BIGINT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Contrasenia VARCHAR(255) NOT NULL,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    DNI VARCHAR(8) NOT NULL UNIQUE,
    Telefono VARCHAR(15) NULL,
    Img_Perfil VARCHAR(200) NULL,
    Correo VARCHAR(100) NULL,
    Rol VARCHAR(20) NOT NULL
        CHECK (Rol IN ('ADMINISTRADOR', 'RECEPCIONISTA', 'CAJERO')),
    Estado VARCHAR(20) NOT NULL
        DEFAULT 'ACTIVO'
        CHECK (Estado IN ('ACTIVO', 'INACTIVO'))
)
GO

-------------------------------------------------------

INSERT INTO Usuario (Username, Contrasenia, Nombres, Apellidos, DNI, Telefono, Correo, Rol)
VALUES 
	('admin1', CONVERT(varchar(255), HASHBYTES('SHA2_512', '1234'), 2), 'Ricardo', 'Mendoza Arce', '12345678', '945123456', 'admin@hospital.com', 'ADMINISTRADOR'),
	('recep1', CONVERT(varchar(255), HASHBYTES('SHA2_512', '1234'), 2), 'Laura Maria', 'Garc a Torres', '87654321', '987654321', 'lgarcia@hospital.com', 'RECEPCIONISTA'),
	('caje1', CONVERT(varchar(255), HASHBYTES('SHA2_512', '1234'), 2), 'Marcos Fernando', 'Villanueva Ruiz', '44556677', '912345678', 'mvillanueva@hospital.com', 'CAJERO');
GO

--DROP DATABASE BDSeg
--GO

SELECT * FROM usuario
go

--alter database BDSeg set single_user with rollback immediate
--go