using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly DbConnectionFactory _db;

        public PedidoRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public IEnumerable<Pedido> GetAll()
        {
            var list = new List<Pedido>();
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT p.Id, p.UsuarioId, p.Fecha, p.DireccionEnvio, p.Subtotal, p.Iva, p.Total, p.Estado, u.Nombre AS UsuarioNombre
                    FROM Pedidos p
                    INNER JOIN Usuarios u ON p.UsuarioId = u.Id
                    ORDER BY p.Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Pedido
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2),
                                DireccionEnvio = reader.GetString(3),
                                Subtotal = reader.GetDecimal(4),
                                Iva = reader.GetDecimal(5),
                                Total = reader.GetDecimal(6),
                                Estado = reader.GetString(7),
                                UsuarioNombre = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public Pedido? GetById(int id)
        {
            Pedido? pedido = null;
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT p.Id, p.UsuarioId, p.Fecha, p.DireccionEnvio, p.Subtotal, p.Iva, p.Total, p.Estado, u.Nombre AS UsuarioNombre
                    FROM Pedidos p
                    INNER JOIN Usuarios u ON p.UsuarioId = u.Id
                    WHERE p.Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pedido = new Pedido
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2),
                                DireccionEnvio = reader.GetString(3),
                                Subtotal = reader.GetDecimal(4),
                                Iva = reader.GetDecimal(5),
                                Total = reader.GetDecimal(6),
                                Estado = reader.GetString(7),
                                UsuarioNombre = reader.GetString(8)
                            };
                        }
                    }
                }

                if (pedido != null)
                {
                    // Cargar detalles de forma segura
                    var detailsQuery = @"
                        SELECT dp.Id, dp.PedidoId, dp.ProductoId, dp.Cantidad, dp.PrecioUnitario, prod.Nombre AS ProductoNombre, prod.ImagenUrl AS ProductoImagenUrl
                        FROM DetallePedidos dp
                        INNER JOIN Productos prod ON dp.ProductoId = prod.Id
                        WHERE dp.PedidoId = @PedidoId";
                    using (var cmdDetails = new SqlCommand(detailsQuery, conn))
                    {
                        cmdDetails.Parameters.Add("@PedidoId", SqlDbType.Int).Value = pedido.Id;
                        using (var reader = cmdDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pedido.Detalles.Add(new DetallePedido
                                {
                                    Id = reader.GetInt32(0),
                                    PedidoId = reader.GetInt32(1),
                                    ProductoId = reader.GetInt32(2),
                                    Cantidad = reader.GetInt32(3),
                                    PrecioUnitario = reader.GetDecimal(4),
                                    ProductoNombre = reader.GetString(5),
                                    ProductoImagenUrl = reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            return pedido;
        }

        public int Insert(Pedido pedido, SqlConnection conn, SqlTransaction trans)
        {
            var query = @"
                INSERT INTO Pedidos (UsuarioId, Fecha, DireccionEnvio, Subtotal, Iva, Total, Estado)
                VALUES (@UsuarioId, @Fecha, @DireccionEnvio, @Subtotal, @Iva, @Total, @Estado);
                SELECT SCOPE_IDENTITY();";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = pedido.UsuarioId;
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = pedido.Fecha;
                cmd.Parameters.Add("@DireccionEnvio", SqlDbType.VarChar, 250).Value = pedido.DireccionEnvio;
                cmd.Parameters.Add("@Subtotal", SqlDbType.Decimal).Value = pedido.Subtotal;
                cmd.Parameters.Add("@Iva", SqlDbType.Decimal).Value = pedido.Iva;
                cmd.Parameters.Add("@Total", SqlDbType.Decimal).Value = pedido.Total;
                cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 50).Value = pedido.Estado;
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void InsertDetalle(DetallePedido detalle, SqlConnection conn, SqlTransaction trans)
        {
            var query = @"
                INSERT INTO DetallePedidos (PedidoId, ProductoId, Cantidad, PrecioUnitario)
                VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario)";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.Add("@PedidoId", SqlDbType.Int).Value = detalle.PedidoId;
                cmd.Parameters.Add("@ProductoId", SqlDbType.Int).Value = detalle.ProductoId;
                cmd.Parameters.Add("@Cantidad", SqlDbType.Int).Value = detalle.Cantidad;
                cmd.Parameters.Add("@PrecioUnitario", SqlDbType.Decimal).Value = detalle.PrecioUnitario;
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<Pedido> GetByUsuarioId(int usuarioId)
        {
            var list = new List<Pedido>();
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT p.Id, p.UsuarioId, p.Fecha, p.DireccionEnvio, p.Subtotal, p.Iva, p.Total, p.Estado, u.Nombre AS UsuarioNombre
                    FROM Pedidos p
                    INNER JOIN Usuarios u ON p.UsuarioId = u.Id
                    WHERE p.UsuarioId = @UsuarioId
                    ORDER BY p.Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Pedido
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2),
                                DireccionEnvio = reader.GetString(3),
                                Subtotal = reader.GetDecimal(4),
                                Iva = reader.GetDecimal(5),
                                Total = reader.GetDecimal(6),
                                Estado = reader.GetString(7),
                                UsuarioNombre = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public decimal GetTotalSales()
        {
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT ISNULL(SUM(Total), 0) FROM Pedidos WHERE Estado = 'Completado'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        public int GetCount()
        {
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT COUNT(*) FROM Pedidos WHERE Estado = 'Completado'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public IEnumerable<Pedido> GetFiltered(DateTime? start, DateTime? end, int? categoryId)
        {
            var list = new List<Pedido>();
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT DISTINCT p.Id, p.UsuarioId, p.Fecha, p.DireccionEnvio, p.Subtotal, p.Iva, p.Total, p.Estado, u.Nombre AS UsuarioNombre
                    FROM Pedidos p
                    INNER JOIN Usuarios u ON p.UsuarioId = u.Id
                    LEFT JOIN DetallePedidos dp ON p.Id = dp.PedidoId
                    LEFT JOIN Productos prod ON dp.ProductoId = prod.Id
                    WHERE p.Estado = 'Completado' ";

                if (start.HasValue)
                {
                    query += " AND p.Fecha >= @Start ";
                }
                if (end.HasValue)
                {
                    query += " AND p.Fecha <= @End ";
                }
                if (categoryId.HasValue)
                {
                    query += " AND prod.CategoriaId = @CategoryId ";
                }

                query += " ORDER BY p.Fecha DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    if (start.HasValue)
                    {
                        cmd.Parameters.Add("@Start", SqlDbType.DateTime).Value = start.Value;
                    }
                    if (end.HasValue)
                    {
                        cmd.Parameters.Add("@End", SqlDbType.DateTime).Value = end.Value;
                    }
                    if (categoryId.HasValue)
                    {
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = categoryId.Value;
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Pedido
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2),
                                DireccionEnvio = reader.GetString(3),
                                Subtotal = reader.GetDecimal(4),
                                Iva = reader.GetDecimal(5),
                                Total = reader.GetDecimal(6),
                                Estado = reader.GetString(7),
                                UsuarioNombre = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
