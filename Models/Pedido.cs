using System;
using System.Collections.Generic;

namespace GestionComercial.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string DireccionEnvio { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Completado"; // 'Completado', 'Pendiente', 'Cancelado'

        // Propiedades extendidas para facilidad en las vistas y procesamiento
        public string UsuarioNombre { get; set; } = string.Empty;
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
