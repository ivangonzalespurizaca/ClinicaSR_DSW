USE BDSeg
GO

CREATE OR ALTER PROCEDURE USP_Usuario_LOGIN
    @Username VARCHAR(50),
    @Contrasenia VARCHAR(255) 
AS
BEGIN
    DECLARE @PasswordHash VARCHAR(255);
    
    -- IMPORTANTE: Convertimos @Contrasenia a VARCHAR antes del HASHBYTES
    SET @PasswordHash = CONVERT(VARCHAR(255), HASHBYTES('SHA2_512', @Contrasenia), 2);

    SELECT 
        ID_Usuario, Username, Nombres, Apellidos, Rol, Estado, Img_Perfil, Correo
    FROM Usuario 
    WHERE Username = @Username 
      AND UPPER(Contrasenia) = UPPER(@PasswordHash)
      AND Estado = 'ACTIVO';
END
GO

CREATE OR ALTER PROCEDURE USP_Usuario_Registrar
    @Username VARCHAR(50),
    @Contrasenia VARCHAR(255),
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @DNI VARCHAR(8),
    @Telefono VARCHAR(15),
    @Img_Perfil VARCHAR(200),
    @Correo VARCHAR(100),
    @Rol VARCHAR(20)
AS
BEGIN
    INSERT INTO Usuario (
        Username, Contrasenia, Nombres, Apellidos, DNI, 
        Telefono, Img_Perfil, Correo, Rol, Estado
    )
    VALUES (
        @Username, CONVERT(VARCHAR(255), HASHBYTES('SHA2_512', @Contrasenia), 2),
        @Nombres, @Apellidos, @DNI, @Telefono, @Img_Perfil, @Correo, @Rol, 'ACTIVO'
    );
END
GO

-- USP_Usuario_Actualizar
CREATE OR ALTER PROCEDURE USP_Usuario_Actualizar
    @ID_Usuario BIGINT,
    @Username VARCHAR(50),
    @Contrasenia VARCHAR(255) = NULL, -- Recibe texto plano desde C#
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @DNI VARCHAR(8),
    @Telefono VARCHAR(15),
    @Correo VARCHAR(100),
    @Img_Perfil VARCHAR(200)
AS
BEGIN
    UPDATE Usuario
    SET Username = @Username,
        -- Si llega una clave, la hasheamos. Si no, mantenemos la actual.
        Contrasenia = ISNULL(CONVERT(VARCHAR(255), HASHBYTES('SHA2_512', @Contrasenia), 2), Contrasenia),
        Nombres = @Nombres,
        Apellidos = @Apellidos,
        DNI = @DNI,
        Telefono = @Telefono,
        Correo = @Correo,
        Img_Perfil = @Img_Perfil
    WHERE ID_Usuario = @ID_Usuario;
END
GO

CREATE OR ALTER PROCEDURE USP_Usuario_ObtenerPorId
    @ID_Usuario BIGINT
AS
BEGIN
    SELECT 
        ID_Usuario, Username, Contrasenia, Nombres, Apellidos, 
        DNI, Telefono, Img_Perfil, Correo, Rol, Estado
    FROM Usuario
    WHERE ID_Usuario = @ID_Usuario;
END
GO

EXEC USP_Usuario_ObtenerPorId '1'
GO

CREATE OR ALTER PROCEDURE USP_Usuario_ObtenerPorUsername
    @Username VARCHAR(50)
AS
BEGIN
    SELECT 
        ID_Usuario, Username, Contrasenia, Nombres, Apellidos, 
        DNI, Telefono, Img_Perfil, Correo, Rol, Estado
    FROM Usuario
    WHERE Username = @Username;
END
GO

EXEC USP_Usuario_ObtenerPorUsername 'admin1'
GO