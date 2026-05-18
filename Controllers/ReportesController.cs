using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Models;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportesController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProductoService _productoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IHistorialAccionService _auditService;
        private readonly ICategoriaService _categoriaService;

        public ReportesController(
            IPedidoService pedidoService,
            IProductoService productoService,
            IUsuarioService usuarioService,
            IHistorialAccionService auditService,
            ICategoriaService categoriaService)
        {
            _pedidoService = pedidoService;
            _productoService = productoService;
            _usuarioService = usuarioService;
            _auditService = auditService;
            _categoriaService = categoriaService;
        }

        // 1. Dashboard Principal Administrativo
        [HttpGet]
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalVentas = _pedidoService.ObtenerVentasTotales(),
                PedidosCompletados = _pedidoService.ObtenerTotalPedidos(),
                AlertasStock = _productoService.ObtenerTodos().Count(p => p.Stock <= 5),
                UsuariosRegistrados = _usuarioService.ObtenerTotalUsuarios(),
                AuditLogs = _auditService.ObtenerUltimos(15).ToList()
            };

            return View(model);
        }

        // 2. Reporte Avanzado de Ventas con Filtros
        [HttpGet]
        public IActionResult Ventas(DateTime? start, DateTime? end, int? categoryId)
        {
            ViewBag.Categorias = _categoriaService.ObtenerTodas().Where(c => c.Estado).ToList();
            ViewBag.Start = start?.ToString("yyyy-MM-dd");
            ViewBag.End = end?.ToString("yyyy-MM-dd");
            ViewBag.SelectedCategoria = categoryId;

            var orders = _pedidoService.ObtenerFiltrados(start, end, categoryId);
            return View(orders.ToList());
        }

        // 3. API JSON para alimentar los gráficos interactivos de Chart.js
        [HttpGet]
        public IActionResult GetSalesChartData()
        {
            try
            {
                // Obtener todos los pedidos completados del último mes
                var orders = _pedidoService.ObtenerTodos().Where(o => o.Estado == "Completado").ToList();

                // Ventas diarias de los últimos 7 días con ventas reales
                var last7Days = Enumerable.Range(0, 7)
                    .Select(i => DateTime.Today.AddDays(-i))
                    .OrderBy(d => d)
                    .ToList();

                var dailySales = last7Days.Select(d => new
                {
                    Fecha = d.ToString("dd/MM"),
                    Total = orders.Where(o => o.Fecha.Date == d.Date).Sum(o => o.Total)
                }).ToList();

                // Ventas agrupadas por Categoría
                // Para esto sumamos los detalles
                var allProducts = _productoService.ObtenerTodos().ToList();
                
                // Agrupar ventas por categoría sumando cantidad * precioUnitario de cada detalle
                // Cargamos todos los detalles en memoria de forma segura
                var categorySales = orders
                    .SelectMany(o => _pedidoService.ObtenerPorId(o.Id)?.Detalles ?? new System.Collections.Generic.List<DetallePedido>())
                    .GroupBy(d => {
                        var p = allProducts.FirstOrDefault(prod => prod.Id == d.ProductoId);
                        return p?.CategoriaNombre ?? "General";
                    })
                    .Select(g => new
                    {
                        Categoria = g.Key,
                        Total = g.Sum(d => d.Cantidad * d.PrecioUnitario)
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    labelsDiario = dailySales.Select(x => x.Fecha),
                    datosDiario = dailySales.Select(x => x.Total),
                    labelsCategoria = categorySales.Select(x => x.Categoria),
                    datosCategoria = categorySales.Select(x => x.Total)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
