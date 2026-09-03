using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_InmuebleMySQL : RepositorioBase, IRepositorio_Inmueble {
        public Repositorio_InmuebleMySQL(IConfiguration configuration) : base(configuration) {
            //https://www.nuget.org/packages/MySql.Data/
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }

        // CREACIÓN, MODIFICACIÓN y ELIMINACIÓN
        public int Alta(Inmueble i) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Inmuebles
                    (direccion, cupo, latitud, longitud, precio_dia, porcentaje_seña, estado, id_propietario, id_tipo)
                    VALUES (@direccion, @cupo, @latitud, @longitud, @precio_dia, @porcentaje_seña, @estado, @id_propietario, @id_tipo);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
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

        public int Modificacion(Inmueble i) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
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

                using (var command = new MySqlCommand(sql, connection)) {
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

        public int Baja(int id) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    DELETE FROM Inmuebles
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
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
        public IList<Inmueble> ObtenerTodos() {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inmuebles
                    ORDER BY Direccion ASC
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Inmueble inmueble = new Inmueble {
                                id = reader.GetInt32("id"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio_Dia = reader.GetDecimal("Precio_Dia"),
                                Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                                Estado = reader.GetString("Estado"),
                                ID_Propietario = reader.GetInt32("ID_Propietario"),
                                ID_Tipo = reader.GetInt32("ID_Tipo")
                            };
                            inmuebles.Add(inmueble);
                        }
                    }
                    connection.Close();
                }
            }

            return inmuebles;
        }

        // OBTENER POR ATRIBUTO (Dirección, ID)
        public IList<Inmueble> ObtenerPorDireccion(string direccion) {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inmuebles
                    WHERE Direccion LIKE @direccion
                    ORDER BY Direccion ASC;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@direccion", $"%{direccion}%");

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            inmuebles.Add(new Inmueble {
                                id = reader.GetInt32("id"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio_Dia = reader.GetDecimal("Precio_Dia"),
                                Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                                Estado = reader.GetString("Estado"),
                                ID_Propietario = reader.GetInt32("ID_Propietario"),
                                ID_Tipo = reader.GetInt32("ID_Tipo")
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return inmuebles;
        }

        public Inmueble? ObtenerPorID(int id) {
            Inmueble? inmueble = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inmuebles
                    WHERE id = @id
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            inmueble = new Inmueble {
                                id = reader.GetInt32("id"),
                                Direccion = reader.GetString("Direccion"),
                                Cupo = reader.GetInt32("Cupo"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Precio_Dia = reader.GetDecimal("Precio_Dia"),
                                Porcentaje_Seña = reader.GetDecimal("Porcentaje_Seña"),
                                Estado = reader.GetString("Estado"),
                                ID_Propietario = reader.GetInt32("ID_Propietario"),
                                ID_Tipo = reader.GetInt32("ID_Tipo")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return inmueble;
        }
    }
}