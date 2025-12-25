USE BDHospital
GO

CREATE OR ALTER PROCEDURE USP_Medico_Buscar
    @Filtro VARCHAR(100)
AS
BEGIN
    SELECT 
        M.ID_Medico, 
        M.Nombres, 
        M.Apellidos, 
        M.DNI,
        E.Nombre AS NombreEspecialidad
    FROM Medico M
    INNER JOIN Especialidad E ON M.ID_Especialidad = E.ID_Especialidad
    WHERE M.Nombres LIKE '%' + @Filtro + '%' 
       OR M.Apellidos LIKE '%' + @Filtro + '%'
       OR M.DNI LIKE '%' + @Filtro + '%'
END
GO

CREATE OR ALTER PROCEDURE USP_Medico_ObtenerPorId
    @ID_Medico BIGINT
AS
BEGIN
    SELECT 
        M.ID_Medico, 
        M.Nombres, 
        M.Apellidos, 
        M.DNI,
        M.Nro_Colegiatura,
        M.Telefono,
        E.ID_Especialidad,
        E.Nombre AS NombreEspecialidad
    FROM Medico M
    INNER JOIN Especialidad E ON M.ID_Especialidad = E.ID_Especialidad
    WHERE M.ID_Medico = @ID_Medico
END
GO

CREATE OR ALTER PROCEDURE USP_Horarios_ListarPorMedico
    @ID_Medico BIGINT
AS
BEGIN
    -- Obtenemos los horarios registrados para el médico
    SELECT 
        ID_Horario,
        Dia_Semana,
		Horario_Entrada,
        Horario_Salida
    FROM Horarios_Atencion
    WHERE ID_Medico = @ID_Medico
    ORDER BY CASE Dia_Semana 
        WHEN 'LUNES' THEN 1 WHEN 'MARTES' THEN 2 WHEN 'MIERCOLES' THEN 3 
        WHEN 'JUEVES' THEN 4 WHEN 'VIERNES' THEN 5 WHEN 'SABADO' THEN 6 
        WHEN 'DOMINGO' THEN 7 END
END
GO

CREATE OR ALTER PROCEDURE USP_Horario_Registrar
    @ID_Medico BIGINT,
    @Dia_Semana VARCHAR(9),
    @Horario_Entrada TIME,
    @Horario_Salida TIME
AS
BEGIN
    -- Validar solapamiento básico o duplicado exacto
    IF EXISTS (SELECT 1 FROM Horarios_Atencion 
               WHERE ID_Medico = @ID_Medico 
               AND Dia_Semana = @Dia_Semana 
               AND Horario_Entrada = @Horario_Entrada)
    BEGIN
        RAISERROR('El médico ya tiene un horario registrado para esa hora y día.', 16, 1);
        RETURN;
    END

    INSERT INTO Horarios_Atencion (ID_Medico, Dia_Semana, Horario_Entrada, Horario_Salida)
    VALUES (@ID_Medico, @Dia_Semana, @Horario_Entrada, @Horario_Salida);
END
GO

CREATE OR ALTER PROCEDURE USP_Horario_Eliminar
    @ID_Horario BIGINT
AS
BEGIN
    DELETE FROM Horarios_Atencion WHERE ID_Horario = @ID_Horario
END
GO

-- USP_Cita_ListarTodo
-- 1. Listado General
CREATE OR ALTER PROCEDURE USP_Cita_ListarTodo
AS
BEGIN
    SELECT 
        C.ID_Cita, 
        C.Fecha_Cita, 
        C.Hora_Cita, 
        C.Estado, 
        C.Motivo,
        P.Nombres + ' ' + P.Apellidos AS PacienteNombre,
        P.DNI AS PacienteDNI,
        M.Nombres + ' ' + M.Apellidos AS MedicoNombre,
        E.Nombre AS Especialidad
    FROM Cita C
    INNER JOIN Paciente P ON C.ID_Paciente = P.ID_Paciente
    INNER JOIN Medico M ON C.ID_Medico = M.ID_Medico
    INNER JOIN Especialidad E ON M.ID_Especialidad = E.ID_Especialidad
    ORDER BY C.Fecha_Cita DESC, C.Hora_Cita DESC
END
GO

CREATE OR ALTER PROCEDURE USP_Cita_BuscarPorCriterio
    @Criterio VARCHAR(100)
AS
BEGIN
    SET @Criterio = LTRIM(RTRIM(@Criterio));

    SELECT 
        C.ID_Cita, C.Fecha_Cita, C.Hora_Cita, C.Estado, C.Motivo,
        P.Nombres + ' ' + P.Apellidos AS PacienteNombre,
        P.DNI AS PacienteDNI,
        M.Nombres + ' ' + M.Apellidos AS MedicoNombre,
        E.Nombre AS Especialidad
    FROM Cita C
    INNER JOIN Paciente P ON C.ID_Paciente = P.ID_Paciente
    INNER JOIN Medico M ON C.ID_Medico = M.ID_Medico
    INNER JOIN Especialidad E ON M.ID_Especialidad = E.ID_Especialidad
    WHERE 
        -- Filtro para Nombres: sigue siendo flexible
        (P.Nombres + ' ' + P.Apellidos LIKE '%' + @Criterio + '%')
        OR 
        -- Filtro para DNI: Solo si empieza con el criterio o es exacto
        (P.DNI LIKE @Criterio + '%') 
    ORDER BY C.Fecha_Cita DESC, C.Hora_Cita DESC
