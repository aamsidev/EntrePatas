create database EntrePatas
drop database EntrePatas
use EntrePatas
go
CREATE TABLE Usuario (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Correo VARCHAR(150) NOT NULL UNIQUE,
    Contrasena VARCHAR(255) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    TipoUsuario VARCHAR(50) NOT NULL DEFAULT 'Cliente'
);
go
CREATE TABLE Denuncia (
    IdDenuncia INT PRIMARY KEY IDENTITY(1,1),
    FechaDenuncia DATETIME DEFAULT GETDATE(),
    Descripcion VARCHAR(500) NOT NULL,
    Evidencia VARCHAR(255),
    Ubicacion VARCHAR(255),
    IdUsuario INT NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);
go
CREATE TABLE Animal(
    IdAnimal INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Especie VARCHAR(100) NOT NULL,
    Raza VARCHAR(100) NOT NULL,
    Edad INT NOT NULL,
    EstadoSalud VARCHAR(50) NOT NULL CHECK (EstadoSalud IN ('Sano', 'Herido', 'Enfermo', 'Crítico')),
    Estado VARCHAR(100) NOT NULL CHECK (Estado IN ('Disponible', 'Adoptado', 'Bajo observación')),
    FechaRegistro DATETIME DEFAULT GETDATE() -- Agregamos la fecha de registro con la fecha y hora actual
);

go

CREATE TABLE SolicitudAdopcion(
IdSolicitud INT PRIMARY KEY IDENTITY(1,1),
FechaSolicitud DATETIME DEFAULT GETDATE(),
Evidencia VARCHAR(100)NOT NULL,
Ubicacion VARCHAR(100)NOT NULL,
IdUsuario INT NOT NULL,
IdAnimal INT NOT NULL,
FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
FOREIGN KEY (IdAnimal) REFERENCES Animal(IdAnimal),
);

GO

CREATE TABLE Vacuna(
IdVacuna  INT PRIMARY KEY IDENTITY(1,1),
Nombre VARCHAR(100)NOT NULL,
Descripcion VARCHAR(100)NOT NULL,
Precio Money NOT NULL
);


CREATE TABLE SolicitudVacuna(
IdSolicitudVacuna INT PRIMARY KEY IDENTITY(1,1),
IdSolicitud INT NOT NULL,
IdVacuna INT NOT NULL,
 Cantidad INT NOT NULL,
FOREIGN KEY (IdSolicitud) REFERENCES SolicitudAdopcion(IdSolicitud),
FOREIGN KEY (IdVacuna) REFERENCES Vacuna(IdVacuna)
);


/*
INSERCIONES
*/
INSERT INTO Usuario (Nombre, Apellido, Correo, Contrasena, TipoUsuario)
VALUES
('Juan', 'Pérez', 'juan.perez@example.com', '1234abcd', 'Administrador'),
('María', 'López', 'maria.lopez@example.com', 'abcd1234', 'Cliente'),
('Carlos', 'García', 'carlos.garcia@example.com', 'pass1234', 'Cliente');

GO

INSERT INTO Denuncia (FechaDenuncia, Descripcion, Evidencia, Ubicacion, IdUsuario)
VALUES 
('2024-05-10', 'Perro herido en la pata cerca del mercado', 'perro_herido.jpg', 'Mercado Municipal - Av. Grau', 1),
('2024-06-21', 'Perro perdido con collar rojo', 'perro_collar_rojo.jpg', 'Plaza San Martin', 2),
('2024-07-03', 'Cachorro abandonado en la calle', 'cachorro_abandonado.jpg', 'Jr. Los Pinos 456', 1);

go

INSERT INTO Animal (Nombre, Especie, Raza, Edad , EstadoSalud, Estado)
VALUES 
    ('Luna', 'Perro', 'Labrador', 3 ,'Sano','Disponible' ),
    ('Max', 'Gato', 'Siames', 2,'Sano','Disponible' ),
    ('Bella', 'Perro', 'Bulldog', 5,'Sano','Disponible'),
    ('Oliver', 'Gato', 'Persa', 4,'Sano','Disponible'),
    ('Rocky', 'Perro', 'Pastor Alemán', 6,'Sano','Disponible');



