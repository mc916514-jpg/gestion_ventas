using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComentariosController : Controller
    {
        private readonly IComentarioService _comentarioService;

        public ComentariosController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        public IActionResult Index()
        {
            var comments = _comentarioService.ObtenerTodos();
            return View(comments.ToList());
        }

        [HttpPost]
        public IActionResult Aprobar(int id)
        {
            try
            {
                _comentarioService.Aprobar(id);
                return Json(new { success = true, message = "El comentario ha sido aprobado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Rechazar(int id)
        {
            try
            {
                _comentarioService.Rechazar(id);
                return Json(new { success = true, message = "El comentario ha sido rechazado y ocultado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _comentarioService.Eliminar(id);
                return Json(new { success = true, message = "El comentario ha sido eliminado permanentemente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
