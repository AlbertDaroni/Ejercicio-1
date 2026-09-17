using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public class Repositorio_PagoMySQL : RepositorioBase, IRepositorio_Pago {
        public Repositorio_PagoMySQL(IConfiguration configuration) : base(configuration) {}

        
        public int Alta(Pago pago) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO Pagos
                    (
                        concepto,
                        fecha_pago,
                        fecha_anulacion,
                        importe,
                        estado,
                        id_reserva,
                        id_usuario_creador,
                        id_usuario_finalizador
                    )
                    VALUES
                    (
                        @concepto,
                        @fecha_pago,
                        @fecha_anulacion,
                        @importe,
                        @estado,
                        @id_reserva,
                        @id_usuario_creador,
                        @id_usuario_finalizador
                    );

                    SELECT LAST_INSERT_ID();
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@fecha_pago", pago.Fecha_Pago);
                    command.Parameters.AddWithValue("@fecha_anulacion", pago.Fecha_Anulacion.HasValue ? pago.Fecha_Anulacion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@estado", pago.Estado);
                    command.Parameters.AddWithValue("@id_reserva", pago.ID_Reserva);
                    command.Parameters.AddWithValue("@id_usuario_creador", pago.ID_Usuario_Creador.HasValue ? pago.ID_Usuario_Creador.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@id_usuario_finalizador", pago.ID_Usuario_Finalizador.HasValue ? pago.ID_Usuario_Finalizador.Value : DBNull.Value);

                    connection.Open();
                    respuesta = Convert.ToInt32(command.ExecuteScalar());
                    pago.id = respuesta;
                    connection.Close();
                }
            }

            return respuesta;
        }
       
        public int Modificacion(Pago pago) {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    UPDATE Pagos
                    SET
                        concepto = @concepto,
                        fecha_pago = @fecha_pago,
                        fecha_anulacion = @fecha_anulacion,
                        importe = @importe,
                        estado = @estado,
                        id_reserva = @id_reserva,
                        id_usuario_creador = @id_usuario_creador,
                        id_usuario_finalizador = @id_usuario_finalizador
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@fecha_pago", pago.Fecha_Pago);
                    command.Parameters.AddWithValue("@fecha_anulacion", pago.Fecha_Anulacion.HasValue ? pago.Fecha_Anulacion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@estado", pago.Estado);
                    command.Parameters.AddWithValue("@id_reserva", pago.ID_Reserva);
                    command.Parameters.AddWithValue("@id_usuario_creador", pago.ID_Usuario_Creador.HasValue ? pago.ID_Usuario_Creador.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@id_usuario_finalizador", pago.ID_Usuario_Finalizador.HasValue ? pago.ID_Usuario_Finalizador.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@id", pago.id);

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
                    DELETE FROM Pagos
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
        
        public IList<Pago> ObtenerTodos() {
            var pagos = new List<Pago>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT
                        id,
                        concepto,
                        fecha_pago,
                        fecha_anulacion,
                        importe,
                        estado,
                        id_reserva,
                        id_usuario_creador,
                        id_usuario_finalizador
                    FROM Pagos
                    ORDER BY fecha_pago DESC;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) { pagos.Add(MapearPago(reader)); }
                    }
                    connection.Close();
                }
            }

            return pagos;
        }
        
        public Pago? ObtenerPorID(int id) {
            Pago? pago = null;

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"SELECT *
                    FROM Pagos
                    WHERE id = @id
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            pago = new Pago {
                                id = reader.GetInt32("id"),
                                Concepto = reader.GetString("concepto"),
                                Fecha_Pago = reader.GetDateTime("fecha_pago"),
                                Fecha_Anulacion = reader.IsDBNull(reader.GetOrdinal("fecha_anulacion"))
                                    ? null
                                    : reader.GetDateTime("fecha_anulacion"),
                                Importe = reader.GetDecimal("importe"),
                                Estado = reader.GetString("estado"),
                                ID_Reserva = reader.GetInt32("id_reserva"),
                                ID_Usuario_Creador = reader.IsDBNull(reader.GetOrdinal("id_usuario_creador"))
                                    ? null
                                    : reader.GetInt32("id_usuario_creador"),
                                ID_Usuario_Finalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador"))
                                    ? null
                                    : reader.GetInt32("id_usuario_finalizador")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return pago;
        }
        
        public IList<Pago> ObtenerPorReserva(int idReserva) {
            var pagos = new List<Pago>();

            using (var connection = new MySqlConnection(connectionString)) {
                string sql = @"
                    SELECT
                        id,
                        concepto,
                        fecha_pago,
                        fecha_anulacion,
                        importe,
                        estado,
                        id_reserva,
                        id_usuario_creador,
                        id_usuario_finalizador
                    FROM Pagos
                    WHERE id_reserva = @id_reserva
                    ORDER BY fecha_pago DESC;
                ";

                using (var command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@id_reserva", idReserva);

                    connection.Open();
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) { pagos.Add(MapearPago(reader)); }
                    }
                    connection.Close();
                }
            }

            return pagos;
        }
        
        // MAPEO DE MYSQL -> PAGO 
        // Para evitar repeticiones de los metodos obtener, para evitar eso hacemos pagos.Add(MapearPago(reader));
        private Pago MapearPago(MySqlDataReader reader) {
            return new Pago {
                id = reader.GetInt32("id"),
                Concepto = reader.GetString("concepto"),
                Fecha_Pago = reader.GetDateTime("fecha_pago"),
                Fecha_Anulacion = reader.IsDBNull(reader.GetOrdinal("fecha_anulacion")) ? null : reader.GetDateTime("fecha_anulacion"),
                Importe = reader.GetDecimal("importe"),
                Estado = reader.GetString("estado"),
                ID_Reserva = reader.GetInt32("id_reserva"),
                ID_Usuario_Creador = reader.IsDBNull(reader.GetOrdinal("id_usuario_creador")) ? null : reader.GetInt32("id_usuario_creador"),
                ID_Usuario_Finalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador")) ? null : reader.GetInt32("id_usuario_finalizador")
            };
        }
    }
}