INSERT INTO SolicitudAdopcion (Evidencia, Ubicacion, IdUsuario, IdAnimal)
VALUES 
    ('Evidencia de adopción para el perro', 'Ciudad A', 1, 1),
    ('Evidencia de adopción para el gato', 'Ciudad B', 2, 2),
    ('Evidencia de adopción para el conejo', 'Ciudad C', 3, 3);
	
	go

	INSERT INTO Vacuna (Nombre, Descripcion, Precio)
VALUES 
    ('Vacuna Parvovirus', 'Vacuna para prevenir el parvovirus en perros', 250.00),
    ('Vacuna Rabia', 'Vacuna para prevenir la rabia en perros y gatos', 150.00),
    ('Vacuna Leptospirosis', 'Vacuna para prevenir la leptospirosis en perros', 200.00),
    ('Vacuna Hepatitis Canina', 'Vacuna para prevenir la hepatitis canina', 220.00),
    ('Vacuna Bordetella', 'Vacuna para prevenir la tos de las perreras', 180.00);

	go

	INSERT INTO SolicitudVacuna (IdSolicitud, IdVacuna, Cantidad)
VALUES 
    (1, 1, 2),  -- Solicitud 1, Vacuna 1, Cantidad 2
    (1, 2, 1),  -- Solicitud 1, Vacuna 2, Cantidad 1
    (2, 1, 3);  -- Solicitud 2, Vacuna 1, Cantidad 3

	go
/*
PROSEGURES USUARIO
*/

CREATE PROCEDURe sp_ObtenerUsuarios
as
begin
select * from Usuario 
end;

go

Create procedure sp_ObtenerUsuariosPorId
@IdUsuario INT 
as
begin select * from Usuario where IdUsuario=@IdUsuario;
end;
go


CREATE OR ALTER PROC sp_InsertarUsuario
    @Nombre       VARCHAR(100),
    @Apellido     VARCHAR(100),
    @Correo       VARCHAR(150),
    @Contrasena   VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos obligatorios
    IF (@Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = '' OR
        @Apellido IS NULL OR LTRIM(RTRIM(@Apellido)) = '' OR
        @Correo IS NULL OR LTRIM(RTRIM(@Correo)) = '' OR
        @Contrasena IS NULL OR LTRIM(RTRIM(@Contrasena)) = '')
    BEGIN
        SELECT -1 AS Resultado; -- Faltan datos
        RETURN;
    END

    -- Validar correo duplicado
    IF EXISTS (SELECT 1 FROM Usuario WHERE Correo = @Correo)
    BEGIN
        SELECT -2 AS Resultado; -- Correo ya existe
        RETURN;
    END

    -- Insertar usuario con tipo por defecto 'Ciudadano'
    INSERT INTO Usuario (Nombre, Apellido, Correo, Contrasena)
    VALUES (@Nombre, @Apellido, @Correo, @Contrasena);

    -- Devolver ID
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;

go


CREATE OR ALTER PROC sp_InsertarAdmin
    @Nombre       VARCHAR(100),
    @Apellido     VARCHAR(100),
    @Correo       VARCHAR(150),
    @Contrasena   VARCHAR(255),
    @TipoUsuario  VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Solo permitir 'Administrador'
    IF (@TipoUsuario <> 'Administrador')
    BEGIN
        SELECT -3 AS Resultado; -- Tipo inválido
        RETURN;
    END

    -- Validar campos obligatorios
    IF (@Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = '' OR
        @Apellido IS NULL OR LTRIM(RTRIM(@Apellido)) = '' OR
        @Correo IS NULL OR LTRIM(RTRIM(@Correo)) = '' OR
        @Contrasena IS NULL OR LTRIM(RTRIM(@Contrasena)) = '')
    BEGIN
        SELECT -1 AS Resultado;
        RETURN;
    END

    -- Validar correo duplicado
    IF EXISTS (SELECT 1 FROM Usuario WHERE Correo = @Correo)
    BEGIN
        SELECT -2 AS Resultado;
        RETURN;
    END

    -- Insertar admin
    INSERT INTO Usuario (Nombre, Apellido, Correo, Contrasena, TipoUsuario)
    VALUES (@Nombre, @Apellido, @Correo, @Contrasena, @TipoUsuario);

    -- Devolver ID
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;




