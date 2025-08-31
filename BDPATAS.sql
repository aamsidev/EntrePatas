use master

Create DATABASE EntrePatas 
GO

use EntrePatas
GO

-- ===========================================
-- CREACIÓN DE TABLAS
-- ===========================================

CREATE TABLE Usuario(
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(150) UNIQUE NOT NULL,
    Telefono NVARCHAR(20),
    Direccion NVARCHAR(255) NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    TipoUsuario NVARCHAR(50) NOT NULL CHECK (TipoUsuario IN ('Administrador','Cliente')),
    FechaRegistro DATETIME DEFAULT GETDATE() NOT NULL
);
GO
CREATE TABLE Animal(
    IdAnimal INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Especie NVARCHAR(50) NOT NULL,
    Raza NVARCHAR(50),
    Edad INT NOT NULL,
	Estado NVARCHAR(20) NOT NULL CHECK (Estado IN ('Disponible', 'Adoptado', 'Reservado')),
	FechaRegistro DATETIME DEFAULT GETDATE() NOT NULL,
	Foto NVARCHAR(50) NULL,
	Descripcion NVarchar(100) NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);



create TABLE Solicitud(
    IdSolicitud INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL,
    IdAnimal INT NOT NULL,
    FechaSolicitud DATETIME DEFAULT GETDATE(),
    Estado NVARCHAR(50) NOT NULL CHECK (Estado IN ('Pendiente','Aprobada','Rechazada')),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (IdAnimal) REFERENCES Animal(IdAnimal)
);


 Create TABLE Producto(
    IdProducto INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Precio DECIMAL(10,2) NOT NULL,
	FotoUrl NVARCHAR(500) NULL,
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
INSERT INTO Usuario (Nombre, Apellido, Correo, Telefono, Direccion, Contrasena, TipoUsuario)
VALUES 
('Juan', 'Pérez', 'juan.perez@mail.com', '987654321', 'Av. Siempre Viva 123', 'HASH123', 'Cliente'),
('María', 'García', 'maria.garcia@mail.com', '999888777', 'Calle Falsa 456', 'HASH456', 'Cliente'),
('Carlos', 'López', 'carlos.lopez@mail.com', '955666444', 'Jr. Las Flores 789', 'HASH789', 'Administrador');
-- ========================================
-- INSERTS DE MASCOTA
-- ========================================
INSERT INTO Animal (IdUsuario, Nombre, Especie, Raza, Edad, Estado, Foto, Descripcion)
VALUES
(1, 'Firulais', 'Perro', 'Labrador', 3, 'Disponible', 'perro1.jpg', 'Perro juguetón y amigable'),
(1, 'Michi', 'Gato', 'Siames', 2, 'Adoptado', 'perro2.jpg', 'Gato tranquilo y cariñoso'),
(2, 'Rocky', 'Perro', 'Bulldog', 4, 'Disponible', 'perro3.jpg', 'Bulldog fuerte pero muy noble'),
(3, 'Luna', 'Perro', 'Golden Retriever', 1, 'Disponible', 'perro4.jpg', 'Cachorra muy sociable'),
(2, 'Toby', 'Perro', 'Beagle', 5, 'Adoptado', 'perro5.jpg', 'Perro curioso y activo'),
(3, 'Nina', 'Gato', 'Persa', 3, 'Disponible', 'perro6.jpg', 'Gata elegante y tranquila'),
(1, 'Zeus', 'Perro', 'Pastor Alemán', 6, 'Disponible', 'perro7.jpg', 'Excelente guardián y compañero'),
(3, 'Max', 'Perro', 'Husky Siberiano', 2, 'Adoptado', 'perro8.jpg', 'Husky energético y leal'),
(2, 'Pelusa', 'Gato', 'Angora', 4, 'Disponible', 'perro9.jpg', 'Peludo y muy cariñoso'),
(3, 'Bruno', 'Perro', 'Dálmata', 3, 'Disponible', 'perro10.jpg', 'Activo y con mucha personalidad');
-- ========================================
-- INSERTS DE SOLICITUD
-- ========================================
INSERT INTO Solicitud (IdUsuario, IdAnimal, Estado)
VALUES
(1, 1, 'Pendiente'),
(1, 2, 'Pendiente'),
(2, 3, 'Aprobada');

-- ========================================
-- INSERTS DE PRODUCTO
-- ========================================
INSERT INTO Producto (Nombre, Descripcion, Precio, FotoUrl, Stock) VALUES
('Alimento para Perro Premium', 'Alimento balanceado para perros adultos.', 45.99, 'producto1.jpg', 50),
('Juguete para Gatos con Plumas', 'Juguete interactivo para gatos.', 12.50, 'producto2.jpg', 30),
('Champú para Mascotas', 'Champú suave para perros y gatos.', 9.99, 'producto3.jpg', 25),
('Cama Mediana para Perros', 'Cama cómoda y lavable para perros.', 39.95, 'producto4.jpg', 15),
('Transportadora Pequeña', 'Transportadora para mascotas pequeñas.', 29.90, 'producto5.jpg', 20),
('Rascador para Gatos', 'Rascador resistente con base antideslizante.', 24.80, 'producto6.jpg', 18),
('Alimento para Gato Esterilizado', 'Alimento para gatos esterilizados.', 36.40, 'producto7.jpg', 35),
('Pechera Ajustable para Perro', 'Pechera con diseño ergonómico.', 19.95, 'producto8.jpg', 40),
('Pelota con Sonido', 'Pelota de goma con sonido para juegos.', 5.75, 'producto9.jpg', 60),
('Snacks Naturales para Perros', 'Snacks 100% naturales para perros.', 7.25, 'producto10.jpg', 45);

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
    @Correo NVARCHAR(150),
    @Telefono NVARCHAR(20),
    @Direccion NVARCHAR(255),
    @Contrasena NVARCHAR(255),
    @TipoUsuario NVARCHAR(50)
AS
BEGIN
    INSERT INTO Usuario(Nombre, Apellido, Correo, Telefono, Direccion, Contrasena, TipoUsuario)
    VALUES(@Nombre, @Apellido, @Correo, @Telefono, @Direccion, @Contrasena, @TipoUsuario);

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
create PROCEDURE sp_ActualizarUsuario
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Correo NVARCHAR(150),
    @Telefono NVARCHAR(20),
    @Direccion NVARCHAR(255),
    @Contrasena NVARCHAR(255),
    @TipoUsuario NVARCHAR(50)
AS
BEGIN
    UPDATE Usuario
    SET Nombre = @Nombre,
        Apellido = @Apellido,
        Correo = @Correo,
        Telefono = @Telefono,
        Direccion = @Direccion,
        Contrasena = @Contrasena,
        TipoUsuario = @TipoUsuario
    WHERE IdUsuario = @IdUsuario;
	-- Retornar 1 si se actualizó, 0 si no se encontró
    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;

END;
GO

-- ELIMINAR USUARIO
CREATE PROCEDURE sp_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    DELETE FROM Usuario WHERE IdUsuario = @IdUsuario;

	 SELECT @@ROWCOUNT AS Resultado; 

END;
GO

-- MASCOTA
CREATE PROCEDURE sp_InsertarAnimal
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Especie NVARCHAR(50),
    @Raza NVARCHAR(50),
    @Edad INT,
    @Estado NVARCHAR(20),
    @Foto NVARCHAR(255) = NULL,
    @Descripcion NVARCHAR(255)
AS
BEGIN
    INSERT INTO Animal (IdUsuario, Nombre, Especie, Raza, Edad, Estado, Foto, Descripcion)
    VALUES (@IdUsuario, @Nombre, @Especie, @Raza, @Edad, @Estado, @Foto, @Descripcion);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Resultado;
END;
GO

-- LISTAR
CREATE  PROCEDURE sp_ListarAnimal
AS
BEGIN
    SELECT * FROM Animal;
END;
GO

-- OBTENER POR ID
CREATE  PROCEDURE sp_ObtenerAnimalPorId
    @IdAnimal INT
AS
BEGIN
    SELECT * FROM Animal WHERE IdAnimal = @IdAnimal;
END;
GO

-- ACTUALIZAR
CREATE PROCEDURE sp_ActualizarAnimal
    @IdAnimal INT,
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Especie NVARCHAR(50),
    @Raza NVARCHAR(50),
    @Edad INT,
    @Estado NVARCHAR(20),
    @Foto NVARCHAR(255) = NULL,
    @Descripcion NVARCHAR(255)
AS
BEGIN
    UPDATE Animal
    SET IdUsuario = @IdUsuario,
        Nombre = @Nombre,
        Especie = @Especie,
        Raza = @Raza,
        Edad = @Edad,
        Estado = @Estado,
        Foto = @Foto,
        Descripcion = @Descripcion
    WHERE IdAnimal = @IdAnimal;

    -- Retornar 1 si se actualizó, 0 si no
    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;
END;
GO

-- ELIMINAR
CREATE  PROCEDURE sp_EliminarAnimal
    @IdAnimal INT
AS
BEGIN
    DELETE FROM Animal WHERE IdAnimal = @IdAnimal;

    SELECT @@ROWCOUNT AS Resultado;
END;
GO

-- SOLICITUD
CREATE PROCEDURE sp_InsertarSolicitud
    @IdUsuario INT,
    @IdAnimal INT,
    @Estado NVARCHAR(50)
AS
BEGIN
    INSERT INTO Solicitud(IdUsuario,IdAnimal,Estado)
    VALUES(@IdUsuario,@IdAnimal,@Estado);
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
	SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;

END;
GO

CREATE  PROCEDURE sp_EliminarSolicitud
    @IdSolicitud INT
AS
BEGIN
    DELETE FROM Solicitud WHERE IdSolicitud=@IdSolicitud;
	SELECT @@ROWCOUNT AS Resultado;
END;
GO


-- PRODUCTO
CREATE PROCEDURE sp_InsertarProducto
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Precio MONEY,
	@FotoUrl NVARCHAR(500) null,
    @Stock INT
AS
BEGIN
    INSERT INTO Producto(Nombre,Descripcion,Precio,FotoUrl,Stock)
    VALUES(@Nombre,@Descripcion,@Precio,@FotoUrl,@Stock);
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

CREATE PROCEDURE sp_ActualizarProducto
    @IdProducto INT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Precio MONEY,
	@FotoUrl NVARCHAR(255) null,
    @Stock INT
AS
BEGIN
    UPDATE Producto
    SET Nombre=@Nombre,Descripcion=@Descripcion,Precio=@Precio,FotoUrl=@FotoUrl,Stock=@Stock
    WHERE IdProducto=@IdProducto;
	SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;
END;
GO

CREATE  PROCEDURE sp_EliminarProducto
    @IdProducto INT
AS
BEGIN
    DELETE FROM Producto WHERE IdProducto=@IdProducto;
	SELECT @@ROWCOUNT AS Resultado;
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
	SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;
END;
GO

CREATE PROCEDURE sp_EliminarPedido
    @IdPedido INT
AS
BEGIN
    DELETE FROM Pedido WHERE IdPedido=@IdPedido;
	SELECT @@ROWCOUNT AS Resultado;
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

CREATE PROCEDURE sp_ActualizarDetallePedido
    @IdDetalle INT,
    @IdPedido INT,
    @IdProducto INT,
    @Cantidad INT,
    @PrecioUnitario MONEY
AS
BEGIN
    UPDATE DetallePedido
    SET IdPedido = @IdPedido,
        IdProducto = @IdProducto,
        Cantidad = @Cantidad,
        PrecioUnitario = @PrecioUnitario
    WHERE IdDetalle = @IdDetalle;

    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;
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
	SELECT @@ROWCOUNT AS Resultado;
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

CREATE PROCEDURE sp_ActualizarEstadoPago
    @IdPago INT,
    @EstadoPago NVARCHAR(50),
	@MetodoPago NVARCHAR(50)
AS	
BEGIN
    UPDATE Pago
    SET EstadoPago = @EstadoPago , MetodoPago = @MetodoPago
    WHERE IdPago = @IdPago;

    SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;
END;

go

CREATE PROCEDURE sp_ListarPagos
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
	SELECT @@ROWCOUNT AS Resultado;
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

CREATE PROCEDURE sp_ActualizarEnvio
    @IdEnvio INT,
    @DireccionEnvio VARCHAR(255),
    @EstadoEnvio VARCHAR(50)
AS
BEGIN
    UPDATE Envio
    SET DireccionEnvio=@DireccionEnvio,EstadoEnvio=@EstadoEnvio
    WHERE IdEnvio=@IdEnvio;
	 SELECT CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS Resultado;
END;
GO

CREATE PROCEDURE sp_EliminarEnvio
    @IdEnvio INT
AS
BEGIN
    DELETE FROM Envio WHERE IdEnvio=@IdEnvio;
	SELECT @@ROWCOUNT AS Resultado;
END;
GO


CREATE PROCEDURE sp_VerificarLogin
    @Correo NVARCHAR(150),
    @Contrasena NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        *
    FROM Usuario
    WHERE Correo = @Correo
      AND Contrasena = @Contrasena;
END;
GO
select * from Usuario