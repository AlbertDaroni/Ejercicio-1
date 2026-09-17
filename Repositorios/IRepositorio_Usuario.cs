using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Usuario {
        Usuario? ObtenerPorCorreo (string correo);
        IList<Usuario> ObtenerTodos ();
    }
}