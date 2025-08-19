create database EntrePatas
drop database EntrePatas
use EntrePatas

-- ===========================================
-- CREACIÓN DE TABLAS
-- ===========================================

CREATE TABLE Usuario(
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) UNIQUE NOT NULL,
    Telefono NVARCHAR(20),
    Direccion NVARCHAR(255) NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    TipoUsuario NVARCHAR(50) NOT NULL CHECK (TipoUsuario IN ('Administrador','Cliente','Veterinario')),
    FechaRegistro DATETIME DEFAULT GETDATE() NOT NULL
);

CREATE TABLE Mascota(
    IdMascota INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Especie NVARCHAR(50) NOT NULL,
    Raza NVARCHAR(50),
    Edad INT,
	FechaRegistro DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE Vacuna(
    IdVacuna INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Precio MONEY NOT NULL
);

CREATE TABLE Solicitud(
    IdSolicitud INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    IdMascota INT NOT NULL,
    FechaSolicitud DATETIME DEFAULT GETDATE(),
    Estado NVARCHAR(50) NOT NULL CHECK (Estado IN ('Pendiente','Aprobada','Rechazada')),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (IdMascota) REFERENCES Mascota(IdMascota)
);

CREATE TABLE SolicitudVacuna(
    IdSolicitudVacuna INT PRIMARY KEY IDENTITY(1,1),
    IdSolicitud INT NOT NULL,
    IdVacuna INT NOT NULL,
    Cantidad INT NOT NULL,
    FOREIGN KEY (IdSolicitud) REFERENCES Solicitud(IdSolicitud),
    FOREIGN KEY (IdVacuna) REFERENCES Vacuna(IdVacuna)
);

CREATE TABLE Producto(
    IdProducto INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Precio MONEY NOT NULL,
    Stock INT NOT NULL
);

CREATE TABLE Pedido(
    IdPedido INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    FechaPedido DATETIME DEFAULT GETDATE(),
    Estado VARCHAR(50) NOT NULL CHECK (Estado IN ('Pendiente','Pagado','Enviado','Completado','Cancelado')),
    Total MONEY NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

CREATE TABLE DetallePedido(
    IdDetalle INT PRIMARY KEY IDENTITY(1,1),
    IdPedido INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario MONEY NOT NULL,
    FOREIGN KEY (IdPedido) REFERENCES Pedido(IdPedido),
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
);

CREATE TABLE Pago(
    IdPago INT PRIMARY KEY IDENTITY(1,1),
    IdPedido INT NOT NULL,
    FechaPago DATETIME DEFAULT GETDATE(),
    Monto MONEY NOT NULL,
    MetodoPago VARCHAR(50) NOT NULL CHECK (MetodoPago IN ('Tarjeta','Yape','Plin','Transferencia','Efectivo')),
    EstadoPago VARCHAR(50) NOT NULL CHECK (EstadoPago IN ('Pendiente','Completado','Fallido')),
    FOREIGN KEY (IdPedido) REFERENCES Pedido(IdPedido)
);

CREATE TABLE Envio(
    IdEnvio INT PRIMARY KEY IDENTITY(1,1),
    IdPedido INT NOT NULL,
    DireccionEnvio VARCHAR(255) NOT NULL,
    EstadoEnvio VARCHAR(50) NOT NULL CHECK (EstadoEnvio IN ('Pendiente','En camino','Entregado','Devuelto')),
    FechaEnvio DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdPedido) REFERENCES Pedido(IdPedido)
);
go 
-- ========================================
-- INSERTS DE USUARIO
-- ========================================
INSERT INTO Usuario (Nombre, Apellido, Email, Telefono, Direccion, Contrasena, TipoUsuario)
VALUES 
('Juan', 'Pérez', 'juan.perez@mail.com', '987654321', 'Av. Siempre Viva 123', 'HASH123', 'Cliente'),
('María', 'García', 'maria.garcia@mail.com', '999888777', 'Calle Falsa 456', 'HASH456', 'Veterinario'),
('Carlos', 'López', 'carlos.lopez@mail.com', '955666444', 'Jr. Las Flores 789', 'HASH789', 'Administrador');
-- ========================================
-- INSERTS DE MASCOTA
-- ========================================
INSERT INTO Mascota (IdUsuario, Nombre, Especie, Raza, Edad)
VALUES
(1, 'Firulais', 'Perro', 'Labrador', 3),
(1, 'Michi', 'Gato', 'Siames', 2),
(2, 'Rocky', 'Perro', 'Bulldog', 4);

-- ========================================
-- INSERTS DE VACUNA
-- ========================================
INSERT INTO Vacuna (Nombre, Descripcion, Precio)
VALUES
('Antirrábica', 'Protección contra la rabia', 50.00),
('Triple Felina', 'Protección contra rinotraqueítis, calicivirus y panleucopenia', 80.00),
('Parvovirus', 'Prevención de parvovirosis canina', 60.00);

-- ========================================
-- INSERTS DE SOLICITUD
-- ========================================
INSERT INTO Solicitud (IdUsuario, IdMascota, Estado)
VALUES
(1, 1, 'Pendiente'),
(1, 2, 'Pendiente'),
(2, 3, 'Aprobada');

-- ========================================
-- INSERTS DE SOLICITUDVACUNA
-- ========================================
INSERT INTO SolicitudVacuna (IdSolicitud, IdVacuna, Cantidad)
VALUES
(1, 1, 1), -- Firulais → Antirrábica
(2, 2, 1), -- Michi → Triple Felina
(3, 3, 2); -- Rocky → 2 dosis Parvovirus

-- ========================================
-- INSERTS DE PRODUCTO
-- ========================================
INSERT INTO Producto (Nombre, Descripcion, Precio, Stock)
VALUES
('Croquetas Dog Chow', 'Alimento balanceado para perros adultos', 120.00, 50),
('Arena Sanitaria', 'Arena absorbente para gatos', 35.00, 100),
('Juguete Pelota', 'Pelota mordedora resistente', 20.00, 200),
('Vitaminas Caninas', 'Suplemento vitamínico para perros', 75.00, 40);

-- ========================================
-- INSERTS DE PEDIDO
-- ========================================
INSERT INTO Pedido (IdUsuario, Estado, Total)
VALUES
(1, 'Pendiente', 140.00),
(2, 'Pagado', 35.00);

-- ========================================
-- INSERTS DE DETALLEPEDIDO
-- ========================================
INSERT INTO DetallePedido (IdPedido, IdProducto, Cantidad, PrecioUnitario)
VALUES
(1, 1, 1, 120.00), -- Croquetas
(1, 3, 1, 20.00),  -- Pelota
(2, 2, 1, 35.00);  -- Arena

-- ========================================
-- INSERTS DE PAGO
-- ========================================
INSERT INTO Pago (IdPedido, Monto, MetodoPago, EstadoPago)
VALUES
(1, 140.00, 'Tarjeta', 'Pendiente'),
(2, 35.00, 'Yape', 'Completado');

-- ========================================
-- INSERTS DE ENVIO
-- ========================================
INSERT INTO Envio (IdPedido, DireccionEnvio, EstadoEnvio)
VALUES
(1, 'Av. Siempre Viva 123', 'Pendiente'),
(2, 'Calle Falsa 456', 'En camino');












go
-- ===========================================
-- PROCEDIMIENTOS ALMACENADOS
-- ===========================================

-- INSERTAR USUARIO
CREATE PROCEDURE sp_InsertarUsuario
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Email NVARCHAR(150),
    @Telefono NVARCHAR(20),
    @Direccion NVARCHAR(255),
    @Contrasena NVARCHAR(255),
    @TipoUsuario NVARCHAR(50)
AS
BEGIN
    INSERT INTO Usuario(Nombre, Apellido, Email, Telefono, Direccion, Contrasena, TipoUsuario)
    VALUES(@Nombre, @Apellido, @Email, @Telefono, @Direccion, @Contrasena, @TipoUsuario);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

-- LISTAR USUARIOS
CREATE PROCEDURE sp_ListarUsuarios
AS
BEGIN
    SELECT *
    FROM Usuario;
    
END;
GO

-- OBTENER USUARIO POR ID
CREATE PROCEDURE sp_ObtenerUsuarioPorId
    @IdUsuario INT
AS
BEGIN
    SELECT * FROM Usuario 
    WHERE IdUsuario = @IdUsuario;
    -- No devolvemos Contraseña
END;
GO

-- ACTUALIZAR USUARIO
CREATE PROCEDURE sp_ActualizarUsuario
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Email NVARCHAR(150),
    @Telefono NVARCHAR(20),
    @Direccion NVARCHAR(255),
    @Contrasena NVARCHAR(255),
    @TipoUsuario NVARCHAR(50)
AS
BEGIN
    UPDATE Usuario
    SET Nombre = @Nombre,
        Apellido = @Apellido,
        Email = @Email,
        Telefono = @Telefono,
        Direccion = @Direccion,
        Contrasena = @Contrasena,
        TipoUsuario = @TipoUsuario
    WHERE IdUsuario = @IdUsuario;
END;
GO

-- ELIMINAR USUARIO
CREATE PROCEDURE sp_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    DELETE FROM Usuario WHERE IdUsuario = @IdUsuario;
END;
GO

-- MASCOTA
CREATE  PROCEDURE sp_InsertarMascota
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Especie NVARCHAR(50),
    @Raza NVARCHAR(50),
    @Edad INT
AS
BEGIN
    INSERT INTO Mascota(IdUsuario,Nombre,Especie,Raza,Edad)
    VALUES(@IdUsuario,@Nombre,@Especie,@Raza,@Edad);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarMascotas
AS
BEGIN
    SELECT * FROM Mascota;
END;
GO

CREATE  PROCEDURE sp_ObtenerMascotaPorId
    @IdMascota INT
AS
BEGIN
    SELECT * FROM Mascota WHERE IdMascota=@IdMascota;
END;
GO

CREATE  PROCEDURE sp_ActualizarMascota
    @IdMascota INT,
    @Nombre NVARCHAR(100),
    @Especie NVARCHAR(50),
    @Raza NVARCHAR(50),
    @Edad INT
AS
BEGIN
    UPDATE Mascota
    SET Nombre=@Nombre,Especie=@Especie,Raza=@Raza,Edad=@Edad
    WHERE IdMascota=@IdMascota;
END;
GO

CREATE  PROCEDURE sp_EliminarMascota
    @IdMascota INT
AS
BEGIN
    DELETE FROM Mascota WHERE IdMascota=@IdMascota;
END;
GO

-- VACUNA
CREATE  PROCEDURE sp_InsertarVacuna
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Precio MONEY
AS
BEGIN
    INSERT INTO Vacuna(Nombre,Descripcion,Precio)
    VALUES(@Nombre,@Descripcion,@Precio);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarVacunas
AS
BEGIN
    SELECT * FROM Vacuna;
END;
GO

CREATE PROCEDURE sp_ObtenerVacunaPorId
    @IdVacuna INT
AS
BEGIN
    SELECT * FROM Vacuna WHERE IdVacuna=@IdVacuna;
END;
GO

CREATE PROCEDURE sp_ActualizarVacuna
    @IdVacuna INT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Precio MONEY
AS
BEGIN
    UPDATE Vacuna
    SET Nombre=@Nombre,Descripcion=@Descripcion,Precio=@Precio
    WHERE IdVacuna=@IdVacuna;
END;
GO

CREATE PROCEDURE sp_EliminarVacuna
    @IdVacuna INT
AS
BEGIN
    DELETE FROM Vacuna WHERE IdVacuna=@IdVacuna;
END;
GO

-- SOLICITUD
CREATE PROCEDURE sp_InsertarSolicitud
    @IdUsuario INT,
    @IdMascota INT,
    @Estado NVARCHAR(50)
AS
BEGIN
    INSERT INTO Solicitud(IdUsuario,IdMascota,Estado)
    VALUES(@IdUsuario,@IdMascota,@Estado);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarSolicitudes
AS
BEGIN
    SELECT * FROM Solicitud;
END;
GO

CREATE  PROCEDURE sp_ObtenerSolicitudPorId
    @IdSolicitud INT
AS
BEGIN
    SELECT * FROM Solicitud WHERE IdSolicitud=@IdSolicitud;
END;
GO

CREATE PROCEDURE sp_ActualizarSolicitud
    @IdSolicitud INT,
    @Estado NVARCHAR(50)
AS
BEGIN
    UPDATE Solicitud SET Estado=@Estado WHERE IdSolicitud=@IdSolicitud;
END;
GO

CREATE  PROCEDURE sp_EliminarSolicitud
    @IdSolicitud INT
AS
BEGIN
    DELETE FROM Solicitud WHERE IdSolicitud=@IdSolicitud;
END;
GO

-- SOLICITUD VACUNA
CREATE PROCEDURE sp_InsertarSolicitudVacuna
    @IdSolicitud INT,
    @IdVacuna INT,
    @Cantidad INT
AS
BEGIN
    IF (@IdSolicitud IS NULL OR @IdVacuna IS NULL OR @Cantidad <= 0)
    BEGIN
        SELECT -1 AS Resultado;
        RETURN;
    END
    INSERT INTO SolicitudVacuna(IdSolicitud,IdVacuna,Cantidad)
    VALUES(@IdSolicitud,@IdVacuna,@Cantidad);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarSolicitudVacunas
AS
BEGIN
    SELECT * FROM SolicitudVacuna;
END;
GO

CREATE  PROCEDURE sp_ObtenerSolicitudVacunaPorId
    @IdSolicitudVacuna INT
AS
BEGIN
    SELECT * FROM SolicitudVacuna WHERE IdSolicitudVacuna=@IdSolicitudVacuna;
END;
GO

CREATE  PROCEDURE sp_EliminarSolicitudVacuna
    @IdSolicitudVacuna INT
AS
BEGIN
    DELETE FROM SolicitudVacuna WHERE IdSolicitudVacuna=@IdSolicitudVacuna;
END;
GO

-- PRODUCTO
CREATE  PROCEDURE sp_InsertarProducto
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Precio MONEY,
    @Stock INT
AS
BEGIN
    INSERT INTO Producto(Nombre,Descripcion,Precio,Stock)
    VALUES(@Nombre,@Descripcion,@Precio,@Stock);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarProductos
AS
BEGIN
    SELECT * FROM Producto;
END;
GO

CREATE  PROCEDURE sp_ObtenerProductoPorId
    @IdProducto INT
AS
BEGIN
    SELECT * FROM Producto WHERE IdProducto=@IdProducto;
END;
GO

CREATE  PROCEDURE sp_ActualizarProducto
    @IdProducto INT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Precio MONEY,
    @Stock INT
AS
BEGIN
    UPDATE Producto
    SET Nombre=@Nombre,Descripcion=@Descripcion,Precio=@Precio,Stock=@Stock
    WHERE IdProducto=@IdProducto;
END;
GO

CREATE  PROCEDURE sp_EliminarProducto
    @IdProducto INT
AS
BEGIN
    DELETE FROM Producto WHERE IdProducto=@IdProducto;
END;
GO

-- PEDIDO
CREATE  PROCEDURE sp_InsertarPedido
    @IdUsuario INT,
    @Estado VARCHAR(50),
    @Total MONEY
AS
BEGIN
    INSERT INTO Pedido(IdUsuario,FechaPedido,Estado,Total)
    VALUES(@IdUsuario,GETDATE(),@Estado,@Total);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarPedidos
AS
BEGIN
    SELECT * FROM Pedido;
END;
GO

CREATE  PROCEDURE sp_ObtenerPedidoPorId
    @IdPedido INT
AS
BEGIN
    SELECT * FROM Pedido WHERE IdPedido=@IdPedido;
END;
GO

CREATE  PROCEDURE sp_ActualizarPedido
    @IdPedido INT,
    @Estado VARCHAR(50),
    @Total MONEY
AS
BEGIN
    UPDATE Pedido SET Estado=@Estado,Total=@Total WHERE IdPedido=@IdPedido;
END;
GO

CREATE  PROCEDURE sp_EliminarPedido
    @IdPedido INT
AS
BEGIN
    DELETE FROM Pedido WHERE IdPedido=@IdPedido;
END;
GO

-- DETALLE PEDIDO
CREATE  PROCEDURE sp_InsertarDetallePedido
    @IdPedido INT,
    @IdProducto INT,
    @Cantidad INT,
    @PrecioUnitario MONEY
AS
BEGIN
    INSERT INTO DetallePedido(IdPedido,IdProducto,Cantidad,PrecioUnitario)
    VALUES(@IdPedido,@IdProducto,@Cantidad,@PrecioUnitario);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarDetallesPedido
AS
BEGIN
    SELECT * FROM DetallePedido;
END;
GO

CREATE  PROCEDURE sp_ObtenerDetallePedidoPorId
    @IdDetalle INT
AS
BEGIN
    SELECT * FROM DetallePedido WHERE IdDetalle=@IdDetalle;
END;
GO

CREATE  PROCEDURE sp_EliminarDetallePedido
    @IdDetalle INT
AS
BEGIN
    DELETE FROM DetallePedido WHERE IdDetalle=@IdDetalle;
END;
GO

-- PAGO
CREATE  PROCEDURE sp_InsertarPago
    @IdPedido INT,
    @MetodoPago VARCHAR(50),
    @Monto MONEY,
    @EstadoPago VARCHAR(50)
AS
BEGIN
    IF (@Monto <= 0)
    BEGIN
        SELECT -1 AS Resultado;
        RETURN;
    END
    INSERT INTO Pago(IdPedido,MetodoPago,Monto,EstadoPago)
    VALUES(@IdPedido,@MetodoPago,@Monto,@EstadoPago);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarPagos
AS
BEGIN
    SELECT * FROM Pago;
END;
GO

CREATE  PROCEDURE sp_ObtenerPagoPorId
    @IdPago INT
AS
BEGIN
    SELECT * FROM Pago WHERE IdPago=@IdPago;
END;
GO

CREATE  PROCEDURE sp_EliminarPago
    @IdPago INT
AS
BEGIN
    DELETE FROM Pago WHERE IdPago=@IdPago;
END;
GO

-- ENVIO
CREATE  PROCEDURE sp_InsertarEnvio
    @IdPedido INT,
    @DireccionEnvio VARCHAR(255),
    @EstadoEnvio VARCHAR(50)
AS
BEGIN
    INSERT INTO Envio(IdPedido,DireccionEnvio,EstadoEnvio)
    VALUES(@IdPedido,@DireccionEnvio,@EstadoEnvio);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

CREATE  PROCEDURE sp_ListarEnvios
AS
BEGIN
    SELECT * FROM Envio;
END;
GO

CREATE  PROCEDURE sp_ObtenerEnvioPorId
    @IdEnvio INT
AS
BEGIN
    SELECT * FROM Envio WHERE IdEnvio=@IdEnvio;
END;
GO

CREATE  PROCEDURE sp_ActualizarEnvio
    @IdEnvio INT,
    @DireccionEnvio VARCHAR(255),
    @EstadoEnvio VARCHAR(50)
AS
BEGIN
    UPDATE Envio
    SET DireccionEnvio=@DireccionEnvio,EstadoEnvio=@EstadoEnvio
    WHERE IdEnvio=@IdEnvio;
END;
GO

CREATE  PROCEDURE sp_EliminarEnvio
    @IdEnvio INT
AS
BEGIN
    DELETE FROM Envio WHERE IdEnvio=@IdEnvio;
END;
GO
