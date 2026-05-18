using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface IUsuarioRepository
    {
        Usuario? GetById(int id);
        Usuario? GetByEmail(string email);
        void Insert(Usuario usuario);
        int GetCount();
    }
}
