using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactosController : Controller
    {
        private readonly IContactoService _contactoService;

        public ContactosController(IContactoService contactoService)
        {
            _contactoService = contactoService;
        }

        public IActionResult Index()
        {
            var contacts = _contactoService.ObtenerTodos();
            return View(contacts.ToList());
        }

        [HttpPost]
        public IActionResult Responder(int id)
        {
            try
            {
                _contactoService.MarcarRespondido(id);
                return Json(new { success = true, message = "El mensaje ha sido marcado como gestionado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
