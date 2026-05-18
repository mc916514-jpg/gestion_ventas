using System;
using System.Collections.Generic;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class ContactoService : IContactoService
    {
        private readonly IContactoRepository _repo;
        private readonly IHistorialAccionRepository _audit;

        public ContactoService(IContactoRepository repo, IHistorialAccionRepository audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public IEnumerable<Contacto> ObtenerTodos()
        {
            return _repo.GetAll();
        }

        public void RegistrarContacto(Contacto contacto)
        {
            contacto.Fecha = DateTime.Now;
            contacto.Respondido = false;
            _repo.Insert(contacto);

            _audit.Insert(new HistorialAccion
            {
                Accion = "CONTACTO_REGISTRAR",
                Detalle = $"Mensaje de contacto de '{contacto.Nombre}' ({contacto.Email}) recibido y archivado.",
                Fecha = DateTime.Now
            });
        }

        public void MarcarRespondido(int id)
        {
            _repo.MarkAsReplied(id);
            _audit.Insert(new HistorialAccion
            {
                Accion = "CONTACTO_RESPONDER",
                Detalle = $"Mensaje de contacto ID {id} marcado como respondido/gestionado por el Administrador.",
                Fecha = DateTime.Now
            });
        }
    }
}
