using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList.
using Microsoft.AspNetCore.Authorization; // Para usar autorizacion.

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Inmueble_Controller : Controller {
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Propietario repositorio_Propietario;
        private readonly IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble;
        private readonly ILogger<Inmueble_Controller> logger;

        public Inmueble_Controller(
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Propietario repositorio_Propietario,
            IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble,
            ILogger<Inmueble_Controller> logger
        ) {
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Propietario = repositorio_Propietario;
            this.repositorio_Tipo_Inmueble = repositorio_Tipo_Inmueble;
            this.logger = logger;
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "Nombre");
            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");

            return View();
        }

        [HttpPost]
        public IActionResult Crear(Inmueble inmueble) {
            if (!ModelState.IsValid) {
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "Nombre");
                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");

                return View(inmueble);
            }

            repositorio_Inmueble.Alta(inmueble);
            logger.LogInformation($"Se registró correctamente el Inmueble con el ID: {inmueble.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminar(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();

            repositorio_Inmueble.Baja(id);
            logger.LogInformation($"Se eliminó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Inmueble inmueble) {
            if (id != inmueble.id) return BadRequest();
            if (!ModelState.IsValid) return View(inmueble);
            
            var inmuebleExistente = repositorio_Inmueble.ObtenerPorID(id);
            if (inmuebleExistente == null) return NotFound();

            repositorio_Inmueble.Modificacion(inmueble);
            logger.LogInformation($"Se modificó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Obtener todos
        public IActionResult Indice() { return View(repositorio_Inmueble.ObtenerTodos()); }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        // Obtener por dirección
        public IActionResult Direccion(string direccion) { return View(repositorio_Inmueble.ObtenerPorDireccion(direccion)); }
    }
}