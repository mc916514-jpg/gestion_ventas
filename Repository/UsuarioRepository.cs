using System;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbConnectionFactory _db;

        public UsuarioRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public Usuario? GetById(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT Id, Nombre, Email, PasswordHash, Rol, FechaRegistro FROM Usuarios WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Email = reader.GetString(2),
                                PasswordHash = reader.GetString(3),
                                Rol = reader.GetString(4),
                                FechaRegistro = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Usuario? GetByEmail(string email)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT Id, Nombre, Email, PasswordHash, Rol, FechaRegistro FROM Usuarios WHERE Email = @Email";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = email;
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Email = reader.GetString(2),
                                PasswordHash = reader.GetString(3),
                                Rol = reader.GetString(4),
                                FechaRegistro = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Insert(Usuario usuario)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol, FechaRegistro) VALUES (@Nombre, @Email, @PasswordHash, @Rol, @FechaRegistro)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = usuario.Email;
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar, 256).Value = usuario.PasswordHash;
                    cmd.Parameters.Add("@Rol", SqlDbType.VarChar, 50).Value = usuario.Rol;
                    cmd.Parameters.Add("@FechaRegistro", SqlDbType.DateTime).Value = usuario.FechaRegistro;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetCount()
        {
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT COUNT(*) FROM Usuarios";
                using (var cmd = new SqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}
