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

CREATE OR ALTER PROCEDURE USP_Listar_Usuarios
AS
BEGIN
	SELECT * FROM Usuario 
END
GO

