using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Identity;


namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize]
    public class Usuario_Controller : Controller
    {
        private readonly IRepositorio_Usuario repositorio;
        private readonly ILogger<Usuario_Controller> logger;
        private readonly IWebHostEnvironment environment;
        private readonly IPasswordHasher<Usuario> passwordHasher;


        public Usuario_Controller(
            IRepositorio_Usuario repositorio,
            ILogger<Usuario_Controller> logger,
            IWebHostEnvironment environment,
            IPasswordHasher<Usuario> passwordHasher)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.environment = environment;
            this.passwordHasher = passwordHasher;
        }

        // Listar
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Indice(bool todos = false)
        {
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);
        }

        // Crear
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Crear() { return View(); }

        // Crear
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador")]
        public IActionResult Crear(Usuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);

            var usuarioExistente = repositorio.ObtenerPorCorreo(usuario.Correo);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError("Correo", "Ya existe un usuario registrado con este correo.");
                return View(usuario);
            }

            // Hashear la contraseña antes de guardarla
            usuario.Contraseña = passwordHasher.HashPassword(
                usuario,
                usuario.Contraseña
            );

            int id = repositorio.Alta(usuario);

            logger.LogInformation("Se creó correctamente el usuario con ID {Id}", id);
            TempData["Mensaje"] = "El usuario fue creado correctamente.";

            return RedirectToAction(nameof(Detalles), new { id });
        }

        // Detalles
        [HttpGet]
        public IActionResult Detalles(int id)
        {
            var idUsuarioActual =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idUsuarioActual, out int idActual))
            {
                return Unauthorized();
            }

            if (!User.IsInRole("Administrador") && idActual != id)
            {
                return Forbid();
            }

            var usuario = repositorio.ObtenerPorID(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var idUsuarioActual = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idUsuarioActual, out int idActual)) return Unauthorized();
            if (!User.IsInRole("Administrador") && idActual != id) return Forbid();

            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }


        // Modificar
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Modificar(
            int id,
            Usuario usuario,
            IFormFile? ArchivoAvatar
        )
        {
            if (id != usuario.id) return BadRequest();

            var idUsuarioActual =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idUsuarioActual, out int idActual))
                return Unauthorized();

            bool esAdministrador = User.IsInRole("Administrador");

            if (!esAdministrador && idActual != id)
                return Forbid();
            var usuarioExistente = repositorio.ObtenerPorID(id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            if (!esAdministrador)
            {
                usuario.Rol = usuarioExistente.Rol;
                usuario.Estado = usuarioExistente.Estado;
            } 
            // La contraseña no se modifica desde la edicion del perfil.
            usuario.Contraseña = usuarioExistente.Contraseña;   

            var usuarioActual = repositorio.ObtenerPorID(id);
            if (usuarioActual == null) return NotFound();

            if (!esAdministrador)
            {
                ModelState.Remove(nameof(usuario.Rol));
                ModelState.Remove(nameof(usuario.Estado));

                usuario.Rol = usuarioActual.Rol;
                usuario.Estado = usuarioActual.Estado;
            }

            ModelState.Remove(nameof(usuario.Avatar));
            usuario.Avatar = usuarioActual.Avatar;

            var usuarioConMismoCorreo =
                repositorio.ObtenerPorCorreo(usuario.Correo);

            if (usuarioConMismoCorreo != null &&
                usuarioConMismoCorreo.id != id)
            {
                ModelState.AddModelError(
                    nameof(usuario.Correo),
                    "Ya existe otro usuario registrado con este correo."
                );
            }

            string? extensionAvatar = null;

            if (ArchivoAvatar != null)
            {
                if (ArchivoAvatar.Length == 0 ||
                    ArchivoAvatar.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        nameof(usuario.Avatar),
                        "La imagen debe tener contenido y no superar 2 MB."
                    );
                }
                else
                {
                    byte[] cabecera = new byte[8];
                    int leidos = 0;

                    using (var stream = ArchivoAvatar.OpenReadStream())
                    {
                        while (leidos < cabecera.Length)
                        {
                            int cantidad = await stream.ReadAsync(
                                cabecera,
                                leidos,
                                cabecera.Length - leidos
                            );

                            if (cantidad == 0) break;
                            leidos += cantidad;
                        }
                    }

                    bool esPng = leidos == 8 &&
                        cabecera[0] == 0x89 &&
                        cabecera[1] == 0x50 &&
                        cabecera[2] == 0x4E &&
                        cabecera[3] == 0x47 &&
                        cabecera[4] == 0x0D &&
                        cabecera[5] == 0x0A &&
                        cabecera[6] == 0x1A &&
                        cabecera[7] == 0x0A;

                    bool esJpg = leidos >= 3 &&
                        cabecera[0] == 0xFF &&
                        cabecera[1] == 0xD8 &&
                        cabecera[2] == 0xFF;

                    string extension = Path.GetExtension(
                        ArchivoAvatar.FileName
                    ).ToLowerInvariant();

                    if (esPng && extension == ".png")
                    {
                        extensionAvatar = ".png";
                    }
                    else if (esJpg &&
                        (extension == ".jpg" || extension == ".jpeg"))
                    {
                        extensionAvatar = ".jpg";
                    }
                    else
                    {
                        ModelState.AddModelError(
                            nameof(usuario.Avatar),
                            "Seleccioná una imagen JPG o PNG válida."
                        );
                    }
                }
            }
            // La contraseña no se modifica desde este formulario.
            ModelState.Remove(nameof(usuario.Contraseña));
            if (!ModelState.IsValid)
                return View(usuario);

            if (ArchivoAvatar != null && extensionAvatar != null)
            {
                string carpeta = Path.Combine(
                    environment.WebRootPath,
                    "uploads",
                    "avatares"
                );

                Directory.CreateDirectory(carpeta);

                string nombre = $"{Guid.NewGuid():N}{extensionAvatar}";
                string ruta = Path.Combine(carpeta, nombre);

                using (var destino = new FileStream(
                    ruta, FileMode.CreateNew
                ))
                {
                    await ArchivoAvatar.CopyToAsync(destino);
                }

                usuario.Avatar = "/uploads/avatares/" + nombre;
            }

            repositorio.Modificacion(usuario);

            logger.LogInformation(
                "Se modificó correctamente el usuario con ID {Id}", id
            );

            TempData["Mensaje"] = "El usuario fue modificado correctamente.";

            return RedirectToAction(nameof(Detalles), new { id });
        }


        // Eliminar
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }


        // Eliminar
        [HttpPost, ValidateAntiForgeryToken, ActionName("Eliminar"), Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarEliminar(int id)
        {
            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();

            var idUsuarioActual = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (idUsuarioActual == id.ToString())
            {
                TempData["Mensaje"] = "No puede dar de baja su propio usuario.";
                return RedirectToAction(nameof(Detalles));
            }

            repositorio.Baja(id);

            logger.LogInformation("Se dio de baja al usuario con ID {Id}", id);
            TempData["Mensaje"] = "El usuario fue dado de baja correctamente.";

            return RedirectToAction(nameof(Detalles));
        }
    }
}