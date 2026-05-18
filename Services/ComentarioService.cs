using System;
using System.Collections.Generic;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _repo;
        private readonly IHistorialAccionRepository _audit;

        public ComentarioService(IComentarioRepository repo, IHistorialAccionRepository audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public IEnumerable<Comentario> ObtenerTodos()
        {
            return _repo.GetAll();
        }

        public IEnumerable<Comentario> ObtenerAprobadosPorProductoId(int productoId)
        {
            return _repo.GetApprovedByProductoId(productoId);
        }

        public void Crear(Comentario comentario)
        {
            // Comentarios creados por defecto están en Pendiente para moderación
            comentario.Estado = "Pendiente";
            comentario.Fecha = DateTime.Now;
            _repo.Insert(comentario);

            _audit.Insert(new HistorialAccion
            {
                Accion = "COMENTARIO_CREAR",
                Detalle = $"Comentario de '{comentario.UsuarioEmail}' sobre el Producto ID {comentario.ProductoId} registrado en espera de moderación.",
                Fecha = DateTime.Now
            });
        }

        public void Aprobar(int id)
        {
            _repo.Approve(id);
            _audit.Insert(new HistorialAccion
            {
                Accion = "COMENTARIO_APROBAR",
                Detalle = $"Comentario ID {id} aprobado por el Administrador.",
                Fecha = DateTime.Now
            });
        }

        public void Rechazar(int id)
        {
            _repo.Reject(id);
            _audit.Insert(new HistorialAccion
            {
                Accion = "COMENTARIO_RECHAZAR",
                Detalle = $"Comentario ID {id} rechazado y ocultado al público.",
                Fecha = DateTime.Now
            });
        }

        public void Eliminar(int id)
        {
            _repo.Delete(id);
            _audit.Insert(new HistorialAccion
            {
                Accion = "COMENTARIO_ELIMINAR",
                Detalle = $"Comentario ID {id} eliminado de forma permanente.",
                Fecha = DateTime.Now
            });
        }
    }
}
