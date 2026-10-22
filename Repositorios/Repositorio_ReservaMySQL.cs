using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios
{
    public class Repositorio_ReservaMySQL : RepositorioBase, IRepositorio_Reserva
    {
        public Repositorio_ReservaMySQL(IConfiguration configuration) : base(configuration) { }

        public int Alta(Reserva r)
        {
            int respuesta = -1;
            r.Fecha_Creacion = DateTime.Now;
            r.Estado = "1";

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    
                    INSERT INTO Reservas (
                        fecha_creacion, fecha_inicio, fecha_fin_original,
                        fecha_fin_efectiva, monto_dia, multa, estado,
                        id_inquilino, id_inmueble, id_usuario_creador, id_usuario_finalizador
                    )
                    VALUES (
                        @fecha_creacion, @fecha_inicio, @fecha_fin_original,
                        @fecha_fin_efectiva, @monto_dia, @multa, @estado,
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
                    command.Parameters.AddWithValue("@estado", r.Estado);

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
                    command.Parameters.AddWithValue("@id", r.id);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int Baja(int id, int idUsuario)
        {
            int respuesta = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE Reservas
                    SET estado = 0, id_usuario_finalizador = @idUsuario
                    WHERE id = @id;
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        public int FinalizarConPago(
    int idReserva,
    DateTime fechaEfectiva,
    int idUsuario,
    bool confirmarPagoMulta
)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            // Bloqueamos la reserva para evitar dos finalizaciones simultáneas.
            DateTime inicio;
            DateTime finOriginal;
            decimal montoDia;
            string estado;

            using (var consultar = new MySqlCommand(@"
        SELECT fecha_inicio, fecha_fin_original, monto_dia, estado
        FROM Reservas
        WHERE id = @id
        FOR UPDATE;
    ", connection, transaction))
            {
                consultar.Parameters.AddWithValue("@id", idReserva);

                using var reader = consultar.ExecuteReader();

                if (!reader.Read())
                    throw new InvalidOperationException("La reserva no existe.");

                inicio = reader.GetDateTime("fecha_inicio");
                finOriginal = reader.GetDateTime("fecha_fin_original");
                montoDia = reader.GetDecimal("monto_dia");
                estado = reader.GetString("estado");
            }

            if (estado != "1")
                throw new InvalidOperationException(
                    "La reserva ya está finalizada."
                );

            if (finOriginal <= inicio || montoDia <= 0m)
                throw new InvalidOperationException(
                    "La reserva tiene una duración o un monto diario inválido."
                );

            if (fechaEfectiva < inicio || fechaEfectiva > finOriginal)
                throw new InvalidOperationException(
                    "La fecha efectiva debe estar entre el inicio y el fin original."
                );

            decimal duracion = (decimal)(finOriginal - inicio).TotalDays;
            decimal cumplido = (decimal)(fechaEfectiva - inicio).TotalDays;
            decimal restante = (decimal)(finOriginal - fechaEfectiva).TotalDays;

            decimal porcentaje = cumplido < duracion / 2m ? 0.50m : 0.25m;

            decimal multa = Math.Round(
                restante * montoDia * porcentaje,
                2,
                MidpointRounding.AwayFromZero
            );

            if (multa > 0m && !confirmarPagoMulta)
                throw new InvalidOperationException(
                    "Debés confirmar el cobro de la multa para finalizar."
                );

            DateTime ahora = DateTime.Now;

            if (multa > 0m)
            {
                using var registrarPago = new MySqlCommand(@"
                    INSERT INTO Pagos (
                        concepto, fecha_pago, importe, estado,
                        id_reserva, id_usuario_creador
                    )
                    VALUES (
                        @concepto, @fecha, @importe, '1',
                        @reserva, @usuario
                    );
                ", connection, transaction);

                registrarPago.Parameters.AddWithValue(
                    "@concepto", "Multa por finalización anticipada"
                );
                registrarPago.Parameters.AddWithValue("@fecha", ahora);
                registrarPago.Parameters.AddWithValue("@importe", multa);
                registrarPago.Parameters.AddWithValue("@reserva", idReserva);
                registrarPago.Parameters.AddWithValue("@usuario", idUsuario);

                registrarPago.ExecuteNonQuery();
            }

            using var finalizar = new MySqlCommand(@"
                UPDATE Reservas
                SET estado = '0',
                    fecha_fin_efectiva = @fecha,
                    multa = @multa,
                    id_usuario_finalizador = @usuario
                WHERE id = @id AND estado = '1';
            ", connection, transaction);

            finalizar.Parameters.AddWithValue("@fecha", fechaEfectiva);
            finalizar.Parameters.AddWithValue("@multa", multa);
            finalizar.Parameters.AddWithValue("@usuario", idUsuario);
            finalizar.Parameters.AddWithValue("@id", idReserva);

            int filas = finalizar.ExecuteNonQuery();

            if (filas != 1)
                throw new InvalidOperationException(
                    "No se pudo finalizar la reserva."
                );

            transaction.Commit();
            return filas;
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
                                Estado = reader.GetString("estado"),
                                Fecha_Creacion = reader.GetDateTime("Fecha_Creacion"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin_Original = reader.GetDateTime("Fecha_Fin_Original"),
                                Fecha_Fin_Efectiva = reader.GetDateTime("Fecha_Fin_Efectiva"),
                                Monto_Dia = reader.GetDecimal("Monto_Dia"),
                                Multa = reader.GetDecimal("Multa"),
                                ID_Inquilino = reader.GetInt32("ID_Inquilino"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble"),
                                ID_Usuario_Creador = reader.GetInt32("ID_Usuario_Creador"),
                                ID_Usuario_Finalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador")) ? null
                                    : reader.GetInt32("id_usuario_finalizador"),
                            };

                            reservas.Add(reserva);
                        }
                    }
                    connection.Close();
                }
            }

            return reservas;
        }

        public IList<Reserva> MisReservas(int id)
        {
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT r.*
                    FROM Reservas r
                    WHERE id_inquilino = @id
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reserva reserva = new Reserva
                            {
                                id = reader.GetInt32("id"),
                                Estado = reader.GetString("estado"),
                                Fecha_Creacion = reader.GetDateTime("Fecha_Creacion"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin_Original = reader.GetDateTime("Fecha_Fin_Original"),
                                Fecha_Fin_Efectiva = reader.GetDateTime("Fecha_Fin_Efectiva"),
                                Monto_Dia = reader.GetDecimal("Monto_Dia"),
                                Multa = reader.GetDecimal("Multa"),
                                ID_Inquilino = reader.GetInt32("ID_Inquilino"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble"),
                                ID_Usuario_Creador = reader.GetInt32("ID_Usuario_Creador"),
                                ID_Usuario_Finalizador = reader.IsDBNull(reader.GetOrdinal("id_usuario_finalizador")) ? null
                                    : reader.GetInt32("id_usuario_finalizador"),
                            };

                            reservas.Add(reserva);
                        }
                    }
                    connection.Close();
                }
            }

            return reservas;
        }

        public bool ExisteSuperposicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(*)
                    FROM Reservas
                    WHERE id_inmueble = @idInmueble
                    AND estado = 1
                    AND fecha_inicio < @fechaFin
                    AND fecha_fin_efectiva > @fechaInicio
                    AND (@idReservaExcluir IS NULL OR id <> @idReservaExcluir);
                ";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", fechaFin);
                    command.Parameters.AddWithValue(
                        "@idReservaExcluir",
                        idReservaExcluir.HasValue ? idReservaExcluir.Value : DBNull.Value
                    );

                    connection.Open();
                    int cantidad = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();

                    return cantidad > 0;
                }
            }
        }

        public Reserva? ObtenerPorID(int id)
        {
            Reserva? reserva = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        r.*,
                        inq.id AS InqId, inq.Nombre AS InqNombre, inq.Apellido AS InqApellido,
                        inm.id AS InmId,
                        u1.Nombre AS U1Nombre, u1.Apellido AS U1Apellido, u1.Correo AS U1Correo, u1.Avatar AS U1Avatar,
                        u2.Nombre AS U2Nombre, u2.Apellido AS U2Apellido, u2.Correo AS U2Correo, u2.Avatar AS U2Avatar
                    FROM Reservas r
                    JOIN Inquilinos inq ON r.id_inquilino = inq.id
                    JOIN Inmuebles inm ON r.id_inmueble = inm.id
                    JOIN Usuarios u1 ON r.id_usuario_creador = u1.id
                    LEFT JOIN Usuarios u2 ON r.id_usuario_finalizador = u2.id
                    WHERE r.id = @id
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
                                Estado = reader.GetString("estado"),
                                Fecha_Creacion = reader.GetDateTime("Fecha_Creacion"),
                                Fecha_Inicio = reader.GetDateTime("Fecha_Inicio"),
                                Fecha_Fin_Original = reader.GetDateTime("Fecha_Fin_Original"),
                                Fecha_Fin_Efectiva = reader.GetDateTime("Fecha_Fin_Efectiva"),
                                Monto_Dia = reader.GetDecimal("Monto_Dia"),
                                Multa = reader.GetDecimal("Multa"),
                                ID_Inquilino = reader.GetInt32("ID_Inquilino"),
                                ID_Inmueble = reader.GetInt32("ID_Inmueble"),
                                ID_Usuario_Creador = reader.GetInt32("ID_Usuario_Creador"),
                                ID_Usuario_Finalizador =
                                    reader.IsDBNull(reader.GetOrdinal("ID_Usuario_Finalizador"))
                                        ? null
                                        : reader.GetInt32("ID_Usuario_Finalizador"),

                                Inquilino = new Inquilino
                                {
                                    id = reader.GetInt32("InqId"),
                                    Nombre = reader.GetString("InqNombre"),
                                    Apellido = reader.GetString("InqApellido")
                                },

                                Inmueble = new Inmueble
                                {
                                    id = reader.GetInt32("InmId")
                                },

                                Usuario_Creador = new Usuario
                                {
                                    Nombre = reader.GetString("U1Nombre"),
                                    Apellido = reader.GetString("U1Apellido"),
                                    Correo = reader.GetString("U1Correo"),
                                    Avatar = reader.GetString("U1Avatar")
                                },

                                Usuario_Finalizador =
                                    reader.IsDBNull(reader.GetOrdinal("U2Nombre"))
                                        ? null
                                        : new Usuario
                                        {
                                            Nombre = reader.GetString("U2Nombre"),
                                            Apellido = reader.GetString("U2Apellido"),
                                            Correo = reader.GetString("U2Correo"),
                                            Avatar = reader.GetString("U2Avatar")
                                        }
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