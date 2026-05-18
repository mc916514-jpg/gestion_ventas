using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface IContactoRepository
    {
        IEnumerable<Contacto> GetAll();
        void Insert(Contacto contacto);
        void MarkAsReplied(int id);
    }
}
