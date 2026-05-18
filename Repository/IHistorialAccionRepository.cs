using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface IHistorialAccionRepository
    {
        IEnumerable<HistorialAccion> GetLatest(int count);
        void Insert(HistorialAccion log);
    }
}
