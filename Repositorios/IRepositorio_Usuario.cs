using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios
{
    public interface IRepositorio_Usuario
    {
        // Crear usuario
        int Alta(Usuario usuario);

        // Modificar usuario
        int Modificacion(Usuario usuario);

        // Dar de baja / eliminar usuario
        int Baja(int id);

        // Obtener todos los usuarios
        IList<Usuario> ObtenerTodos();

        // Buscar usuario por ID
        Usuario? ObtenerPorID(int id);

        // Buscar usuario por correo
        Usuario? ObtenerPorCorreo(string correo);
    }
}