END
GO

exec USP_Cita_BuscarPorCriterio ''
go

CREATE OR ALTER PROCEDURE USP_Cita_Registrar
    @ID_Medico BIGINT,
    @ID_Paciente BIGINT,
    @ID_Usuario BIGINT,
    @Fecha_Cita DATE,
    @Hora_Cita TIME,
    @Motivo VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    -- IMPORTANTE: Sincronizar el primer día de la semana
    SET DATEFIRST 7; 

    BEGIN TRY
        DECLARE @DiaSemana VARCHAR(10);
        -- Calculamos el nombre del día con la configuración fija
        SET @DiaSemana = (CASE DATEPART(DW, @Fecha_Cita)
                            WHEN 1 THEN 'DOMINGO' WHEN 2 THEN 'LUNES'
                            WHEN 3 THEN 'MARTES'  WHEN 4 THEN 'MIERCOLES'
                            WHEN 5 THEN 'JUEVES'  WHEN 6 THEN 'VIERNES'
                            WHEN 7 THEN 'SABADO' END);

        -- 1. VALIDACIÓN: Usar LTRIM/RTRIM para evitar fallos por espacios en blanco
        IF NOT EXISTS (
            SELECT 1 FROM Horarios_Atencion 
            WHERE ID_Medico = @ID_Medico 
              AND LTRIM(RTRIM(Dia_Semana)) = @DiaSemana
              AND @Hora_Cita >= Horario_Entrada 
              AND @Hora_Cita < Horario_Salida
        )
        BEGIN
            RAISERROR('El médico no tiene turno programado para este día o en esa hora específica.', 16, 1);
            RETURN;
        END

        -- 2. VALIDACIÓN: Disponibilidad (Cruce de citas)
        IF EXISTS (SELECT 1 FROM Cita 
                   WHERE ID_Medico = @ID_Medico 
                   AND Fecha_Cita = @Fecha_Cita 
                   AND Hora_Cita = @Hora_Cita 
                   AND Estado IN ('PENDIENTE', 'CONFIRMADO'))
        BEGIN
            RAISERROR('El médico ya tiene una cita programada para esa fecha y hora.', 16, 1);
            RETURN;
        END

        -- Inserción
        INSERT INTO Cita (ID_Medico, ID_Paciente, ID_Usuario, Fecha_Cita, Hora_Cita, Motivo, Estado)
        VALUES (@ID_Medico, @ID_Paciente, @ID_Usuario, @Fecha_Cita, @Hora_Cita, @Motivo, 'PENDIENTE');

        SELECT SCOPE_IDENTITY() AS ID_Cita;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE USP_Cita_ObtenerPorId
    @ID_Cita BIGINT
AS
BEGIN
    SELECT 
        C.ID_Cita, 
        C.Fecha_Cita, 
        C.Hora_Cita, 
        C.Motivo, 
        C.Estado,
        C.ID_Medico,
        P.Nombres + ' ' + P.Apellidos AS PacienteNombre,
        P.DNI AS PacienteDNI, -- Agregado para el mapeador
        M.Nombres + ' ' + M.Apellidos AS MedicoNombre,
        E.Nombre AS Especialidad -- Agregado para el mapeador
    FROM Cita C
    INNER JOIN Paciente P ON C.ID_Paciente = P.ID_Paciente
    INNER JOIN Medico M ON C.ID_Medico = M.ID_Medico
    INNER JOIN Especialidad E ON M.ID_Especialidad = E.ID_Especialidad
    WHERE C.ID_Cita = @ID_Cita
END
GO

