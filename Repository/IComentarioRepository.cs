using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface IComentarioRepository
    {
        IEnumerable<Comentario> GetAll();
        IEnumerable<Comentario> GetApprovedByProductoId(int productoId);
        void Insert(Comentario comentario);
        void Approve(int id);
        void Reject(int id);
        void Delete(int id);
    }
}
