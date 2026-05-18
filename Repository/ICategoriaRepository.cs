using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface ICategoriaRepository
    {
        IEnumerable<Categoria> GetAll();
        Categoria? GetById(int id);
        void Insert(Categoria categoria);
        void Update(Categoria categoria);
        void Delete(int id);
    }
}
