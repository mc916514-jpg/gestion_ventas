using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public class HistorialAccionRepository : IHistorialAccionRepository
    {
        private readonly DbConnectionFactory _db;

        public HistorialAccionRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public IEnumerable<HistorialAccion> GetLatest(int count)
        {
            var list = new List<HistorialAccion>();
            using (var conn = _db.GetConnection())
            {
                var query = $"SELECT TOP ({count}) Id, Accion, Detalle, Fecha, IpAddress FROM HistorialAcciones ORDER BY Fecha DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new HistorialAccion
                            {
                                Id = reader.GetInt32(0),
                                Accion = reader.GetString(1),
                                Detalle = reader.GetString(2),
                                Fecha = reader.GetDateTime(3),
                                IpAddress = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(HistorialAccion log)
        {
            using (var conn = _db.GetConnection())
            {
                var query = "INSERT INTO HistorialAcciones (Accion, Detalle, Fecha, IpAddress) VALUES (@Accion, @Detalle, @Fecha, @IpAddress)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Accion", SqlDbType.VarChar, 100).Value = log.Accion;
                    cmd.Parameters.Add("@Detalle", SqlDbType.VarChar, 500).Value = log.Detalle;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = log.Fecha;
                    cmd.Parameters.Add("@IpAddress", SqlDbType.VarChar, 50).Value = log.IpAddress;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
