using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Models;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosController(IProductoService productoService, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public IActionResult Index()
        {
            var products = _productoService.ObtenerTodos();
            return View(products.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateCategoriasDropdown();
            return View(new Producto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto model)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoriasDropdown();
                return View(model);
            }

            try
            {
                _productoService.Crear(model);
                TempData["SuccessMessage"] = $"Producto '{model.Nombre}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear el producto: {ex.Message}");
                PopulateCategoriasDropdown();
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productoService.ObtenerPorId(id);
            if (product == null)
            {
                return NotFound();
            }

            PopulateCategoriasDropdown();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Producto model)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoriasDropdown();
                return View(model);
            }

            try
            {
                _productoService.Actualizar(model);
                TempData["SuccessMessage"] = $"Producto '{model.Nombre}' actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el producto: {ex.Message}");
                PopulateCategoriasDropdown();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _productoService.Eliminar(id);
                TempData["SuccessMessage"] = "Producto eliminado de la base de datos.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se pudo eliminar el producto: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // Endpoint AJAX para eliminar de manera asíncrona sin recargar la página entera
        [HttpDelete]
        public IActionResult DeleteAjax(int id)
        {
            try
            {
                _productoService.Eliminar(id);
                return Json(new { success = true, message = "Producto eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void PopulateCategoriasDropdown()
        {
            var categories = _categoriaService.ObtenerTodas().Where(c => c.Estado).ToList();
            ViewBag.Categorias = categories;
        }
    }
}
