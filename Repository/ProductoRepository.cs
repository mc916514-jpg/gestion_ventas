using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly DbConnectionFactory _db;

        public ProductoRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public IEnumerable<Producto> GetAll()
        {
            var list = new List<Producto>();
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.ImagenUrl, p.Estado, p.CategoriaId, c.Nombre AS CategoriaNombre
                    FROM Productos p
                    INNER JOIN Categorias c ON p.CategoriaId = c.Id
                    ORDER BY p.Nombre";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Producto
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Precio = reader.GetDecimal(3),
                                Stock = reader.GetInt32(4),
                                ImagenUrl = reader.GetString(5),
                                Estado = reader.GetBoolean(6),
                                CategoriaId = reader.GetInt32(7),
                                CategoriaNombre = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public Producto? GetById(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.ImagenUrl, p.Estado, p.CategoriaId, c.Nombre AS CategoriaNombre
                    FROM Productos p
                    INNER JOIN Categorias c ON p.CategoriaId = c.Id
                    WHERE p.Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Producto
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Precio = reader.GetDecimal(3),
                                Stock = reader.GetInt32(4),
                                ImagenUrl = reader.GetString(5),
                                Estado = reader.GetBoolean(6),
                                CategoriaId = reader.GetInt32(7),
                                CategoriaNombre = reader.GetString(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Insert(Producto producto)
        {
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, ImagenUrl, Estado, CategoriaId)
                    VALUES (@Nombre, @Descripcion, @Precio, @Stock, @ImagenUrl, @Estado, @CategoriaId)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 150).Value = producto.Nombre;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 1000).Value = producto.Descripcion;
                    cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = producto.Precio;
                    cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = producto.Stock;
                    cmd.Parameters.Add("@ImagenUrl", SqlDbType.VarChar, 500).Value = string.IsNullOrEmpty(producto.ImagenUrl) ? (object)DBNull.Value : producto.ImagenUrl;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = producto.Estado;
                    cmd.Parameters.Add("@CategoriaId", SqlDbType.Int).Value = producto.CategoriaId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Producto producto)
        {
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    UPDATE Productos
                    SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, Stock = @Stock,
                        ImagenUrl = @ImagenUrl, Estado = @Estado, CategoriaId = @CategoriaId
                    WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = producto.Id;
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 150).Value = producto.Nombre;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 1000).Value = producto.Descripcion;
                    cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = producto.Precio;
                    cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = producto.Stock;
                    cmd.Parameters.Add("@ImagenUrl", SqlDbType.VarChar, 500).Value = string.IsNullOrEmpty(producto.ImagenUrl) ? (object)DBNull.Value : producto.ImagenUrl;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = producto.Estado;
                    cmd.Parameters.Add("@CategoriaId", SqlDbType.Int).Value = producto.CategoriaId;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "DELETE FROM Productos WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AdjustStock(int id, int newStock)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "UPDATE Productos SET Stock = @Stock WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = newStock;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
