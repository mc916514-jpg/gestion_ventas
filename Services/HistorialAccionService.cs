using System;
using System.Collections.Generic;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class HistorialAccionService : IHistorialAccionService
    {
        private readonly IHistorialAccionRepository _repo;

        public HistorialAccionService(IHistorialAccionRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<HistorialAccion> ObtenerUltimos(int count)
        {
            return _repo.GetLatest(count);
        }

        public void RegistrarLog(string accion, string detalle)
        {
            _repo.Insert(new HistorialAccion
            {
                Accion = accion,
                Detalle = detalle,
                Fecha = DateTime.Now
            });
        }
    }
}
