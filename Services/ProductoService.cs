using System;
using System.Collections.Generic;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repo;
        private readonly IHistorialAccionRepository _audit;

        public ProductoService(IProductoRepository repo, IHistorialAccionRepository audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public IEnumerable<Producto> ObtenerTodos()
        {
            return _repo.GetAll();
        }

        public Producto? ObtenerPorId(int id)
        {
            return _repo.GetById(id);
        }

        public void Crear(Producto producto)
        {
            _repo.Insert(producto);
            _audit.Insert(new HistorialAccion
            {
                Accion = "PRODUCTO_CREAR",
                Detalle = $"Producto '{producto.Nombre}' creado. Precio: {producto.Precio:C}, Stock inicial: {producto.Stock}.",
                Fecha = DateTime.Now
            });
        }

        public void Actualizar(Producto producto)
        {
            _repo.Update(producto);
            _audit.Insert(new HistorialAccion
            {
                Accion = "PRODUCTO_ACTUALIZAR",
                Detalle = $"Producto '{producto.Nombre}' (ID {producto.Id}) actualizado exitosamente.",
                Fecha = DateTime.Now
            });
        }

        public void Eliminar(int id)
        {
            var prod = _repo.GetById(id);
            _repo.Delete(id);
            if (prod != null)
            {
                _audit.Insert(new HistorialAccion
                {
                    Accion = "PRODUCTO_ELIMINAR",
                    Detalle = $"Producto '{prod.Nombre}' (ID {id}) eliminado físicamente de la base de datos.",
                    Fecha = DateTime.Now
                });
            }
        }

        public void AjustarStock(int id, int nuevoStock)
        {
            var prod = _repo.GetById(id);
            if (prod != null)
            {
                var stockAnterior = prod.Stock;
                _repo.AdjustStock(id, nuevoStock);
                _audit.Insert(new HistorialAccion
                {
                    Accion = "PRODUCTO_AJUSTESTOCK",
                    Detalle = $"Stock del producto '{prod.Nombre}' ajustado de {stockAnterior} a {nuevoStock}.",
                    Fecha = DateTime.Now
                });
            }
        }
    }
}
