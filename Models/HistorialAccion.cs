using System;

namespace GestionComercial.Models
{
    public class HistorialAccion
    {
        public int Id { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string IpAddress { get; set; } = "127.0.0.1";
    }
}
