using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly DbConnectionFactory _db;

        public ComentarioRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public IEnumerable<Comentario> GetAll()
        {
            var list = new List<Comentario>();
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT c.Id, c.UsuarioEmail, c.ProductoId, c.Calificacion, c.Contenido, c.Fecha, c.Estado, p.Nombre AS ProductoNombre
                    FROM Comentarios c
                    INNER JOIN Productos p ON c.ProductoId = p.Id
                    ORDER BY c.Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Comentario
                            {
                                Id = reader.GetInt32(0),
                                UsuarioEmail = reader.GetString(1),
                                ProductoId = reader.GetInt32(2),
                                Calificacion = reader.GetInt32(3),
                                Contenido = reader.GetString(4),
                                Fecha = reader.GetDateTime(5),
                                Estado = reader.GetString(6),
                                ProductoNombre = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public IEnumerable<Comentario> GetApprovedByProductoId(int productoId)
        {
            var list = new List<Comentario>();
            using (var conn = _db.GetConnection())
            {
                var query = @"
                    SELECT c.Id, c.UsuarioEmail, c.ProductoId, c.Calificacion, c.Contenido, c.Fecha, c.Estado, p.Nombre AS ProductoNombre
                    FROM Comentarios c
                    INNER JOIN Productos p ON c.ProductoId = p.Id
                    WHERE c.ProductoId = @ProductoId AND c.Estado = 'Aprobado'
                    ORDER BY c.Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@ProductoId", SqlDbType.Int).Value = productoId;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Comentario
                            {
                                Id = reader.GetInt32(0),
                                UsuarioEmail = reader.GetString(1),
                                ProductoId = reader.GetInt32(2),
                                Calificacion = reader.GetInt32(3),
                                Contenido = reader.GetString(4),
                                Fecha = reader.GetDateTime(5),
                                Estado = reader.GetString(6),
                                ProductoNombre = reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(Comentario comentario)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "INSERT INTO Comentarios (UsuarioEmail, ProductoId, Calificacion, Contenido, Fecha, Estado) VALUES (@UsuarioEmail, @ProductoId, @Calificacion, @Contenido, @Fecha, @Estado)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@UsuarioEmail", SqlDbType.VarChar, 150).Value = comentario.UsuarioEmail;
                    cmd.Parameters.Add("@ProductoId", SqlDbType.Int).Value = comentario.ProductoId;
                    cmd.Parameters.Add("@Calificacion", SqlDbType.Int).Value = comentario.Calificacion;
                    cmd.Parameters.Add("@Contenido", SqlDbType.VarChar, 1000).Value = comentario.Contenido;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = comentario.Fecha;
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar, 50).Value = comentario.Estado;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Approve(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "UPDATE Comentarios SET Estado = 'Aprobado' WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Reject(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "UPDATE Comentarios SET Estado = 'Rechazado' WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "DELETE FROM Comentarios WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
