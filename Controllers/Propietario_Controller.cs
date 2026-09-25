using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_.Net_Core.Controllers {
    [Authorize]
    public class Propietario_Controller : Controller {
        private readonly IRepositorio_Propietario repositorio_Propietario;
        private readonly IRepositorio_Usuario repositorio_Usuario;
        private readonly ILogger<Propietario_Controller> logger;

        public Propietario_Controller(
            IRepositorio_Propietario repositorio_Propietario,
            IRepositorio_Usuario repositorio_Usuario,
            ILogger<Propietario_Controller> logger
        ) {
            this.repositorio_Propietario = repositorio_Propietario;
            this.repositorio_Usuario = repositorio_Usuario;
            this.logger = logger;
        }

        // Listar
        public IActionResult Indice() {
            var propietarios = repositorio_Propietario.ObtenerTodos();
            return View(propietarios);
        }

        // Detalles
        [AllowAnonymous]
        public IActionResult Detalles(int id) {
            var propietario = repositorio_Propietario.ObtenerPorID(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // Crear
        [HttpGet]
        public IActionResult Crear(int? ID_Usuario) {
            var nuevoPropietario = new Propietario();

            if (ID_Usuario.HasValue) {
                var usuario = repositorio_Usuario.ObtenerPorID(ID_Usuario.Value);

                if (usuario != null) {
                    nuevoPropietario.Nombre = usuario.Nombre;
                    nuevoPropietario.Apellido = usuario.Apellido;
                    nuevoPropietario.Correo = usuario.Correo;
                }
            }
            
            return View(nuevoPropietario);
        }

        // Crear
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Crear(Propietario propietario) {
            if (!ModelState.IsValid) return View(propietario);

            int IDUsuarioActual = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var usuario = repositorio_Usuario.ObtenerPorID(IDUsuarioActual);

            if (usuario != null) {
                propietario.Nombre = usuario.Nombre;
                propietario.Apellido = usuario.Apellido;
            }

            repositorio_Propietario.Alta(propietario);

            logger.LogInformation("Se registró correctamente el propietario con ID: ", propietario.id);
            TempData["Mensaje"] = "El propietario fue registrado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var propietario = repositorio_Propietario.ObtenerPorID(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // Modificar
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Modificar(int id, Propietario propietario) {
            if (id != propietario.id) return BadRequest();
            if (!ModelState.IsValid) return View(propietario);

            var propietarioExistente = repositorio_Propietario.ObtenerPorID(id);
            if (propietarioExistente == null) return NotFound(); 

            repositorio_Propietario.Modificacion(propietario);

            logger.LogInformation("Se actualizó correctamente el propietario con ID: ", propietario.id);
            TempData["Mensaje"] = "El propietario fue actualizado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Eliminar
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id) {
            var propietario = repositorio_Propietario.ObtenerPorID(id);
            if (propietario == null) { return NotFound(); }
            return View(propietario);
        }

        // Eliminar
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete"), Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarEliminar(int id) {
            var propietario = repositorio_Propietario.ObtenerPorID(id);
            if (propietario == null) return NotFound();

            repositorio_Propietario.Baja(id);

            logger.LogInformation("Se eliminó correctamente el propietario con ID: ", id);
            TempData["Mensaje"] = "El propietario fue eliminado correctamente.";

            return RedirectToAction(nameof(Indice));
        }
    }
}