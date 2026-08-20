using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria_.Net_Core.Models {
    public class Propietario : Persona, RepositorioBase {
        public int Alta(Propietario p) {
            int respuesta = -1;

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string sql = @"
                    INSERT INTO propietarios (Nombre, Apellido, DNI, Telefono, Correo)
                    VALUES (@nombre, @apellido, @dni, @telefono, @correo);
                    SELECT LAST_INSERT_ID();
                ";

                using (SqlCommand command = new SqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);

					connection.Open();
					respuesta = Convert.ToInt32(command.ExecuteScalar());
					p.id = respuesta;
					connection.Close();
                }
            }

            return respuesta;
        }

        public int Baja(int id) {
            int respuesta = -1;

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string sql = @"DELETE FROM propietarios WHERE id = @id";

                using(SqlCommand command = new SqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);

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

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                string sql = @"UPDATE propietarios SET
                    Nombre = @nombre, Apellido = @apellido, DNI = @dni, Telefono = @telefono, Correo = @correo
                    WHERE id = @id
                ";

                using (SqlCommand command = new SqlCommand(sql, connection)) {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@correo", p.Correo);

                    connection.Open();
                    respuesta = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return respuesta;
        }

        public override string ToString() { return $"Propietario: {Nombre} {Apellido} - {DNI}"; }
    }
}