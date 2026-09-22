using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_UsuarioMySQL : RepositorioBase, IRepositorio_Usuario {
        public Repositorio_UsuarioMySQL(IConfiguration configuration) : base(configuration) {}

        // CREACIÓN, MODIFICACIÓN y ELIMINACIÓN
        public int Alta(Usuario usuario) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Usuarios
                    (Nombre, Apellido, Correo, Contraseña, Avatar, Rol, Estado)
                    VALUES
                    (@Nombre, @Apellido, @Correo, @Contraseña, @Avatar, @Rol, @Estado);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Correo", usuario.Correo);
                    command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                    command.Parameters.AddWithValue("@Avatar", usuario.Avatar);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.Parameters.AddWithValue("@Estado", usuario.Estado);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    usuario.id = respuesta;
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Modificacion(Usuario usuario) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Usuarios
                    SET Nombre = @Nombre,
                        Apellido = @Apellido,
                        Correo = @Correo,
                        Contraseña = @Contraseña,
                        Avatar = @Avatar,
                        Rol = @Rol,
                        Estado = @Estado
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Correo", usuario.Correo);
                    command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                    command.Parameters.AddWithValue("@Avatar", usuario.Avatar);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol);
                    command.Parameters.AddWithValue("@Estado", usuario.Estado);
                    command.Parameters.AddWithValue("@id", usuario.id);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Baja(int id) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Usuarios
                    SET Estado = '0'
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        // Obtener todos
        public IList<Usuario> ObtenerTodos() {
            var usuarios = new List<Usuario>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT id, Nombre, Apellido, Correo,
                           Contraseña, Avatar, Rol, Estado
                    FROM Usuarios
                    ORDER BY Apellido, Nombre;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var usuario = new Usuario {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Correo = reader.GetString("Correo"),
                                Contraseña = reader.GetString("Contraseña"),
                                Avatar = reader.GetString("Avatar"),
                                Rol = reader.GetString("Rol"),
                                Estado = reader.GetString("Estado")
                            };

                            usuarios.Add(usuario);
                        }
                    }
                    connection.Close();
                }
            }

            return usuarios;
        }

        // Obtener por ID
        public Usuario? ObtenerPorID(int id) {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT id, Nombre, Apellido, Correo, Contraseña, Avatar, Rol, Estado
                    FROM Usuarios
                    WHERE id = @id
                    LIMIT 1;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            usuario = new Usuario {
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
                    }
                    connection.Close();
                }
            }

            return usuario;
        }

        // Obtener por correo
        public Usuario? ObtenerPorCorreo(string correo) {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT id, Nombre, Apellido, Correo, Contraseña, Avatar, Rol, Estado
                    FROM Usuarios
                    WHERE Correo = @correo
                    LIMIT 1;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@correo", correo);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            usuario = new Usuario {
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
                    }
                    connection.Close();
                }
            }

            return usuario;
        }
    }
}