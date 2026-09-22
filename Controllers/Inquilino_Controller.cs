using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_.Net_Core.Controllers {
    [Authorize]
    public class Inquilino_Controller : Controller {
        private readonly IRepositorio_Inquilino repositorio;
        private readonly ILogger<Inquilino_Controller> logger;

        public Inquilino_Controller(IRepositorio_Inquilino repositorio, ILogger<Inquilino_Controller> logger) {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Listar
        public IActionResult Indice() {
            var inquilinos = repositorio.ObtenerTodos();
            return View(inquilinos);
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() { return View(); }

        // Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Inquilino inquilino) {
            if (!ModelState.IsValid) return View(inquilino);

            repositorio.Alta(inquilino);

            logger.LogInformation("Se registró correctamente el inquilino con ID {Id}", inquilino.id);
            TempData["Mensaje"] = "El inquilino fue registrado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var inquilino = repositorio.ObtenerPorID(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(int id, Inquilino inquilino) {
            if (id != inquilino.id) { return BadRequest(); }
            if (!ModelState.IsValid) { return View(inquilino); }

            var inquilinoExistente = repositorio.ObtenerPorID(id);
            if (inquilinoExistente == null) return NotFound();

            repositorio.Modificacion(inquilino);

            logger.LogInformation("Se actualizó correctamente el inquilino con ID {Id}", inquilino.id);
            TempData["Mensaje"] = "El inquilino fue actualizado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var inquilino = repositorio.ObtenerPorID(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult ConfirmarEliminar(int id) {
            var inquilino = repositorio.ObtenerPorID(id);
            if (inquilino == null) return NotFound();

            repositorio.Baja(id);

            logger.LogInformation("Se eliminó correctamente el inquilino con ID {Id}", id);
            TempData["Mensaje"] = "El inquilino fue eliminado correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Detalles
        [HttpGet]
        public IActionResult Detalles(int id) {
            var inquilino = repositorio.ObtenerPorID(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }
    }
}