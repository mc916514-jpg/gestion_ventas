using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface IContactoService
    {
        IEnumerable<Contacto> ObtenerTodos();
        void RegistrarContacto(Contacto contacto);
        void MarcarRespondido(int id);
    }
}
