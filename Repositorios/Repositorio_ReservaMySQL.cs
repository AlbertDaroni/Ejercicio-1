using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_ReservaMySQL : RepositorioBase, IRepositorio_Reserva {
        public Repositorio_ReservaMySQL(IConfiguration configuration) : base(configuration) {
            //https://www.nuget.org/packages/MySql.Data/
            //https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/
        }

        public int Alta(Reserva r)
        {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Reservas (
                        fecha_creacion, fecha_inicio, fecha_fin_original,
                        fecha_fin_efectiva, monto_dia, multa,
                        id_inquilino, id_inmueble, id_usuario_creador, id_usuario_finalizador
                    )
                    VALUES (
                        @fecha_creacion, @fecha_inicio, @fecha_fin_original,
                        @fecha_fin_efectiva, @monto_dia, @multa,
                        @id_inquilino, @id_inmueble, @id_usuario_creador, @id_usuario_finalizador
                    );

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@fecha_creacion", r.Fecha_Creacion);
                    command.Parameters.AddWithValue("@fecha_inicio", r.Fecha_Inicio);
                    command.Parameters.AddWithValue("@fecha_fin_original", r.Fecha_Fin_Original);
                    command.Parameters.AddWithValue("@fecha_fin_efectiva", r.Fecha_Fin_Efectiva);
                    command.Parameters.AddWithValue("@monto_dia", r.Monto_Dia);
                    command.Parameters.AddWithValue("@multa", r.Multa);
                    command.Parameters.AddWithValue("@id_inquilino", r.ID_Inquilino);
                    command.Parameters.AddWithValue("@id_inmueble", r.ID_Inmueble);
                    command.Parameters.AddWithValue("@id_usuario_creador", r.ID_Usuario_Creador);
                    command.Parameters.AddWithValue("@id_usuario_finalizador", r.ID_Usuario_Finalizador);

                    connection.Open();

                    respuesta = Convert.ToInt32(command.ExecuteScalar());

                    r.id = respuesta;

                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Modificacion(Reserva r)
        {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Reservas
                    SET fecha_creacion = @fecha_creacion,
                        fecha_inicio = @fecha_inicio,
                        fecha_fin_original = @fecha_fin_original,
                        fecha_fin_efectiva = @fecha_fin_efectiva,
                        monto_dia = @monto_dia,
                        multa = @multa,
                        id_inquilino = @id_inquilino,
                        id_inmueble = @id_inmueble,
                        id_usuario_creador = @id_usuario_creador,
                        id_usuario_finalizador = @id_usuario_finalizador
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@fecha_creacion", r.Fecha_Creacion);
                    command.Parameters.AddWithValue("@fecha_inicio", r.Fecha_Inicio);
                    command.Parameters.AddWithValue("@fecha_fin_original", r.Fecha_Fin_Original);
                    command.Parameters.AddWithValue("@fecha_fin_efectiva", r.Fecha_Fin_Efectiva);
                    command.Parameters.AddWithValue("@monto_dia", r.Monto_Dia);
                    command.Parameters.AddWithValue("@multa", r.Multa);
                    command.Parameters.AddWithValue("@id_inquilino", r.ID_Inquilino);
                    command.Parameters.AddWithValue("@id_inmueble", r.ID_Inmueble);
                    command.Parameters.AddWithValue("@id_usuario_creador", r.ID_Usuario_Creador);
                    command.Parameters.AddWithValue("@id_usuario_finalizador", r.ID_Usuario_Finalizador);

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
                    DELETE FROM Reservas
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

        public IList<Reserva> ObtenerTodos()
        {
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT *
                    FROM Reservas
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reserva reserva = new Reserva
                            {
                                id = reader.GetInt32("id"),
                                Fecha_Creacion = reader.GetDateTime("Fecha_Creacion"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin_Original = reader.GetDateTime("Fecha_Fin_Original"),
                                Fecha_Fin_Efectiva = reader.GetDateTime("Fecha_Fin_Efectiva"),
                                Monto_Dia = reader.GetDecimal("Monto_Dia"),
                                Multa = reader.GetDecimal("Multa"),
                                ID_Inquilino = reader.GetInt32("ID_Inquilino"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble"),
                                ID_Usuario_Creador = reader.GetInt32("ID_Usuario_Creador"),
                                ID_Usuario_Finalizador = reader.GetInt32("ID_Usuario_Finalizador")
                            };

                            reservas.Add(reserva);
                        }
                    }

                    connection.Close();
                }
            }

            return reservas;
        }

        public Reserva? ObtenerPorID(int id)
        {
            Reserva? reserva = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT *
                    FROM Reservas
                    WHERE id = @id
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reserva = new Reserva
                            {
                                id = reader.GetInt32("id"),
                                Fecha_Creacion = reader.GetDateTime("Fecha_Creacion"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin_Original = reader.GetDateTime("Fecha_Fin_Original"),
                                Fecha_Fin_Efectiva = reader.GetDateTime("Fecha_Fin_Efectiva"),
                                Monto_Dia = reader.GetDecimal("Monto_Dia"),
                                Multa = reader.GetDecimal("Multa"),
                                ID_Inquilino = reader.GetInt32("ID_Inquilino"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble"),
                                ID_Usuario_Creador = reader.GetInt32("ID_Usuario_Creador"),
                                ID_Usuario_Finalizador = reader.GetInt32("ID_Usuario_Finalizador")
                            };
                        }
                    }

                    connection.Close();
                }
            }

            return reserva;
        }
    }
}