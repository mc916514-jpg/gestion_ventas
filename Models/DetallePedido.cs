namespace GestionComercial.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Propiedades extendidas para facilidad en las vistas
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoImagenUrl { get; set; } = string.Empty;
    }
}
