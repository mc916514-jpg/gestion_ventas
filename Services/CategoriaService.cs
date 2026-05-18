using System;
using System.Collections.Generic;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repo;
        private readonly IHistorialAccionRepository _audit;

        public CategoriaService(ICategoriaRepository repo, IHistorialAccionRepository audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public IEnumerable<Categoria> ObtenerTodas()
        {
            return _repo.GetAll();
        }

        public Categoria? ObtenerPorId(int id)
        {
            return _repo.GetById(id);
        }

        public void Crear(Categoria categoria)
        {
            _repo.Insert(categoria);
            _audit.Insert(new HistorialAccion
            {
                Accion = "CATEGORIA_CREAR",
                Detalle = $"Categoría '{categoria.Nombre}' creada exitosamente.",
                Fecha = DateTime.Now
            });
        }

        public void Actualizar(Categoria categoria)
        {
            _repo.Update(categoria);
            _audit.Insert(new HistorialAccion
            {
                Accion = "CATEGORIA_ACTUALIZAR",
                Detalle = $"Categoría ID {categoria.Id} editada. Nuevo nombre: '{categoria.Nombre}'.",
                Fecha = DateTime.Now
            });
        }

        public void Eliminar(int id)
        {
            var cat = _repo.GetById(id);
            _repo.Delete(id);
            if (cat != null)
            {
                _audit.Insert(new HistorialAccion
                {
                    Accion = "CATEGORIA_ELIMINAR",
                    Detalle = $"Categoría '{cat.Nombre}' (ID {id}) eliminada físicamente de la base de datos.",
                    Fecha = DateTime.Now
                });
            }
        }
    }
}