/*fin */
go
/*Prosegure Denuncias*/

CREATE PROCEDURe sp_ObtenerDenuncias
as
begin
select * from Denuncia 
end;
go

Create procedure sp_ObtenerDenunciasPorId
@IdDenuncia INT 
as
begin select * from Denuncia where IdDenuncia=@IdDenuncia
end;
go

CREATE  PROC sp_InsertarDenuncia
    @Descripcion   VARCHAR(500),
    @Evidencia     VARCHAR(255) = NULL,
    @Ubicacion     VARCHAR(255) = NULL,
    @IdUsuario     INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos obligatorios
    IF (@Descripcion IS NULL OR LTRIM(RTRIM(@Descripcion)) = '' OR
        @IdUsuario IS NULL OR @IdUsuario <= 0)
    BEGIN
        SELECT -1 AS Resultado; -- Faltan datos
        RETURN;
    END

    -- Validar que el usuario exista
    IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT -2 AS Resultado; -- Usuario no encontrado
        RETURN;
    END

    -- Insertar la denuncia
    INSERT INTO Denuncia (Descripcion, Evidencia, Ubicacion, IdUsuario)
    VALUES (@Descripcion, @Evidencia, @Ubicacion, @IdUsuario);

    -- Retornar el ID recién creado
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO
/*Procegure Animal*/

CREATE PROCEDURE sp_ObtenerAnimales
as
begin
select * from Animal
end;
go

Create  procedure sp_ObtenerAnimalesPorId
@IdAnimal INT 
as
begin select * from Animal where IdAnimal=@IdAnimal;
end;
go

CREATE  PROC sp_InsertarAnimal
    @Nombre        VARCHAR(100),
    @Especie       VARCHAR(100),
    @Raza          VARCHAR(100),
    @Edad          INT,
    @EstadoSalud   VARCHAR(50),
    @Estado        VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos obligatorios
    IF (@Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = '' OR
        @Especie IS NULL OR LTRIM(RTRIM(@Especie)) = '' OR
        @Raza IS NULL OR LTRIM(RTRIM(@Raza)) = '' OR
        @Edad IS NULL OR @Edad <= 0 OR
        @EstadoSalud IS NULL OR LTRIM(RTRIM(@EstadoSalud)) = '' OR
        @Estado IS NULL OR LTRIM(RTRIM(@Estado)) = '')
    BEGIN
        SELECT -1 AS Resultado; -- Faltan datos
        RETURN;
    END

    -- Validar que el estado de salud sea uno de los valores permitidos
    IF (@EstadoSalud NOT IN ('Sano', 'Herido', 'Enfermo', 'Crítico'))
    BEGIN
        SELECT -2 AS Resultado; -- Estado de salud no válido
        RETURN;
    END
	IF (@Estado NOT IN ('Disponible', 'Adoptado', 'Bajo observación'))
    BEGIN
        SELECT -3 AS Resultado; -- Estado no válido
        RETURN;
    END

    -- Insertar el animal
    INSERT INTO Animal (Nombre, Especie, Raza, Edad, EstadoSalud, Estado)
    VALUES (@Nombre, @Especie, @Raza, @Edad, @EstadoSalud, @Estado);

    -- Retornar el ID recién creado
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO


/*Procegure SolicitudAdopcion*/


CREATE PROCEDURe sp_ObtenerSolicitudAdopcion
as
begin
select * from SolicitudAdopcion
end;
go

