using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface IProductoRepository
    {
        IEnumerable<Producto> GetAll();
        Producto? GetById(int id);
        void Insert(Producto producto);
        void Update(Producto producto);
        void Delete(int id);
        void AdjustStock(int id, int newStock);
    }
}
