using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly DbConnectionFactory _db;

        public CategoriaRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public IEnumerable<Categoria> GetAll()
        {
            var list = new List<Categoria>();
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT Id, Nombre, Descripcion, Estado FROM Categorias ORDER BY Nombre";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Categoria
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Estado = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public Categoria? GetById(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT Id, Nombre, Descripcion, Estado FROM Categorias WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Categoria
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Estado = reader.GetBoolean(3)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Insert(Categoria categoria)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "INSERT INTO Categorias (Nombre, Descripcion, Estado) VALUES (@Nombre, @Descripcion, @Estado)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = categoria.Nombre;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 500).Value = categoria.Descripcion;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = categoria.Estado;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Categoria categoria)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = categoria.Id;
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = categoria.Nombre;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 500).Value = categoria.Descripcion;
                    cmd.Parameters.Add("@Estado", SqlDbType.Bit).Value = categoria.Estado;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "DELETE FROM Categorias WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
