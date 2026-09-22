using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Usuario {
        int Alta(Usuario usuario);
        int Modificacion(Usuario usuario);
        int Baja(int id);

        IList<Usuario> ObtenerTodos();
        Usuario? ObtenerPorID(int id);
        Usuario? ObtenerPorCorreo(string correo);
    }
}