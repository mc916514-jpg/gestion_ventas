using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionComercial.Models;
using GestionComercial.Services;

namespace GestionComercial.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProductoService _productoService;

        public PedidosController(IPedidoService pedidoService, IProductoService productoService)
        {
            _pedidoService = pedidoService;
            _productoService = productoService;
        }

        // Estructura interna para modelar los ítems del carrito en sesión
        public class CartItem
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public decimal Precio { get; set; }
            public string ImagenUrl { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public decimal Total => Precio * Cantidad;
        }

        // 1. Ver el Carrito de Compras
        [HttpGet]
        public IActionResult Cart()
        {
            var cart = GetCartFromSession();
            ViewBag.Subtotal = cart.Sum(i => i.Total);
            ViewBag.Iva = ViewBag.Subtotal * 0.21m; // IVA del 21%
            ViewBag.Total = ViewBag.Subtotal + ViewBag.Iva;
            return View(cart);
        }

        // 2. Agregar un Producto al Carrito (Endpoint AJAX)
        [HttpPost]
        public IActionResult AddToCart(int productoId, int cantidad = 1)
        {
            var product = _productoService.ObtenerPorId(productoId);
            if (product == null || !product.Estado)
            {
                return Json(new { success = false, message = "El producto no está disponible." });
            }

            if (product.Stock < cantidad)
            {
                return Json(new { success = false, message = $"Stock insuficiente. Solo quedan {product.Stock} unidades." });
            }

            var cart = GetCartFromSession();
            var existing = cart.FirstOrDefault(i => i.ProductoId == productoId);

            if (existing != null)
            {
                if (product.Stock < (existing.Cantidad + cantidad))
                {
                    return Json(new { success = false, message = $"No se puede agregar. El total en el carrito ({existing.Cantidad + cantidad}) supera el stock disponible." });
                }
                existing.Cantidad += cantidad;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductoId = product.Id,
                    Nombre = product.Nombre,
                    Precio = product.Precio,
                    ImagenUrl = product.ImagenUrl,
                    Cantidad = cantidad
                });
            }

            SaveCartToSession(cart);
            var cartCount = cart.Sum(i => i.Cantidad);
            return Json(new { success = true, message = $"'{product.Nombre}' agregado al carrito.", cartCount });
        }

        // 3. Eliminar del Carrito
        [HttpPost]
        public IActionResult RemoveFromCart(int productoId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }
            return RedirectToAction(nameof(Cart));
        }

        // 4. Actualizar Cantidad en el Carrito (Endpoint AJAX)
        [HttpPost]
        public IActionResult UpdateCartQty(int productoId, int cantidad)
        {
            if (cantidad < 1)
            {
                return Json(new { success = false, message = "La cantidad debe ser al menos 1." });
            }

            var product = _productoService.ObtenerPorId(productoId);
            if (product == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            if (product.Stock < cantidad)
            {
                return Json(new { success = false, message = $"Stock insuficiente. Máximo disponible: {product.Stock} unidades." });
            }

            var cart = GetCartFromSession();
            var existing = cart.FirstOrDefault(i => i.ProductoId == productoId);
            if (existing != null)
            {
                existing.Cantidad = cantidad;
                SaveCartToSession(cart);
            }

            var subtotal = cart.Sum(i => i.Total);
            var iva = subtotal * 0.21m;
            var total = subtotal + iva;

            return Json(new { 
                success = true, 
                itemTotal = existing?.Total.ToString("C"), 
                subtotal = subtotal.ToString("C"), 
                iva = iva.ToString("C"), 
                total = total.ToString("C"),
                cartCount = cart.Sum(i => i.Cantidad)
            });
        }

        // 5. Formulario de Pago (Checkout) - Requiere Autenticación
        [HttpGet]
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Tu carrito está vacío. Agrega algunos productos antes de comprar.";
                return RedirectToAction(nameof(Cart));
            }

            ViewBag.Subtotal = cart.Sum(i => i.Total);
            ViewBag.Iva = ViewBag.Subtotal * 0.21m;
            ViewBag.Total = ViewBag.Subtotal + ViewBag.Iva;

            return View();
        }

        // 6. Procesar el Pago (Checkout POST) - Requiere Autenticación y utiliza SqlTransaction
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(string direccionEnvio)
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Tu carrito está vacío.");
                return View();
            }

            var subtotal = cart.Sum(i => i.Total);
            var iva = subtotal * 0.21m;
            var total = subtotal + iva;

            if (string.IsNullOrWhiteSpace(direccionEnvio))
            {
                ModelState.AddModelError("DireccionEnvio", "Debes ingresar una dirección de envío física.");
                ViewBag.Subtotal = subtotal;
                ViewBag.Iva = iva;
                ViewBag.Total = total;
                return View();
            }

            // Obtener el ID del Usuario logueado de los Claims
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Challenge();
            }
            int usuarioId = int.Parse(userIdClaim.Value);

            // Generar la estructura del pedido transaccional
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                Fecha = DateTime.Now,
                DireccionEnvio = direccionEnvio,
                Subtotal = subtotal,
                Iva = iva,
                Total = total,
                Estado = "Completado"
            };

            foreach (var item in cart)
            {
                pedido.Detalles.Add(new DetallePedido
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio
                });
            }

            try
            {
                // EJECUTAR CHECKOUT BAJO LÍMITES DE SQLTRANSACTION
                _pedidoService.ProcesarCompra(pedido);

                // Vaciar carrito tras compra exitosa
                HttpContext.Session.Remove("ShoppingCart");

                TempData["SuccessMessage"] = $"¡Compra procesada con éxito! Su pedido ID #{pedido.Id} se ha generado satisfactoriamente.";
                return RedirectToAction(nameof(Success), new { id = pedido.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al procesar la compra: {ex.Message}");
                ViewBag.Subtotal = subtotal;
                ViewBag.Iva = iva;
                ViewBag.Total = total;
                return View();
            }
        }

        // 7. Página de Éxito
        [HttpGet]
        [Authorize]
        public IActionResult Success(int id)
        {
            var pedido = _pedidoService.ObtenerPorId(id);
            if (pedido == null)
            {
                return NotFound();
            }

            // Control de seguridad: solo el propietario o el administrador pueden ver el pedido
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);
                if (pedido.UsuarioId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
            }

            return View(pedido);
        }

        // 8. Panel de Historial de Pedidos (Para Clientes o Administradores)
        [HttpGet]
        [Authorize]
        public IActionResult Historial()
        {
            if (User.IsInRole("Admin"))
            {
                // Administrador ve todos los pedidos del sistema
                var pedidos = _pedidoService.ObtenerTodos();
                return View(pedidos.ToList());
            }
            else
            {
                // Usuario regular ve solo sus pedidos
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null) return Challenge();
                int userId = int.Parse(userIdClaim.Value);

                var pedidos = _pedidoService.ObtenerPorUsuarioId(userId);
                return View(pedidos.ToList());
            }
        }

        // 9. Detalles del Pedido
        [HttpGet]
        [Authorize]
        public IActionResult Details(int id)
        {
            var pedido = _pedidoService.ObtenerPorId(id);
            if (pedido == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);
                if (pedido.UsuarioId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
            }

            return View(pedido);
        }

        // Helper para leer del carrito en sesión
        private List<CartItem> GetCartFromSession()
        {
            var json = HttpContext.Session.GetString("ShoppingCart");
            if (string.IsNullOrEmpty(json))
            {
                return new List<CartItem>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
        }

        // Helper para guardar el carrito en sesión
        private void SaveCartToSession(List<CartItem> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("ShoppingCart", json);
        }
    }
}
