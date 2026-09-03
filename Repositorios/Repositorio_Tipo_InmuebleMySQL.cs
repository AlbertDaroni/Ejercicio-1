using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_Tipo_InmuebleMySQL : RepositorioBase, IRepositorio_Tipo_Inmueble {
        public Repositorio_Tipo_InmuebleMySQL(IConfiguration configuration) : base(configuration) {}

        // ==========================================
        // ALTA
        // ==========================================
        public int Alta(Tipo_Inmueble tipo_Inmueble) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Tipo_Inmuebles (nombre, descripcion)
                    VALUES (@nombre, @descripcion);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre",tipo_Inmueble.Nombre);
                    command.Parameters.AddWithValue("@descripcion",tipo_Inmueble.Descripcion);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    tipo_Inmueble.id = respuesta;
                }
            }

            return respuesta;
        }

        // ==========================================
        // BAJA
        // ==========================================
        public int Baja(int id) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    DELETE FROM Tipo_Inmuebles
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

        // ==========================================
        // MODIFICACIÓN
        // ==========================================
        public int Modificacion(Tipo_Inmueble tipo_Inmueble) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Tipo_Inmuebles
                    SET nombre = @nombre,
                        descripcion = @descripcion
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", tipo_Inmueble.Nombre);
                    command.Parameters.AddWithValue("@descripcion", tipo_Inmueble.Descripcion);
                    command.Parameters.AddWithValue("@id", tipo_Inmueble.id);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        // ==========================================
        // OBTENER TODOS
        // ==========================================
        public IList<Tipo_Inmueble> ObtenerTodos() {
            var tipos = new List<Tipo_Inmueble>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT id, nombre, descripcion
                    FROM Tipo_Inmuebles
                    ORDER BY nombre;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var tipo = new Tipo_Inmueble {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.GetString("descripcion")
                            };

                            tipos.Add(tipo);
                        }
                    }
                }
            }

            return tipos;
        }

        // ==========================================
        // OBTENER POR ID
        // ==========================================
        public Tipo_Inmueble? ObtenerPorID(int id) {
            Tipo_Inmueble? tipo = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT id, nombre, descripcion
                    FROM Tipo_Inmuebles
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            tipo = new Tipo_Inmueble {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.GetString("descripcion")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return tipo;
        }

        // ==========================================
        // BUSCAR POR NOMBRE
        // ==========================================
        public IList<Tipo_Inmueble> ObtenerPorNombre(string nombre) {
            var tipos = new List<Tipo_Inmueble>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT id, nombre, descripcion
                    FROM Tipo_Inmuebles
                    WHERE nombre LIKE @nombre
                    ORDER BY nombre;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var tipo = new Tipo_Inmueble {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.GetString("descripcion")
                            };

                            tipos.Add(tipo);
                        }
                    }
                    connection.Close();
                }
            }

            return tipos;
        }
    }
}