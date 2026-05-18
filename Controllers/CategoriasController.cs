using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Models;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public IActionResult Index()
        {
            var categories = _categoriaService.ObtenerTodas();
            return View(categories.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Categoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _categoriaService.Crear(model);
                TempData["SuccessMessage"] = $"Categoría '{model.Nombre}' creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear la categoría: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoriaService.ObtenerPorId(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _categoriaService.Actualizar(model);
                TempData["SuccessMessage"] = $"Categoría '{model.Nombre}' actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar la categoría: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _categoriaService.Eliminar(id);
                TempData["SuccessMessage"] = "Categoría eliminada de la base de datos.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se pudo eliminar la categoría. Asegúrese de que no tenga productos asociados: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult DeleteAjax(int id)
        {
            try
            {
                _categoriaService.Eliminar(id);
                return Json(new { success = true, message = "Categoría eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"No se puede eliminar la categoría: {ex.Message}" });
            }
        }
    }
}
