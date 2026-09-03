using Inmobiliaria_.Net_Core.Models;
using MySqlConnector;

namespace Inmobiliaria_.Net_Core.Repositorios
{
    public class RepositorioTipoInmuebleMySql :
        RepositorioBase,
        IRepositorio_Tipo_Inmueble
    {
        public RepositorioTipoInmuebleMySql(
            IConfiguration configuration)
            : base(configuration)
        {
        }

        public IList<Tipo_Inmueble> ObtenerTodos()
        {
            var lista = new List<Tipo_Inmueble>();

            using var connection =
                new MySqlConnection(connectionString);

            var sql = @"SELECT id, nombre, descripcion
                        FROM Tipo_Inmuebles";

            using var command =
                new MySqlCommand(sql, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Tipo_Inmueble
                {
                    id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Descripcion =
                        reader.IsDBNull(reader.GetOrdinal("descripcion"))
                        ? null
                        : reader.GetString("descripcion")
                });
            }

            return lista;
        }

        // Obtener por nombre
        public IList<Tipo_Inmueble> ObtenerPorNombre(string nombre)
        {
            var lista = new List<Tipo_Inmueble>();

            using var connection =
                new MySqlConnection(connectionString);

            var sql = @"SELECT id, nombre, descripcion
                FROM Tipo_Inmuebles
                WHERE nombre LIKE @nombre";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nombre",
                "%" + nombre + "%"
            );

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Tipo_Inmueble
                {
                    id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Descripcion = reader.GetString("descripcion")
                });
            }

            return lista;
        }

        public Tipo_Inmueble? ObtenerPorID(int id)
        {
            Tipo_Inmueble? tipo = null;

            using var connection =
                new MySqlConnection(connectionString);

            var sql = @"SELECT id, nombre, descripcion
                        FROM Tipo_Inmuebles
                        WHERE id = @id";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                tipo = new Tipo_Inmueble
                {
                    id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Descripcion =
                        reader.IsDBNull(reader.GetOrdinal("descripcion"))
                        ? null
                        : reader.GetString("descripcion")
                };
            }

            return tipo;
        }

        public int Alta(Tipo_Inmueble tipo)
        {
            using var connection =
                new MySqlConnection(connectionString);

            var sql = @"INSERT INTO Tipo_Inmuebles
                        (nombre, descripcion)
                        VALUES
                        (@nombre, @descripcion);

                        SELECT LAST_INSERT_ID();";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nombre", tipo.Nombre);

            command.Parameters.AddWithValue(
                "@descripcion", tipo.Descripcion);

            connection.Open();

            tipo.id =
                Convert.ToInt32(command.ExecuteScalar());

            return tipo.id;
        }

        public int Modificacion(Tipo_Inmueble tipo)
        {
            using var connection =
                new MySqlConnection(connectionString);

            var sql = @"UPDATE Tipo_Inmuebles
                        SET nombre = @nombre,
                            descripcion = @descripcion
                        WHERE id = @id";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nombre", tipo.Nombre);

            command.Parameters.AddWithValue(
                "@descripcion", tipo.Descripcion);

            command.Parameters.AddWithValue(
                "@id", tipo.id);

            connection.Open();

            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection =
                new MySqlConnection(connectionString);

            var sql = @"DELETE FROM Tipo_Inmuebles
                        WHERE id = @id";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            return command.ExecuteNonQuery();
        }
    }
}