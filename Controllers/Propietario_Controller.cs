using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Propietario_Controller : Controller {
        private readonly IRepositorio_Propietario repositorio;
        private readonly ILogger<Propietario_Controller> logger;

        public Propietario_Controller(IRepositorio_Propietario repositorio, ILogger<Propietario_Controller> logger) {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Listar
        public IActionResult Indice() {
            var propietarios = repositorio.ObtenerTodos();
            return View(propietarios);
        }

        // Detalles
        public IActionResult Detalles(int id) {
            var propietario = repositorio.ObtenerPorID(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() { return View(); }

        // Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Propietario propietario) {
            if (!ModelState.IsValid) return View(propietario);

            repositorio.Alta(propietario);

            logger.LogInformation("Se registró correctamente el propietario con ID: ", propietario.id);
            TempData["Mensaje"] = "El propietario fue registrado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var propietario = repositorio.ObtenerPorID(id);
            if (propietario == null) return NotFound();
            return View(propietario);
        }

        // Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(int id, Propietario propietario) {
            if (id != propietario.id) return BadRequest();
            if (!ModelState.IsValid) return View(propietario);

            var propietarioExistente = repositorio.ObtenerPorID(id);
            if (propietarioExistente == null) return NotFound(); 

            repositorio.Modificacion(propietario);

            logger.LogInformation("Se actualizó correctamente el propietario con ID: ", propietario.id);
            TempData["Mensaje"] = "El propietario fue actualizado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var propietario = repositorio.ObtenerPorID(id);

            if (propietario == null) { return NotFound(); }

            return View(propietario);
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult ConfirmarEliminar(int id) {
            var propietario = repositorio.ObtenerPorID(id);
            if (propietario == null) return NotFound();

            repositorio.Baja(id);

            logger.LogInformation("Se eliminó correctamente el propietario con ID: ", d);
            TempData["Mensaje"] = "El propietario fue eliminado correctamente.";

            return RedirectToAction(nameof(Indice));
        }
    }
}