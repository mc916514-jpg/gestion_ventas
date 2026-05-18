using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface IHistorialAccionService
    {
        IEnumerable<HistorialAccion> ObtenerUltimos(int count);
        void RegistrarLog(string accion, string detalle);
    }
}
