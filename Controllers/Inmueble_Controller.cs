using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Inmueble_Controller : Controller {
        private readonly IRepositorio_Inmueble repositorio;
        private readonly ILogger<Inmueble_Controller> logger;

        public Inmueble_Controller(IRepositorio_Inmueble repositorio, ILogger<Inmueble_Controller> logger) {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Crear (dar de Alta)
        [HttpGet]
        public IActionResult Crear() { return View(); }

        [HttpPost]
        public IActionResult Crear(Inmueble inmueble) {
            if (!ModelState.IsValid) return View(inmueble);

            repositorio.Alta(inmueble);
            logger.LogInformation($"Se registró correctamente el Inmueble con el ID: {inmueble.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Eliminar (dar de Baja)
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var inmueble = repositorio.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult ConfirmarEliminar(int id) {
            var inmueble = repositorio.ObtenerPorID(id);
            if (inmueble == null) return NotFound();

            repositorio.Baja(id);
            logger.LogInformation($"Se eliminó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Modificar (Modificación)
        [HttpGet]
        public IActionResult Modificar(int id) {
            var inmueble = repositorio.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Inmueble inmueble) {
            if (id != inmueble.id) return BadRequest();
            if (!ModelState.IsValid) return View(inmueble);
            
            var inmuebleExistente = repositorio.ObtenerPorID(id);
            if (inmuebleExistente == null) return NotFound();

            repositorio.Modificacion(inmueble);
            logger.LogInformation($"Se modificó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Obtener todos
        public IActionResult Indice() { return View(repositorio.ObtenerTodos()); }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var inmueble = repositorio.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        // Obtener por dirección
        public IActionResult Direccion(string direccion) { return View(repositorio.ObtenerPorDireccion(direccion)); }
    }
}