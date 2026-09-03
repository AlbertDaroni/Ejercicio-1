using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios
{
    public class RepositorioInmuebleMySql : RepositorioBase, IRepositorio_Inmueble
    {
        public RepositorioInmuebleMySql(IConfiguration configuration)
            : base(configuration)
        {
        }

        public IList<Inmueble> ObtenerTodos()
        {
            var lista = new List<Inmueble>();

            using var connection = new MySqlConnection(connectionString);

            var sql = @"SELECT id, direccion, cupo, latitud, longitud,
                               precio_dia, porcentaje_seña, estado,
                               id_propietario, id_tipo
                        FROM Inmuebles";

            using var command = new MySqlCommand(sql, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Inmueble
                {
                    id = reader.GetInt32("id"),
                    Direccion = reader.GetString("direccion"),
                    Cupo = reader.GetInt32("cupo"),
                    Latitud = reader.GetDecimal("latitud"),
                    Longitud = reader.GetDecimal("longitud"),
                    Precio_Dia = reader.GetDecimal("precio_dia"),
                    Porcentaje_Seña = reader.GetDecimal("porcentaje_seña"),
                    Estado = reader.GetString("estado"),
                    ID_Propietario = reader.GetInt32("id_propietario"),
                    ID_Tipo = reader.GetInt32("id_tipo")
                });
            }

            return lista;
        }

        public Inmueble? ObtenerPorID(int id)
        {
            Inmueble? inmueble = null;

            using var connection = new MySqlConnection(connectionString);

            var sql = @"SELECT id, direccion, cupo, latitud, longitud,
                               precio_dia, porcentaje_seña, estado,
                               id_propietario, id_tipo
                        FROM Inmuebles
                        WHERE id = @id";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                inmueble = new Inmueble
                {
                    id = reader.GetInt32("id"),
                    Direccion = reader.GetString("direccion"),
                    Cupo = reader.GetInt32("cupo"),
                    Latitud = reader.GetDecimal("latitud"),
                    Longitud = reader.GetDecimal("longitud"),
                    Precio_Dia = reader.GetDecimal("precio_dia"),
                    Porcentaje_Seña = reader.GetDecimal("porcentaje_seña"),
                    Estado = reader.GetString("estado"),
                    ID_Propietario = reader.GetInt32("id_propietario"),
                    ID_Tipo = reader.GetInt32("id_tipo")
                };
            }

            return inmueble;
        }

        public IList<Inmueble> ObtenerPorDireccion(string direccion)
        {
            var lista = new List<Inmueble>();

            using var connection = new MySqlConnection(connectionString);

            var sql = @"SELECT id, direccion, cupo, latitud, longitud,
                               precio_dia, porcentaje_seña, estado,
                               id_propietario, id_tipo
                        FROM Inmuebles
                        WHERE direccion LIKE @direccion";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@direccion",
                "%" + direccion + "%"
            );

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Inmueble
                {
                    id = reader.GetInt32("id"),
                    Direccion = reader.GetString("direccion"),
                    Cupo = reader.GetInt32("cupo"),
                    Latitud = reader.GetDecimal("latitud"),
                    Longitud = reader.GetDecimal("longitud"),
                    Precio_Dia = reader.GetDecimal("precio_dia"),
                    Porcentaje_Seña = reader.GetDecimal("porcentaje_seña"),
                    Estado = reader.GetString("estado"),
                    ID_Propietario = reader.GetInt32("id_propietario"),
                    ID_Tipo = reader.GetInt32("id_tipo")
                });
            }

            return lista;
        }

        public int Alta(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(connectionString);

            var sql = @"INSERT INTO Inmuebles
                        (direccion, cupo, latitud, longitud,
                         precio_dia, porcentaje_seña, estado,
                         id_propietario, id_tipo)
                        VALUES
                        (@direccion, @cupo, @latitud, @longitud,
                         @precio_dia, @porcentaje_seña, @estado,
                         @id_propietario, @id_tipo);

                        SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
            command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
            command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
            command.Parameters.AddWithValue("@precio_dia", inmueble.Precio_Dia);
            command.Parameters.AddWithValue("@porcentaje_seña", inmueble.Porcentaje_Seña);
            command.Parameters.AddWithValue("@estado", inmueble.Estado);
            command.Parameters.AddWithValue("@id_propietario", inmueble.ID_Propietario);
            command.Parameters.AddWithValue("@id_tipo", inmueble.ID_Tipo);

            connection.Open();

            inmueble.id = Convert.ToInt32(command.ExecuteScalar());

            return inmueble.id;
        }

        public int Modificacion(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(connectionString);

            var sql = @"UPDATE Inmuebles
                        SET direccion = @direccion,
                            cupo = @cupo,
                            latitud = @latitud,
                            longitud = @longitud,
                            precio_dia = @precio_dia,
                            porcentaje_seña = @porcentaje_seña,
                            estado = @estado,
                            id_propietario = @id_propietario,
                            id_tipo = @id_tipo
                        WHERE id = @id";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
            command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
            command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
            command.Parameters.AddWithValue("@precio_dia", inmueble.Precio_Dia);
            command.Parameters.AddWithValue("@porcentaje_seña", inmueble.Porcentaje_Seña);
            command.Parameters.AddWithValue("@estado", inmueble.Estado);
            command.Parameters.AddWithValue("@id_propietario", inmueble.ID_Propietario);
            command.Parameters.AddWithValue("@id_tipo", inmueble.ID_Tipo);
            command.Parameters.AddWithValue("@id", inmueble.id);

            connection.Open();

            return command.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);

            var sql = @"DELETE FROM Inmuebles
                        WHERE id = @id";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();

            return command.ExecuteNonQuery();
        }
    }
}