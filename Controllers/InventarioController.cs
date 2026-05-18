using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventarioController : Controller
    {
        private readonly IProductoService _productoService;

        public InventarioController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public IActionResult Index()
        {
            var products = _productoService.ObtenerTodos();
            return View(products.ToList());
        }

        [HttpPost]
        public IActionResult AjustarStock(int id, int nuevoStock)
        {
            if (nuevoStock < 0)
            {
                return Json(new { success = false, message = "El stock no puede ser un número negativo." });
            }

            try
            {
                _productoService.AjustarStock(id, nuevoStock);
                return Json(new { success = true, message = "Stock actualizado correctamente.", stock = nuevoStock });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
