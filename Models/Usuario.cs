using System;

namespace GestionComercial.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario"; // 'Admin', 'Usuario'
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