CREATE OR ALTER PROCEDURE USP_Cita_Actualizar
    @ID_Cita BIGINT,
    @Fecha_Cita DATE,
    @Hora_Cita TIME,
    @Motivo VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 7; 

    BEGIN TRY
        DECLARE @ID_Medico BIGINT;
        DECLARE @DiaSemana VARCHAR(10);

        -- 1. Obtener el ID_Medico actual
        SELECT @ID_Medico = ID_Medico FROM Cita WHERE ID_Cita = @ID_Cita;

        -- 2. Calcular nombre del día
        SET @DiaSemana = (CASE DATEPART(DW, @Fecha_Cita)
                            WHEN 1 THEN 'DOMINGO' WHEN 2 THEN 'LUNES'
                            WHEN 3 THEN 'MARTES'  WHEN 4 THEN 'MIERCOLES'
                            WHEN 5 THEN 'JUEVES'  WHEN 6 THEN 'VIERNES'
                            WHEN 7 THEN 'SABADO' END);

        -- 3. VALIDACIÓN: Turno del médico
        IF NOT EXISTS (
            SELECT 1 FROM Horarios_Atencion 
            WHERE ID_Medico = @ID_Medico 
              AND LTRIM(RTRIM(Dia_Semana)) = @DiaSemana
              AND @Hora_Cita >= Horario_Entrada 
              AND @Hora_Cita < Horario_Salida
        )
        BEGIN
            RAISERROR('El médico no tiene turno programado para la nueva fecha/hora.', 16, 1);
            RETURN;
        END

        -- 4. VALIDACIÓN: Disponibilidad (Excluyendo la cita actual)
        IF EXISTS (SELECT 1 FROM Cita 
                   WHERE ID_Medico = @ID_Medico 
                   AND Fecha_Cita = @Fecha_Cita 
                   AND Hora_Cita = @Hora_Cita 
                   AND ID_Cita <> @ID_Cita 
                   AND Estado IN ('PENDIENTE', 'CONFIRMADO'))
        BEGIN
            RAISERROR('El médico ya tiene otra cita programada en ese horario.', 16, 1);
            RETURN;
        END

        -- 5. ACTUALIZACIÓN (Variables corregidas aquí)
        UPDATE Cita 
        SET Fecha_Cita = @Fecha_Cita,
            Hora_Cita = @Hora_Cita, -- Antes decía @Hora_C_ita
            Motivo = @Motivo
        WHERE ID_Cita = @ID_Cita;   -- Antes decía ID_C_ita = @ID_C_ita

        SELECT @ID_Cita AS ID_Cita;

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE USP_Paciente_Buscar
    @Criterio VARCHAR(100) = NULL -- Valor por defecto
AS
BEGIN
    -- Si el criterio es nulo o vacío, podrías optar por no devolver nada 
    -- o devolver los primeros 20 registros.
    SET @Criterio = TRIM(ISNULL(@Criterio, ''));

    SELECT ID_Paciente, Nombres, Apellidos, DNI, Telefono
    FROM Paciente
    WHERE (@Criterio = '' OR 
          (Nombres LIKE '%' + @Criterio + '%' 
           OR Apellidos LIKE '%' + @Criterio + '%' 
           OR DNI LIKE '%' + @Criterio + '%'))
END
GO

CREATE OR ALTER PROCEDURE USP_Horario_VerificarDisponibilidad
    @ID_Medico BIGINT,
    @Fecha DATE
AS
BEGIN
    -- Forzamos que el Domingo sea 1 para el cálculo de esta consulta
    -- Esto evita fallos por configuración regional del servidor
    SET DATEFIRST 7; 

    DECLARE @NombreDia VARCHAR(10);
    SET @NombreDia = (CASE DATEPART(DW, @Fecha)
                        WHEN 1 THEN 'DOMINGO'
                        WHEN 2 THEN 'LUNES'
                        WHEN 3 THEN 'MARTES'
                        WHEN 4 THEN 'MIERCOLES'
                        WHEN 5 THEN 'JUEVES'
                        WHEN 6 THEN 'VIERNES'
                        WHEN 7 THEN 'SABADO'
                      END);

    -- 1. RESULTADO: Horarios de atención (Entrada y Salida)
    -- Usamos Trim() para asegurar que no haya espacios extras en el VARCHAR
    SELECT ID_Horario, Horario_Entrada, Horario_Salida
    FROM Horarios_Atencion
    WHERE ID_Medico = @ID_Medico AND LTRIM(RTRIM(Dia_Semana)) = @NombreDia;

    -- 2. RESULTADO: Citas ya ocupadas
    SELECT Hora_Cita 
    FROM Cita 
    WHERE ID_Medico = @ID_Medico 
      AND Fecha_Cita = @Fecha 
      AND Estado IN ('PENDIENTE', 'CONFIRMADO');
END
GO

CREATE OR ALTER PROCEDURE USP_Cita_Cancelar
    @ID_Cita BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Verificamos si la cita existe y si es cancelable
        IF NOT EXISTS (SELECT 1 FROM Cita WHERE ID_Cita = @ID_Cita)
        BEGIN
            RAISERROR('La cita no existe.', 16, 1);
            RETURN;
        END

        -- Actualizamos el estado a CANCELADO
        UPDATE Cita 
        SET Estado = 'CANCELADO' 
        WHERE ID_Cita = @ID_Cita;

        -- Retornamos el ID para confirmar la acción
        SELECT @ID_Cita AS ID_Cita;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

EXEC USP_Horario_VerificarDisponibilidad 
    @ID_Medico = 1, 
    @Fecha = '2025-12-23';
GO


select * from medico;

select * from cita;

select * from paciente;

SELECT * FROM Horarios_Atencion
GO