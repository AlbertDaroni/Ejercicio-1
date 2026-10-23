using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios
{
    public class Repositorio_InmuebleMySQL : RepositorioBase, IRepositorio_Inmueble
    {
        public Repositorio_InmuebleMySQL(IConfiguration configuration) : base(configuration) { }

        // CREACIÓN, MODIFICACIÓN y ELIMINACIÓN
        public int Alta(Inmueble i)
        {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Inmuebles
                    (direccion, cupo, latitud, longitud, precio_dia, porcentaje_seña, estado, id_propietario, id_tipo)
                    VALUES (@direccion, @cupo, @latitud, @longitud, @precio_dia, @porcentaje_seña, @estado, @id_propietario, @id_tipo);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@cupo", i.Cupo);
                    command.Parameters.AddWithValue("@latitud", i.Latitud);
                    command.Parameters.AddWithValue("@longitud", i.Longitud);
                    command.Parameters.AddWithValue("@precio_dia", i.Precio_Dia);
                    command.Parameters.AddWithValue("@porcentaje_seña", i.Porcentaje_Seña);
                    command.Parameters.AddWithValue("@estado", i.Estado);
                    command.Parameters.AddWithValue("@id_propietario", i.ID_Propietario);
                    command.Parameters.AddWithValue("@id_tipo", i.ID_Tipo);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    i.id = respuesta;
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Modificacion(Inmueble i)
        {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Inmuebles
                    SET direccion = @direccion,
                        cupo = @cupo,
                        latitud = @latitud,
                        longitud = @longitud,
                        precio_dia = @precio_dia,
                        porcentaje_seña = @porcentaje_seña,
                        estado = @estado,
                        id_propietario = @id_propietario,
                        id_tipo = @id_tipo
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@direccion", i.Direccion);
                    command.Parameters.AddWithValue("@cupo", i.Cupo);
                    command.Parameters.AddWithValue("@latitud", i.Latitud);
                    command.Parameters.AddWithValue("@longitud", i.Longitud);
                    command.Parameters.AddWithValue("@precio_dia", i.Precio_Dia);
                    command.Parameters.AddWithValue("@porcentaje_seña", i.Porcentaje_Seña);
                    command.Parameters.AddWithValue("@estado", i.Estado);
                    command.Parameters.AddWithValue("@id_propietario", i.ID_Propietario);
                    command.Parameters.AddWithValue("@id_tipo", i.ID_Tipo);
                    command.Parameters.AddWithValue("@id", i.id);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Baja(int id)
        {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM Inmuebles
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        // OBTENER TODOS
        public IList<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT i.*, p.id AS PropietarioID, p.Nombre AS PropietarioNombre, p.Apellido, ti.*
                    FROM Inmuebles i
                    JOIN Propietarios p ON i.id_propietario = p.id
                    JOIN Tipo_Inmuebles ti ON i.id_tipo = ti.id
                    ORDER BY Direccion ASC
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Inmueble inmueble = new Inmueble
                            {
                                id = reader.GetInt32("id"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio_Dia = reader.GetDecimal("Precio_Dia"),
                                Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                                Estado = reader.GetString("Estado"),
                                ID_Propietario = reader.GetInt32("ID_Propietario"),
                                ID_Tipo = reader.GetInt32("ID_Tipo"),

                                Propietario = new Propietario
                                {
                                    id = reader.GetInt32("PropietarioID"),
                                    Nombre = reader.GetString("PropietarioNombre"),
                                    Apellido = reader.GetString("Apellido")
                                },

                                Tipo_Inmueble = new Tipo_Inmueble
                                {
                                    id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Descripcion = reader.GetString("Descripcion")
                                }
                            };

                            inmuebles.Add(inmueble);
                        }
                    }
                    connection.Close();
                }
            }

            return inmuebles;
        }

        // OBTENER LOS MÍOS
        public IList<Inmueble> MisInmuebles(int id)
        {
            var inmuebles = new List<Inmueble>();

            string sql = @"
                SELECT 
                    i.id AS InmuebleID, i.Direccion, i.Cupo, i.Latitud, i.Longitud, 
                    i.Precio_Dia, i.Porcentaje_Seña, i.Estado, i.id_propietario, i.id_tipo, 
                    p.id AS PropietarioID, p.Nombre AS PropietarioNombre, p.Apellido, 
                    ti.id AS TipoID, ti.Nombre AS TipoNombre, ti.Descripcion AS TipoDescripcion 
                FROM Inmuebles i 
                JOIN Propietarios p ON i.id_propietario = p.id 
                JOIN Tipo_Inmuebles ti ON i.id_tipo = ti.id 
                WHERE i.id_propietario = @id 
                ORDER BY i.Direccion ASC;
            ";

            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            id = reader.GetInt32("InmuebleID"),
                            Direccion = reader.GetString("Direccion"),
                            Cupo = reader.GetInt32("Cupo"),
                            Latitud = reader.GetDecimal("Latitud"),
                            Longitud = reader.GetDecimal("Longitud"),
                            Precio_Dia = reader.GetDecimal("Precio_Dia"),
                            Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                            Estado = reader.GetString("Estado"),
                            ID_Propietario = reader.GetInt32("id_propietario"),
                            ID_Tipo = reader.GetInt32("id_tipo"),

                            Propietario = new Propietario
                            {
                                id = reader.GetInt32("PropietarioID"),
                                Nombre = reader.GetString("PropietarioNombre"),
                                Apellido = reader.GetString("Apellido")
                            },
                            Tipo_Inmueble = new Tipo_Inmueble
                            {
                                id = reader.GetInt32("TipoID"),
                                Nombre = reader.GetString("TipoNombre"),
                                Descripcion = reader.GetString("TipoDescripcion")
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
                connection.Close();
            }

            return inmuebles;
        }

        // OBTENER POR ATRIBUTO (Dirección, ID)
        public IList<Inmueble> ObtenerFiltrados(
            string? estado,
            int pagina = 1,
            int tamPagina = 10,
            int? idPropietario = null
        ){
            if (estado != null && estado != "0" && estado != "1")
                throw new ArgumentException("Estado inválido.");

            pagina = Math.Max(1, pagina);
            tamPagina = Math.Clamp(tamPagina, 1, 100);

            var inmuebles = new List<Inmueble>();

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT
                    i.*,
                    p.id AS PropietarioID,
                    p.Nombre AS PropietarioNombre,
                    p.Apellido AS PropietarioApellido,
                    ti.id AS TipoID,
                    ti.Nombre AS TipoNombre,
                    ti.Descripcion AS TipoDescripcion
                FROM Inmuebles i
                JOIN Propietarios p ON i.id_propietario = p.id
                JOIN Tipo_Inmuebles ti ON i.id_tipo = ti.id
                WHERE (@estado IS NULL OR i.estado = @estado)
                AND (@idPropietario IS NULL OR i.id_propietario = @idPropietario)
                ORDER BY i.direccion, i.id
                LIMIT @limite OFFSET @offset;
            ";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@estado", (object?)estado ?? DBNull.Value
            );
            command.Parameters.AddWithValue(
                "@idPropietario",
                (object?)idPropietario ?? DBNull.Value
            );
            command.Parameters.AddWithValue("@limite", tamPagina);
            command.Parameters.AddWithValue(
                "@offset", ((long)pagina - 1) * tamPagina
            );

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                inmuebles.Add(new Inmueble
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
                    ID_Tipo = reader.GetInt32("id_tipo"),

                    Propietario = new Propietario
                    {
                        id = reader.GetInt32("PropietarioID"),
                        Nombre = reader.GetString("PropietarioNombre"),
                        Apellido = reader.GetString("PropietarioApellido")
                    },

                    Tipo_Inmueble = new Tipo_Inmueble
                    {
                        id = reader.GetInt32("TipoID"),
                        Nombre = reader.GetString("TipoNombre"),
                        Descripcion = reader.GetString("TipoDescripcion")
                    }
                });
            }

            return inmuebles;
        }

        public int ContarFiltrados(string? estado, int? idPropietario = null)
        {
            if (estado != null && estado != "0" && estado != "1")
                throw new ArgumentException("Estado inválido.");

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT COUNT(*)
                FROM Inmuebles i
                JOIN Propietarios p ON i.id_propietario = p.id
                JOIN Tipo_Inmuebles ti ON i.id_tipo = ti.id
                WHERE (@estado IS NULL OR i.estado = @estado)
                AND (@idPropietario IS NULL OR i.id_propietario = @idPropietario)
            ";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@estado", (object?)estado ?? DBNull.Value
            );

            command.Parameters.AddWithValue(
                "@idPropietario",
                (object?)idPropietario ?? DBNull.Value
            );

            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        // OBTENER POR ATRIBUTO (Dirección, ID)
        public IList<Inmueble> ObtenerPorDireccion(string direccion)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT i.*, p.id AS PropietarioID, p.Nombre AS PropietarioNombre, p.Apellido, ti.*
                    FROM Inmuebles i
                    JOIN Propietarios p ON i.id_propietario = p.id
                    JOIN Tipo_Inmuebles ti ON i.id_tipo = ti.id
                    WHERE Direccion LIKE @direccion
                    ORDER BY Direccion ASC;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", $"%{direccion}%");

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {
                                id = reader.GetInt32("id"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio_Dia = reader.GetDecimal("Precio_Dia"),
                                Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                                Estado = reader.GetString("Estado"),
                                ID_Propietario = reader.GetInt32("ID_Propietario"),
                                ID_Tipo = reader.GetInt32("ID_Tipo"),

                                Propietario = new Propietario
                                {
                                    id = reader.GetInt32("PropietarioID"),
                                    Nombre = reader.GetString("PropietarioNombre"),
                                    Apellido = reader.GetString("Apellido")
                                },

                                Tipo_Inmueble = new Tipo_Inmueble
                                {
                                    id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Descripcion = reader.GetString("Descripcion")
                                }
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return inmuebles;
        }

        public Inmueble? ObtenerPorID(int id)
        {
            Inmueble? inmueble = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT i.*, p.id AS PropietarioID, p.Nombre AS PropietarioNombre, p.Apellido, ti.*
                    FROM Inmuebles i
                    JOIN Propietarios p ON i.id_propietario = p.id
                    JOIN Tipo_Inmuebles ti ON i.id_tipo = ti.id
                    WHERE i.id = @id
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                id = reader.GetInt32("id"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio_Dia = reader.GetDecimal("Precio_Dia"),
                                Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                                Estado = reader.GetString("Estado"),
                                ID_Propietario = reader.GetInt32("ID_Propietario"),
                                ID_Tipo = reader.GetInt32("ID_Tipo"),

                                Propietario = new Propietario
                                {
                                    id = reader.GetInt32("PropietarioID"),
                                    Nombre = reader.GetString("PropietarioNombre"),
                                    Apellido = reader.GetString("Apellido")
                                },

                                Tipo_Inmueble = new Tipo_Inmueble
                                {
                                    id = reader.GetInt32("id"),
                                    Nombre = reader.GetString("Nombre"),
                                    Descripcion = reader.GetString("Descripcion")
                                }
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return inmueble;
        }

        public IList<InmuebleMasReservadoViewModel> ObtenerMasReservados(
            DateTime desde,
            DateTime hasta,
            int pagina = 1,
            int tamPagina = 10
        )
        {
            pagina = Math.Max(1, pagina);
            tamPagina = Math.Clamp(tamPagina, 1, 100);

            var resultados = new List<InmuebleMasReservadoViewModel>();

            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT
                    i.id,
                    i.direccion,
                    p.Nombre AS PropietarioNombre,
                    p.Apellido AS PropietarioApellido,
                    COUNT(*) AS CantidadReservas
                FROM Reservas r
                JOIN Inmuebles i ON r.id_inmueble = i.id
                JOIN Propietarios p ON i.id_propietario = p.id
                WHERE r.fecha_creacion >= @desde
                AND r.fecha_creacion <= @hasta
                GROUP BY i.id, i.direccion, p.Nombre, p.Apellido
                ORDER BY CantidadReservas DESC, i.direccion, i.id
                LIMIT @limite OFFSET @offset;
            ";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@desde", desde);
            command.Parameters.AddWithValue("@hasta", hasta);
            command.Parameters.AddWithValue("@limite", tamPagina);
            command.Parameters.AddWithValue(
                "@offset", ((long)pagina - 1) * tamPagina
            );

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                resultados.Add(new InmuebleMasReservadoViewModel
                {
                    ID_Inmueble = reader.GetInt32("id"),
                    Direccion = reader.GetString("direccion"),
                    Propietario =
                        reader.GetString("PropietarioApellido") + " " +
                        reader.GetString("PropietarioNombre"),
                    CantidadReservas = Convert.ToInt32(
                        reader["CantidadReservas"]
                    )
                });
            }

            return resultados;
        }

        public int ContarMasReservados(DateTime desde, DateTime hasta)
        {
            using var connection = new MySqlConnection(connectionString);

            string sql = @"
                SELECT COUNT(DISTINCT i.id)
                FROM Reservas r
                JOIN Inmuebles i ON r.id_inmueble = i.id
                JOIN Propietarios p ON i.id_propietario = p.id
                WHERE r.fecha_creacion >= @desde
                AND r.fecha_creacion <= @hasta;
            ";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@desde", desde);
            command.Parameters.AddWithValue("@hasta", hasta);

            connection.Open();

            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}