-- ==========================================
-- SCRIPT DE BASE DE DATOS: GESTION COMERCIAL
-- MOTOR: SQL SERVER
-- INSTANCIA RECOMENDADA: .\SQLEXPRESS
-- ==========================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GestionComercialDb')
BEGIN
    CREATE DATABASE GestionComercialDb;
END
GO

USE GestionComercialDb;
GO

-- 1. TABLA: Categorias
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categorias]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categorias] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nombre] VARCHAR(100) NOT NULL UNIQUE,
        [Descripcion] VARCHAR(500) NOT NULL,
        [Estado] BIT NOT NULL DEFAULT 1
    );
END
GO

-- 2. TABLA: Productos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Productos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Productos] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nombre] VARCHAR(150) NOT NULL,
        [Descripcion] VARCHAR(1000) NOT NULL,
        [Precio] DECIMAL(18,2) NOT NULL,
        [Stock] INT NOT NULL,
        [ImagenUrl] VARCHAR(500) NOT NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [CategoriaId] INT NOT NULL,
        FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
    );
END
GO

-- 3. TABLA: Usuarios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nombre] VARCHAR(100) NOT NULL,
        [Email] VARCHAR(150) NOT NULL UNIQUE,
        [PasswordHash] VARCHAR(256) NOT NULL,
        [Rol] VARCHAR(50) NOT NULL DEFAULT 'Usuario', -- 'Admin', 'Usuario'
        [FechaRegistro] DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 4. TABLA: Pedidos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pedidos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Pedidos] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UsuarioId] INT NOT NULL,
        [Fecha] DATETIME NOT NULL DEFAULT GETDATE(),
        [DireccionEnvio] VARCHAR(250) NOT NULL,
        [Subtotal] DECIMAL(18,2) NOT NULL,
        [Iva] DECIMAL(18,2) NOT NULL,
        [Total] DECIMAL(18,2) NOT NULL,
        [Estado] VARCHAR(50) NOT NULL DEFAULT 'Completado', -- 'Completado', 'Pendiente', 'Cancelado'
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
    );
END
GO

-- 5. TABLA: DetallePedidos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DetallePedidos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DetallePedidos] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PedidoId] INT NOT NULL,
        [ProductoId] INT NOT NULL,
        [Cantidad] INT NOT NULL,
        [PrecioUnitario] DECIMAL(18,2) NOT NULL,
        FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE CASCADE,
        FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
    );
END
GO

-- 6. TABLA: Comentarios
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comentarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Comentarios] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UsuarioEmail] VARCHAR(150) NOT NULL,
        [ProductoId] INT NOT NULL,
        [Calificacion] INT NOT NULL CHECK (Calificacion >= 1 AND Calificacion <= 5),
        [Contenido] VARCHAR(1000) NOT NULL,
        [Fecha] DATETIME NOT NULL DEFAULT GETDATE(),
        [Estado] VARCHAR(50) NOT NULL DEFAULT 'Pendiente', -- 'Pendiente', 'Aprobado', 'Rechazado'
        FOREIGN KEY (ProductoId) REFERENCES Productos(Id) ON DELETE CASCADE
    );
END
GO

-- 7. TABLA: Contactos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contactos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Contactos] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nombre] VARCHAR(100) NOT NULL,
        [Email] VARCHAR(150) NOT NULL,
        [Mensaje] VARCHAR(2000) NOT NULL,
        [Fecha] DATETIME NOT NULL DEFAULT GETDATE(),
        [Respondido] BIT NOT NULL DEFAULT 0
    );
END
GO

-- 8. TABLA: HistorialAcciones
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HistorialAcciones]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[HistorialAcciones] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Accion] VARCHAR(250) NOT NULL,
        [Detalle] VARCHAR(500) NOT NULL,
        [Fecha] DATETIME NOT NULL DEFAULT GETDATE(),
        [IpAddress] VARCHAR(50) NOT NULL
    );
