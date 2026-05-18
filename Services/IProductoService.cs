using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface IProductoService
    {
        IEnumerable<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        void Crear(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
        void AjustarStock(int id, int nuevoStock);
    }
}
