namespace GestionComercial.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
        public int CategoriaId { get; set; }

        // Propiedad extendida para facilidad en la vista
        public string CategoriaNombre { get; set; } = string.Empty;
    }
}
