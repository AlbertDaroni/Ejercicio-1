using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Tipo_Inmueble_Controller : Controller {
        private readonly IRepositorio_Tipo_Inmueble repositorio;
        private readonly ILogger<Tipo_Inmueble_Controller> logger;

        public Tipo_Inmueble_Controller(IRepositorio_Tipo_Inmueble repositorio, ILogger<Tipo_Inmueble_Controller> logger) {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Crear (dar de alta)
        [HttpGet]
        public IActionResult Crear() { return View(); }

        [HttpPost]
        public IActionResult Crear(Tipo_Inmueble tipo_Inmueble) {
            if (!ModelState.IsValid) return View(tipo_Inmueble);

            repositorio.Alta(tipo_Inmueble);
            logger.LogInformation($"Se registró correctamente el Tipo de Inmueble con el ID: {tipo_Inmueble.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Eliminar (dar de baja)
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var tipo_Inmueble = repositorio.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();
            return View(tipo_Inmueble);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id) {
            var tipo_Inmueble = repositorio.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();

            repositorio.Baja(id);
            logger.LogInformation($"Se eliminó correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Modificar (Modificación)
        [HttpGet]
        public IActionResult Modificar(int id) {
            var tipo_Inmueble = repositorio.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();
            return View(tipo_Inmueble);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Tipo_Inmueble tipo_Inmueble) {
            if (id != tipo_Inmueble.id) return BadRequest();
            if (!ModelState.IsValid) return View(tipo_Inmueble);
            
            var tipo_InmuebleExistente = repositorio.ObtenerPorID(id);
            if (tipo_InmuebleExistente == null) return NotFound();

            repositorio.Modificacion(tipo_Inmueble);
            logger.LogInformation($"Se modificó correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Obtener todos
        public IActionResult Indice() { return View(repositorio.ObtenerTodos()); }

        // Obtener por nombre
        public IActionResult Nombre(string nombre) {
            if (string.IsNullOrEmpty(nombre)) return RedirectToAction(nameof(Indice));

            var tipo_Inmueble = repositorio.ObtenerPorNombre(nombre);
            if (tipo_Inmueble == null) return NotFound();

            return View(tipo_Inmueble);
        }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var tipo_Inmueble = repositorio.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();
            return View(tipo_Inmueble);
        }
    }
}