using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface ICategoriaService
    {
        IEnumerable<Categoria> ObtenerTodas();
        Categoria? ObtenerPorId(int id);
        void Crear(Categoria categoria);
        void Actualizar(Categoria categoria);
        void Eliminar(int id);
    }
}