END
GO


-- ==========================================
-- INSERCIÓN DE DATOS SEMILLA (SOLO SI ESTÁN VACÍAS)
-- ==========================================

-- Categorías
IF NOT EXISTS (SELECT * FROM Categorias)
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, Estado) VALUES
    ('Electrónica', 'Smartphones, laptops, Smart TVs y accesorios de última tecnología.', 1),
    ('Hogar y Cocina', 'Artículos electrodomésticos, utensilios de cocina y decoración.', 1),
    ('Moda y Calzado', 'Ropa de temporada, calzado deportivo y accesorios de moda.', 1),
    ('Deportes', 'Equipamiento deportivo, fitness, ropa para entrenar y outdoor.', 1);
END
GO

-- Productos
IF NOT EXISTS (SELECT * FROM Productos)
BEGIN
    DECLARE @ElecId INT = (SELECT Id FROM Categorias WHERE Nombre = 'Electrónica');
    DECLARE @HogarId INT = (SELECT Id FROM Categorias WHERE Nombre = 'Hogar y Cocina');

    INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, ImagenUrl, Estado, CategoriaId) VALUES
    ('Smart TV LED 4K 55"', 'Pantalla Ultra HD Smart TV con soporte HDR10+ y sonido Dolby Atmos.', 12499.00, 12, 'https://images.unsplash.com/photo-1593305841991-05c297ba4575?auto=format&fit=crop&w=150&q=80', 1, @ElecId),
    ('Laptop Pro Core i7 16GB', 'Computadora portátil de alto rendimiento con almacenamiento SSD de 512GB.', 24500.00, 8, 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=150&q=80', 1, @ElecId),
    ('Smartphone Galaxy S24', 'Teléfono inteligente de última generación con cámara de 50MP e Inteligencia Artificial integrada.', 19999.00, 15, 'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?auto=format&fit=crop&w=150&q=80', 1, @ElecId),
    ('Cafetera Espresso Pro', 'Cafetera semiautomática con vaporizador de leche y bomba de presión de 15 bares.', 3450.00, 3, 'https://images.unsplash.com/photo-1517701604599-bb29b565090c?auto=format&fit=crop&w=150&q=80', 1, @HogarId);
END
GO

-- Usuarios
-- Contraseñas hasheadas en SHA256:
-- 'admin123' -> '2407891877b4d1fd7db337d11ec26d36e05342a8b98b9f1d011f06798150495f'
-- 'cliente123' -> '332997184ef4fa6067fa2a06f4f2c00329944a9561b36f1c42f0a1490214a1e9'
IF NOT EXISTS (SELECT * FROM Usuarios)
BEGIN
    INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol, FechaRegistro) VALUES
    ('Martín López', 'admin@comercio.com', '2407891877b4d1fd7db337d11ec26d36e05342a8b98b9f1d011f06798150495f', 'Admin', GETDATE()),
    ('Juan Pérez', 'juan.perez@email.com', '332997184ef4fa6067fa2a06f4f2c00329944a9561b36f1c42f0a1490214a1e9', 'Usuario', GETDATE()),
    ('Lucía Mora', 'lucia.mora@email.com', '332997184ef4fa6067fa2a06f4f2c00329944a9561b36f1c42f0a1490214a1e9', 'Usuario', GETDATE());
END
GO

-- Pedidos y DetallePedidos
IF NOT EXISTS (SELECT * FROM Pedidos)
BEGIN
    DECLARE @JuanId INT = (SELECT Id FROM Usuarios WHERE Email = 'juan.perez@email.com');
    DECLARE @LuciaId INT = (SELECT Id FROM Usuarios WHERE Email = 'lucia.mora@email.com');
    
    DECLARE @TvId INT = (SELECT Id FROM Productos WHERE Nombre LIKE '%Smart TV%');
    DECLARE @LaptopId INT = (SELECT Id FROM Productos WHERE Nombre LIKE '%Laptop%');
    DECLARE @PhoneId INT = (SELECT Id FROM Productos WHERE Nombre LIKE '%Smartphone%');

    -- Pedido 1 (Juan Pérez)
    INSERT INTO Pedidos (UsuarioId, Fecha, DireccionEnvio, Subtotal, Iva, Total, Estado) VALUES
    (@JuanId, DATEADD(day, -16, GETDATE()), 'Av. Juárez #450, Monterrey, N.L.', 12000.00, 1920.00, 13920.00, 'Completado');
    DECLARE @Pedido1Id INT = SCOPE_IDENTITY();
    INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario) VALUES
    (@Pedido1Id, @TvId, 1, 12000.00);

    -- Pedido 2 (Lucía Mora)
    INSERT INTO Pedidos (UsuarioId, Fecha, DireccionEnvio, Subtotal, Iva, Total, Estado) VALUES
    (@LuciaId, DATEADD(day, -8, GETDATE()), 'Calle Pino #120, Guadalupe, N.L.', 24000.00, 3840.00, 27840.00, 'Completado');
    DECLARE @Pedido2Id INT = SCOPE_IDENTITY();
    INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario) VALUES
    (@Pedido2Id, @LaptopId, 1, 24000.00);

    -- Pedido 3 (Juan Pérez)
    INSERT INTO Pedidos (UsuarioId, Fecha, DireccionEnvio, Subtotal, Iva, Total, Estado) VALUES
    (@JuanId, DATEADD(day, -3, GETDATE()), 'Av. Juárez #450, Monterrey, N.L.', 8500.00, 1360.00, 9860.00, 'Completado');
    DECLARE @Pedido3Id INT = SCOPE_IDENTITY();
    INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario) VALUES
    (@Pedido3Id, @PhoneId, 1, 8500.00);
END
GO

-- Comentarios
IF NOT EXISTS (SELECT * FROM Comentarios)
BEGIN
    DECLARE @TvId INT = (SELECT Id FROM Productos WHERE Nombre LIKE '%Smart TV%');
    DECLARE @CoffeeId INT = (SELECT Id FROM Productos WHERE Nombre LIKE '%Cafetera%');

    INSERT INTO Comentarios (UsuarioEmail, ProductoId, Calificacion, Contenido, Fecha, Estado) VALUES
    ('juan.perez@email.com', @TvId, 5, 'Excelente pantalla, el brillo y los colores son espectaculares.', DATEADD(day, -1, GETDATE()), 'Aprobado'),
    ('lucia.mora@email.com', @CoffeeId, 3, 'Hace buen café, pero es un poco ruidosa por las mañanas.', DATEADD(day, -2, GETDATE()), 'Pendiente');
END
GO

-- Contactos
IF NOT EXISTS (SELECT * FROM Contactos)
BEGIN
    INSERT INTO Contactos (Nombre, Email, Mensaje, Fecha, Respondido) VALUES
    ('Carlos Sánchez', 'carlos.s@email.com', 'Me gustaría solicitar una cotización por volumen de 10 laptops. ¿Manejan precios de mayoreo?', DATEADD(day, -2, GETDATE()), 0);
END
GO

-- Historial de Acciones
IF NOT EXISTS (SELECT * FROM HistorialAcciones)
BEGIN
    INSERT INTO HistorialAcciones (Accion, Detalle, Fecha, IpAddress) VALUES
    ('Registro de usuario nuevo', 'Cliente: juan.perez@email.com', DATEADD(minute, -5, GETDATE()), '192.168.1.5'),
    ('Pedido #1084 completado', 'Monto total: $4,550.00 MXN', DATEADD(minute, -24, GETDATE()), '192.168.1.12'),
    ('Entrada de stock de inventario', 'Producto: Smart TV LED 4K (+20 u)', DATEADD(hour, -1, GETDATE()), '127.0.0.1');
END
GO
