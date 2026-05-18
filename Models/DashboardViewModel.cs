using System.Collections.Generic;

namespace GestionComercial.Models
{
    public class DashboardViewModel
    {
        public decimal TotalVentas { get; set; }
        public int PedidosCompletados { get; set; }
        public int AlertasStock { get; set; }
        public int UsuariosRegistrados { get; set; }
        public List<HistorialAccion> AuditLogs { get; set; } = new List<HistorialAccion>();
    }
}
