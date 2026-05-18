using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface IComentarioService
    {
        IEnumerable<Comentario> ObtenerTodos();
        IEnumerable<Comentario> ObtenerAprobadosPorProductoId(int productoId);
        void Crear(Comentario comentario);
        void Aprobar(int id);
        void Rechazar(int id);
        void Eliminar(int id);
    }
}