Create procedure sp_ObtenerSolicitudPorId
@IdSolicitud INT 
as
begin select * from SolicitudAdopcion where IdSolicitud=@IdSolicitud;
end;

go

CREATE  PROC sp_InsertarSolicitudAdopcion
    @Evidencia     VARCHAR(100),
    @Ubicacion     VARCHAR(100),
    @IdUsuario     INT,
    @IdAnimal      INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos obligatorios
    IF (@Evidencia IS NULL OR LTRIM(RTRIM(@Evidencia)) = '' OR
        @Ubicacion IS NULL OR LTRIM(RTRIM(@Ubicacion)) = '' OR
        @IdUsuario IS NULL OR @IdUsuario <= 0 OR
        @IdAnimal IS NULL OR @IdAnimal <= 0)
    BEGIN
        SELECT -1 AS Resultado; -- Faltan datos
        RETURN;
    END

    -- Validar que el usuario exista
    IF NOT EXISTS (SELECT 1 FROM Usuario WHERE IdUsuario = @IdUsuario)
    BEGIN
        SELECT -2 AS Resultado; -- Usuario no encontrado
        RETURN;
    END

    -- Validar que el animal exista
    IF NOT EXISTS (SELECT 1 FROM Animal WHERE IdAnimal = @IdAnimal)
    BEGIN
        SELECT -3 AS Resultado; -- Animal no encontrado
        RETURN;
    END

    -- Insertar la solicitud de adopción
    INSERT INTO SolicitudAdopcion (Evidencia, Ubicacion, IdUsuario, IdAnimal)
    VALUES (@Evidencia, @Ubicacion, @IdUsuario, @IdAnimal);

    -- Retornar el ID recién creado
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO


/*Procedure Vacuna*/

CREATE PROCEDURE sp_ObtenerVacunas
AS
BEGIN
    SELECT * FROM Vacuna;
END;
GO


CREATE PROCEDURE sp_ObtenerVacunaPorId
    @IdVacuna INT
AS
BEGIN
    SELECT * FROM Vacuna WHERE IdVacuna = @IdVacuna;
END;
GO

CREATE  PROCEDURE sp_InsertarVacuna
    @Nombre       VARCHAR(100),
    @Descripcion  VARCHAR(100),
    @Precio       MONEY
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos obligatorios
    IF (@Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = '' OR
        @Descripcion IS NULL OR LTRIM(RTRIM(@Descripcion)) = '' OR
        @Precio IS NULL)
    BEGIN
        SELECT -1 AS Resultado; -- Faltan datos
        RETURN;
    END

    -- Insertar vacuna
    INSERT INTO Vacuna (Nombre, Descripcion, Precio)
    VALUES (@Nombre, @Descripcion, @Precio);

    -- Devolver ID
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO


/*PROCEDURE SOLICITUDVACUNA*/

CREATE PROCEDURE sp_ObtenerSolicitudVacunas
AS
BEGIN
    SELECT * FROM SolicitudVacuna;
END;
GO

CREATE PROCEDURE sp_ObtenerSolicitudVacunaPorId
    @IdSolicitudVacuna INT
AS
BEGIN
    SELECT * FROM SolicitudVacuna WHERE IdSolicitudVacuna = @IdSolicitudVacuna;
END
GO

CREATE  PROCEDURE sp_InsertarSolicitudVacuna
    @IdSolicitud INT,
    @IdVacuna INT,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar campos obligatorios
    IF (@IdSolicitud IS NULL OR @IdVacuna IS NULL OR @Cantidad <= 0)
    BEGIN
        SELECT -1 AS Resultado; -- Faltan datos
        RETURN;
    END

    -- Insertar solicitud vacuna
    INSERT INTO SolicitudVacuna (IdSolicitud, IdVacuna, Cantidad)
    VALUES (@IdSolicitud, @IdVacuna, @Cantidad);

    -- Devolver ID
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO
