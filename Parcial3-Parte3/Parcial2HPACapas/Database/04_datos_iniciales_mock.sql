-- =====================================================================
-- 04_datos_iniciales_mock.sql
-- Proposito: Cargar datos de prueba (mockups).
-- Ejecutar DESPUES de 03.
--
-- IMPORTANTE (usuarios): el hash fue generado con BCrypt por la MISMA clase
-- de la aplicacion (BCryptHashService, factor de trabajo 11):
--     salt = BCrypt.GenerateSalt(11)
--     hash = BCrypt.HashPassword(contrasena, salt)
-- El login lo comprueba con BCrypt.Verify(contrasena, hash).
-- Credenciales de prueba (correo / contrasena):
--     admin@utp.ac.pa     / Admin123*     (rol Administrador)
--     usuario@utp.ac.pa   / Usuario123*   (rol Usuario)
--     invitado@utp.ac.pa  / Invitado123*  (rol Invitado)
-- =====================================================================

USE parcial2_hpa3;

-- Limpieza para poder re-ejecutar (respeta llaves foraneas).
DELETE FROM login_historial;
DELETE FROM pedidos;
DELETE FROM productos;
DELETE FROM proveedores;
DELETE FROM empleados;
DELETE FROM clientes;
DELETE FROM usuarios;
ALTER TABLE login_historial AUTO_INCREMENT = 1;
ALTER TABLE pedidos     AUTO_INCREMENT = 1;
ALTER TABLE productos   AUTO_INCREMENT = 1;
ALTER TABLE proveedores AUTO_INCREMENT = 1;
ALTER TABLE empleados   AUTO_INCREMENT = 1;
ALTER TABLE clientes    AUTO_INCREMENT = 1;
ALTER TABLE usuarios    AUTO_INCREMENT = 1;

-- Usuarios (hash BCrypt precalculado con BCryptHashService)
INSERT INTO usuarios (nombre, correo, password_hash, salt, rol, activo) VALUES
('Administrador',       'admin@utp.ac.pa',    '$2a$11$quByvJc41Piz2N55Re8xF.91bGY6KTnkGURgOaspvEdZFw9ytSKBK', '$2a$11$quByvJc41Piz2N55Re8xF.', 'Administrador', 1),
('Usuario de Prueba',   'usuario@utp.ac.pa',  '$2a$11$N.Q3C8uwtmNkHNRwRVSCf.aIHLsqXewBfySVNpTpZYKWS79L.gPjW', '$2a$11$N.Q3C8uwtmNkHNRwRVSCf.', 'Usuario',       1),
('Usuario Invitado',    'invitado@utp.ac.pa', '$2a$11$zX11JxuiwF3v7rYdG5sPOe2OiJXlDzd5ULRgynjp9aGy9bEsAh/zK', '$2a$11$zX11JxuiwF3v7rYdG5sPOe', 'Invitado',      1);

-- Clientes
INSERT INTO clientes (nombre, correo, telefono, activo) VALUES
('Comercial La Victoria',   'ventas@lavictoria.com',  '6000-1001', 1),
('Distribuidora Istmo',     'compras@istmo.com',      '6000-1002', 1),
('Tecno Soluciones S.A.',   'info@tecnosol.com',      '6000-1003', 1),
('Importadora El Dorado',   'contacto@eldorado.com',  '6000-1004', 1),
('Servicios Globales',      'admin@globales.com',     '6000-1005', 1),
('Mayorista Central',       'pedidos@central.com',    '6000-1006', 1);

-- Empleados
INSERT INTO empleados (nombre, cargo, activo) VALUES
('Ana Gomez',     'Vendedora',          1),
('Luis Perez',    'Almacen',            1),
('Carla Diaz',    'Atencion al cliente',1),
('Jorge Mora',    'Supervisor',         1);

-- Proveedores
INSERT INTO proveedores (nombre, telefono) VALUES
('Suministros Panama',   '200-3001'),
('Global Parts',         '200-3002'),
('Tech Importers',       '200-3003');

-- Productos
INSERT INTO productos (nombre, precio, stock) VALUES
('Teclado mecanico',      35.00, 40),
('Mouse inalambrico',     18.50, 55),
('Monitor 24 pulgadas',  150.00, 12),
('Laptop Core i5',       820.00,  8),
('Impresora multifuncional', 210.00, 6),
('Disco solido 1TB',      95.00, 25),
('Memoria RAM 16GB',      60.00, 30),
('Audifonos con microfono', 25.00, 48);

-- Pedidos (referencian clientes 1..6)
INSERT INTO pedidos (cliente_id, fecha, total, estado) VALUES
(1, '2026-10-20 09:15:00', 185.00, 'Entregado'),
(2, '2026-10-22 11:40:00', 820.00, 'Enviado'),
(3, '2026-10-25 14:05:00', 150.00, 'Pendiente'),
(4, '2026-10-28 10:30:00', 275.50, 'Entregado'),
(5, '2026-11-01 08:50:00',  95.00, 'Cancelado'),
(1, '2026-11-03 16:20:00', 120.00, 'Enviado'),
(6, '2026-11-04 13:10:00', 430.00, 'Pendiente'),
(2, '2026-11-05 09:00:00',  60.00, 'Entregado'),
(3, '2026-11-06 15:45:00', 210.00, 'Enviado'),
(4, '2026-11-07 12:25:00',  43.50, 'Pendiente');

-- Historial de inicios de sesion (referencia usuarios 1..3).
-- usuario_id NULL = se intento entrar con un correo que no existe.
-- motivo NULL = intento exitoso.
INSERT INTO login_historial (usuario_id, correo, fecha, ip, exito, motivo) VALUES
(1,    'admin@utp.ac.pa',     '2026-11-01 08:02:11', '192.168.1.10', 1, NULL),
(2,    'usuario@utp.ac.pa',   '2026-11-01 09:15:40', '192.168.1.22', 0, 'Contrasena incorrecta'),
(2,    'usuario@utp.ac.pa',   '2026-11-01 09:16:05', '192.168.1.22', 1, NULL),
(NULL, 'admin@utp.com',       '2026-11-02 10:30:12', '192.168.1.35', 0, 'Correo inexistente'),
(3,    'invitado@utp.ac.pa',  '2026-11-02 14:45:00', '192.168.1.41', 1, NULL),
(1,    'admin@utp.ac.pa',     '2026-11-03 07:58:33', '192.168.1.10', 0, 'Contrasena incorrecta'),
(1,    'admin@utp.ac.pa',     '2026-11-03 07:59:02', '192.168.1.10', 0, 'Contrasena incorrecta'),
(1,    'admin@utp.ac.pa',     '2026-11-03 07:59:40', '192.168.1.10', 1, NULL),
(NULL, 'root@utp.ac.pa',      '2026-11-04 23:12:09', '10.0.0.87',    0, 'Correo inexistente'),
(NULL, 'root@utp.ac.pa',      '2026-11-04 23:12:30', '10.0.0.87',    0, 'Correo inexistente'),
(2,    'usuario@utp.ac.pa',   '2026-11-05 11:20:18', '192.168.1.22', 1, NULL),
(3,    'invitado@utp.ac.pa',  '2026-11-06 16:05:51', '192.168.1.41', 0, 'Contrasena incorrecta'),
(1,    'admin@utp.ac.pa',     '2026-11-07 08:10:27', '192.168.1.10', 1, NULL);
