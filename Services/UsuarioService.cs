using System;
using System.Security.Cryptography;
using System.Text;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IHistorialAccionRepository _audit;

        public UsuarioService(IUsuarioRepository repo, IHistorialAccionRepository audit)
        {
            _repo = repo;
            _audit = audit;
        }

        public Usuario? Login(string email, string password)
        {
            var user = _repo.GetByEmail(email);
            if (user == null)
            {
                _audit.Insert(new HistorialAccion
                {
                    Accion = "LOGIN_FALLIDO",
                    Detalle = $"Intento de inicio de sesión fallido para el correo electrónico: '{email}'. Usuario no encontrado.",
                    Fecha = DateTime.Now
                });
                return null;
            }

            var hashInput = HashPassword(password);
            if (user.PasswordHash.Equals(hashInput, StringComparison.OrdinalIgnoreCase))
            {
                _audit.Insert(new HistorialAccion
                {
                    Accion = "LOGIN_EXITOSO",
                    Detalle = $"Sesión iniciada correctamente por '{user.Nombre}' (Rol: {user.Rol}).",
                    Fecha = DateTime.Now
                });
                return user;
            }

            _audit.Insert(new HistorialAccion
            {
                Accion = "LOGIN_FALLIDO",
                Detalle = $"Intento de inicio de sesión fallido para el correo electrónico: '{email}'. Contraseña incorrecta.",
                Fecha = DateTime.Now
            });
            return null;
        }

        public bool Registrar(string nombre, string email, string password, string rol = "Usuario")
        {
            var existing = _repo.GetByEmail(email);
            if (existing != null)
            {
                _audit.Insert(new HistorialAccion
                {
                    Accion = "REGISTRO_RECHAZADO",
                    Detalle = $"Se rechazó el registro de '{nombre}' ({email}). El correo electrónico ya se encuentra registrado.",
                    Fecha = DateTime.Now
                });
                return false;
            }

            var newUser = new Usuario
            {
                Nombre = nombre,
                Email = email,
                PasswordHash = HashPassword(password),
                Rol = rol,
                FechaRegistro = DateTime.Now
            };

            _repo.Insert(newUser);

            _audit.Insert(new HistorialAccion
            {
                Accion = "REGISTRO_EXITOSO",
                Detalle = $"Nuevo usuario '{nombre}' ({email}) registrado con éxito con el rol: {rol}.",
                Fecha = DateTime.Now
            });

            return true;
        }

        public int ObtenerTotalUsuarios()
        {
            return _repo.GetCount();
        }

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash).ToLower();
            }
        }
    }
}
