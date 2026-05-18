using System;

namespace GestionComercial.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public int ProductoId { get; set; }
        public int Calificacion { get; set; } // 1 a 5
        public string Contenido { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente"; // 'Pendiente', 'Aprobado', 'Rechazado'

        // Propiedad extendida para la interfaz del Admin
        public string ProductoNombre { get; set; } = string.Empty;
    }
}
