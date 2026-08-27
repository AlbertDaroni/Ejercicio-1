using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class RepositorioInquilinoMySql : RepositorioBase, IRepositorioInquilino {
        public RepositorioInquilinoMySql(IConfiguration configuration) : base(configuration) {
            //https://www.nuget.org/packages/MySql.Data/
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }

        // CREACIÓN, MODIFICACIÓN y ELIMINACIÓN
        public int Alta(Inquilino i) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Inquilinos
                    (Nombre, Apellido, DNI, Telefono, Correo)
                    VALUES
                    (@nombre, @apellido, @dni, @telefono, @correo);

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.DNI);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@correo", i.Correo);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    i.id = respuesta;
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Modificacion(Inquilino i) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Inquilinos
                    SET Nombre = @nombre,
                        Apellido = @apellido,
                        DNI = @dni,
                        Telefono = @telefono,
                        Correo = @correo
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.DNI);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@correo", i.Correo);
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
                    DELETE FROM Inquilinos
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
        public IList<Inquilino> ObtenerTodos() {
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inquilinos
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Inquilino inquilino = new Inquilino {
                                id = reader.GetInt32("id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                DNI = reader.GetString("DNI"),
                                Telefono = reader.GetString("Telefono"),
                                Correo = reader.GetString("Correo")
                            };

                            inquilinos.Add(inquilino);
                        }
                    }
                }
            }

            return inquilinos;
        }

        // OBTENER POR ATRIBUTO (Email, Nombre, ID)
        public Inquilino? ObtenerPorEmail(string email) {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inquilinos
                    WHERE Correo = @correo;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@correo", email);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            inquilino = new Inquilino {
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

            return inquilino;
        }

        public IList<Inquilino> BuscarPorNombre(string nombre) {
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inquilinos
                    WHERE Nombre LIKE @nombre
                    OR Apellido LIKE @nombre;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@nombre", $"%{nombre}%");

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            inquilinos.Add(new Inquilino {
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

            return inquilinos;
        }

        public Inquilino? ObtenerPorId(int id) {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT *
                    FROM Inquilinos
                    WHERE id = @id
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            inquilino = new Inquilino {
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

            return inquilino;
        }
    }
}