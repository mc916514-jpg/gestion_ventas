using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly DbConnectionFactory _db;

        public ContactoRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public IEnumerable<Contacto> GetAll()
        {
            var list = new List<Contacto>();
            using (var conn = _db.GetConnection())
            {
                var query = "SELECT Id, Nombre, Email, Mensaje, Fecha, Respondido FROM Contactos ORDER BY Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Contacto
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Email = reader.GetString(2),
                                Mensaje = reader.GetString(3),
                                Fecha = reader.GetDateTime(4),
                                Respondido = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(Contacto contacto)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "INSERT INTO Contactos (Nombre, Email, Mensaje, Fecha, Respondido) VALUES (@Nombre, @Email, @Mensaje, @Fecha, @Respondido)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = contacto.Nombre;
                    cmd.Parameters.Add("@Email", SqlDbType.VarChar, 150).Value = contacto.Email;
                    cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 2000).Value = contacto.Mensaje;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = contacto.Fecha;
                    cmd.Parameters.Add("@Respondido", SqlDbType.Bit).Value = contacto.Respondido;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MarkAsReplied(int id)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "UPDATE Contactos SET Respondido = 1 WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
