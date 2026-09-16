using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios
{
       public class Repositorio_UsuarioMySQL :
        RepositorioBase,
        IRepositorio_Usuario
    {
        public Repositorio_UsuarioMySQL(
            IConfiguration configuration)
            : base(configuration)
        {
        }

        public Usuario? ObtenerPorCorreo(string correo)
        {
            Usuario? usuario = null;

            using var connection =
                new MySqlConnection(connectionString);

            string sql = @"
                SELECT id, Nombre, Apellido, Correo,
                       Contraseña, Avatar, Rol, Estado
                FROM Usuarios
                WHERE Correo = @correo
                LIMIT 1;
            ";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@correo",
                correo
            );

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                usuario = new Usuario
                {
                    id = reader.GetInt32("id"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Correo = reader.GetString("Correo"),
                    Contraseña = reader.GetString("Contraseña"),
                    Avatar = reader.GetString("Avatar"),
                    Rol = reader.GetString("Rol"),
                    Estado = reader.GetString("Estado")
                };
            }

            return usuario;
        }
    }
}