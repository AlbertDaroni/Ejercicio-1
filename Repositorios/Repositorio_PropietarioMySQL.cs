using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_PropietarioMySQL : RepositorioBase, IRepositorio_Propietario {
        public Repositorio_PropietarioMySQL(IConfiguration configuration) : base(configuration) {
            //https://www.nuget.org/packages/MySql.Data/
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }

        // CREACIÓN, MODIFICACIÓN y ELIMINACIÓN
        public int Alta(Propietario p) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Propietarios
                    (Nombre, Apellido, DNI, Telefono, Correo)
                    VALUES
                    (@nombre, @apellido, @dni, @telefono, @correo);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.DNI);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@correo", p.Correo);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    p.id = respuesta;
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Modificacion(Propietario p) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Propietarios
                    SET Nombre = @nombre,
                        Apellido = @apellido,
                        DNI = @dni,
                        Telefono = @telefono,
                        Correo = @correo
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.DNI);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@correo", p.Correo);
                    command.Parameters.AddWithValue("@id", p.id);

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
                    DELETE FROM Propietarios
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
        public IList<Propietario> ObtenerTodos() {
            var propietarios = new List<Propietario>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Propietarios
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Propietario propietario = new Propietario {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("DNI"),
                                Telefono = reader.GetString("Telefono"),
                                Correo = reader.GetString("Correo")
                            };

                            propietarios.Add(propietario);
                        }
                    }
                    connection.Close();
                }
            }

            return propietarios;
        }

        // OBTENER POR ATRIBUTO (Email, Nombre, ID)
        public Propietario? ObtenerPorEmail(string email) {
            Propietario? propietario = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Propietarios
                    WHERE Correo = @correo;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@correo", email);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            propietario = new Propietario {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("DNI"),
                                Telefono = reader.GetString("Telefono"),
                                Correo = reader.GetString("Correo")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return propietario;
        }

        public IList<Propietario> BuscarPorNombre(string nombre) {
            var propietarios = new List<Propietario>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Propietarios
                    WHERE Nombre LIKE @nombre
                    OR Apellido LIKE @nombre;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@nombre", $"%{nombre}%");

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            propietarios.Add(new Propietario {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("DNI"),
                                Telefono = reader.GetString("Telefono"),
                                Correo = reader.GetString("Correo")
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return propietarios;
        }

        public Propietario? ObtenerPorId(int id) {
            Propietario? propietario = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Propietarios
                    WHERE id = @id
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            propietario = new Propietario {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("DNI"),
                                Telefono = reader.GetString("Telefono"),
                                Correo = reader.GetString("Correo")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return propietario;
        }
    }
}