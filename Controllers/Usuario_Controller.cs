using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers {
    [Authorize(Roles = "Administrador")]
    public class Usuario_Controller : Controller {
        private readonly IRepositorio_Usuario repositorio;
        private readonly ILogger<Usuario_Controller> logger;

        public Usuario_Controller(
            IRepositorio_Usuario repositorio,
            ILogger<Usuario_Controller> logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Listar
        [HttpGet]
        public IActionResult Indice() {
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() { return View(); }

        // Crear
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Crear(Usuario usuario) {
            if (!ModelState.IsValid) return View(usuario);

            var usuarioExistente = repositorio.ObtenerPorCorreo(usuario.Correo);

            if (usuarioExistente != null) {
                ModelState.AddModelError("Correo", "Ya existe un usuario registrado con este correo.");
                return View(usuario);
            }

            int id = repositorio.Alta(usuario);

            logger.LogInformation("Se creó correctamente el usuario con ID {Id}", id);
            TempData["Mensaje"] = "El usuario fue creado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Detalles
        [HttpGet]
        public IActionResult Detalles(int id) {
            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }


        // Modificar
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Modificar(int id, Usuario usuario) {
            if (id != usuario.id) return BadRequest();
            if (!ModelState.IsValid) return View(usuario);

            var usuarioConMismoCorreo = repositorio.ObtenerPorCorreo(usuario.Correo);

            if (usuarioConMismoCorreo != null && usuarioConMismoCorreo.id != usuario.id) {
                ModelState.AddModelError("Correo", "Ya existe otro usuario registrado con este correo.");
                return View(usuario);
            }

            repositorio.Modificacion(usuario);

            logger.LogInformation("Se modificó correctamente el usuario con ID {Id}", usuario.id);
            TempData["Mensaje"] = "El usuario fue modificado correctamente.";

            return RedirectToAction(nameof(Indice));
        }


        // Eliminar
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }


        // Eliminar
        [HttpPost, ValidateAntiForgeryToken, ActionName("Eliminar")]
        public IActionResult ConfirmarEliminar(int id) {
            var usuario = repositorio.ObtenerPorID(id);
            if (usuario == null) return NotFound();

            var idUsuarioActual = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (idUsuarioActual == id.ToString()) {
                TempData["Mensaje"] = "No puede dar de baja su propio usuario.";
                return RedirectToAction(nameof(Indice));
            }

            repositorio.Baja(id);

            logger.LogInformation("Se dio de baja al usuario con ID {Id}", id);
            TempData["Mensaje"] = "El usuario fue dado de baja correctamente.";

            return RedirectToAction(nameof(Indice));
        }
    }
}