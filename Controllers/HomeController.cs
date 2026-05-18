using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Models;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IComentarioService _comentarioService;
        private readonly IContactoService _contactoService;

        public HomeController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            IComentarioService comentarioService,
            IContactoService contactoService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _comentarioService = comentarioService;
            _contactoService = contactoService;
        }

        public IActionResult Index(int? categoriaId, string? search)
        {
            var categories = _categoriaService.ObtenerTodas().Where(c => c.Estado);
            var products = _productoService.ObtenerTodos().Where(p => p.Estado);

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                products = products.Where(p => p.CategoriaId == categoriaId.Value);
                ViewBag.SelectedCategoria = categoriaId.Value;
            }

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase) 
                                            || p.Descripcion.Contains(search, StringComparison.OrdinalIgnoreCase));
                ViewBag.SearchText = search;
            }

            ViewBag.Categorias = categories.ToList();
            return View(products.ToList());
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _productoService.ObtenerPorId(id);
            if (product == null || !product.Estado)
            {
                return NotFound();
            }

            var reviews = _comentarioService.ObtenerAprobadosPorProductoId(id);
            ViewBag.Reviews = reviews.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarContacto(Contacto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor verifica los campos del formulario de contacto.";
                return RedirectToAction(nameof(Index));
            }

            _contactoService.RegistrarContacto(model);
            TempData["SuccessMessage"] = "¡Gracias por contactarnos! Responderemos a la brevedad.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarComentario(Comentario model)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión para publicar un comentario.";
                return RedirectToAction(nameof(Details), new { id = model.ProductoId });
            }

            if (model.Calificacion < 1 || model.Calificacion > 5 || string.IsNullOrEmpty(model.Contenido))
            {
                TempData["ErrorMessage"] = "Por favor ingresa una calificación válida (1-5 estrellas) y escribe un mensaje.";
                return RedirectToAction(nameof(Details), new { id = model.ProductoId });
            }

            model.UsuarioEmail = User.Identity.Name ?? "anonimo@gestioncomercial.com";
            _comentarioService.Crear(model);

            TempData["SuccessMessage"] = "Tu comentario ha sido enviado y se encuentra en espera de aprobación.";
            return RedirectToAction(nameof(Details), new { id = model.ProductoId });
        }

        [HttpGet]
        public async Task<IActionResult> ClimaProxy()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Madrid coords
                    var response = await client.GetStringAsync("https://api.open-meteo.com/v1/forecast?latitude=40.4168&longitude=-3.7038&current_weather=true");
                    return Content(response, "application/json");
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
