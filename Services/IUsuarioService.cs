using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface IUsuarioService
    {
        Usuario? Login(string email, string password);
        bool Registrar(string nombre, string email, string password, string rol = "Usuario");
        int ObtenerTotalUsuarios();
    }
}
