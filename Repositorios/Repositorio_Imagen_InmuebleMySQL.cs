using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_Imagen_InmuebleMySQL : RepositorioBase, IRepositorio_Imagen_Inmueble {
        public Repositorio_Imagen_InmuebleMySQL(IConfiguration configuration) : base(configuration) {
            //https://www.nuget.org/packages/MySql.Data/
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }

        // CREACIÓN, MODIFICACIÓN y ELIMINACIÓN
        public int Alta(Imagen_Inmueble i) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Imagen_Inmuebles
                    (url, esPortada, orden, id_inmueble)
                    VALUES (@url, @esPortada, @orden, @id_inmueble);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@url", i.URL);
                    command.Parameters.AddWithValue("@esPortada", i.EsPortada);
                    command.Parameters.AddWithValue("@orden", i.Orden);
                    command.Parameters.AddWithValue("@id_inmueble", i.ID_Inmueble);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    i.id = respuesta;
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Modificacion(Imagen_Inmueble i) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Imagen_Inmuebles
                    SET url = @url,
                        esPortada = @esPortada,
                        orden = @orden,
                        id_inmueble = @id_inmueble
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@url", i.URL);
                    command.Parameters.AddWithValue("@esPortada", i.EsPortada);
                    command.Parameters.AddWithValue("@orden", i.Orden);
                    command.Parameters.AddWithValue("@id_inmueble", i.ID_Inmueble);

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
                    DELETE FROM Imagen_Inmueble
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
        public IList<Imagen_Inmueble> ObtenerTodos() {
            var imagenes_inmuebles = new List<Imagen_Inmueble>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Imagen_Inmuebles
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Imagen_Inmueble imagen_inmueble = new Imagen_Inmueble {
                                id = reader.GetInt32("id"),
                                URL = reader.GetString("URL"),
                                EsPortada = reader.GetInt32("EsPortada"),
                                Orden = reader.GetInt32("Orden"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble")
                            };
                            imagenes_inmuebles.Add(imagen_inmueble);
                        }
                    }
                    connection.Close();
                }
            }

            return imagenes_inmuebles;
        }

        // OBTENER POR ATRIBUTO (ID)
        public Imagen_Inmueble? ObtenerPorID(int id) {
            Imagen_Inmueble? imagen_inmueble = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Imagen_Inmueble
                    WHERE id = @id
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            imagen_inmueble = new Imagen_Inmueble {
                                id = reader.GetInt32("id"),
                                URL = reader.GetString("URL"),
                                EsPortada = reader.GetInt32("EsPortada"),
                                Orden = reader.GetInt32("Orden"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return imagen_inmueble;
        }
    }
}