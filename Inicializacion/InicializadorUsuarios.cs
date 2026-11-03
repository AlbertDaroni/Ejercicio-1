
using Microsoft.AspNetCore.Identity;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Inicializacion
{
    public static class InicializadorUsuarios
    {
        public static void Ejecutar(
            IConfiguration configuration,
            IRepositorio_Usuario repositorio,
            IPasswordHasher<Usuario> passwordHasher)
        {
            CrearSiNoExiste(
                "Admin",
                "Sistema",
                "admin@inmobiliaria.com",
                "Administrador",
                configuration["UsuariosIniciales:Admin"],
                repositorio,
                passwordHasher
            );

            CrearSiNoExiste(
                "Empleado",
                "Prueba",
                "empleado@inmobiliaria.com",
                "Usuario",
                configuration["UsuariosIniciales:Empleado"],
                repositorio,
                passwordHasher
            );
        }

        private static void CrearSiNoExiste(
            string nombre,
            string apellido,
            string correo,
            string rol,
            string? contraseña,
            IRepositorio_Usuario repositorio,
            IPasswordHasher<Usuario> passwordHasher)
        {
            // No modificar cuentas existentes.
            if (repositorio.ObtenerPorCorreo(correo) != null)
                return;

            if (string.IsNullOrWhiteSpace(contraseña))
                throw new InvalidOperationException(
                    $"Falta configurar la contraseña inicial de {nombre}."
                );

            var usuario = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Correo = correo,
                Rol = rol,
                Estado = "1",
                Avatar = "/uploads/profile_picture/image.png"
            };

            // Hashear antes de almacenar.
            usuario.Contraseña = passwordHasher.HashPassword(
                usuario,
                contraseña
            );

            repositorio.Alta(usuario);
        }
    }
}
