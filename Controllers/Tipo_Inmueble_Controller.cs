using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Tipo_Inmueble_Controller : Controller {
        private readonly IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble;
        private readonly IRepositorio_Propietario repositorio_Propietario;
        private readonly ILogger<Tipo_Inmueble_Controller> logger;

        public Tipo_Inmueble_Controller(IRepositorio_Tipo_Inmueble repositorio, IRepositorio_Propietario repositorio_Propietario, ILogger<Tipo_Inmueble_Controller> logger) {
            this.repositorio_Tipo_Inmueble = repositorio;
            this.repositorio_Propietario = repositorio_Propietario;
            this.logger = logger;
        }

        // Crear (dar de alta)
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "DNI", "Apellido");

            return View();
        }

        [HttpPost]
        public IActionResult Crear(Tipo_Inmueble tipo_Inmueble) {
            if (!ModelState.IsValid) {
                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "DNI", "Apellido");

                return View(tipo_Inmueble);
            }

            repositorio_Tipo_Inmueble.Alta(tipo_Inmueble);
            logger.LogInformation($"Se registró correctamente el Tipo de Inmueble con el ID: {tipo_Inmueble.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Eliminar (dar de baja)
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var tipo_Inmueble = repositorio_Tipo_Inmueble.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();
            return View(tipo_Inmueble);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id) {
            var tipo_Inmueble = repositorio_Tipo_Inmueble.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();

            repositorio_Tipo_Inmueble.Baja(id);
            logger.LogInformation($"Se eliminó correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Modificar (Modificación)
        [HttpGet]
        public IActionResult Modificar(int id) {
            var tipo_Inmueble = repositorio_Tipo_Inmueble.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();

            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "DNI", "Apellido");

            return View(tipo_Inmueble);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Tipo_Inmueble tipo_Inmueble) {
            if (id != tipo_Inmueble.id) return BadRequest();
            if (!ModelState.IsValid) {
                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "DNI", "Apellido");

                return View(tipo_Inmueble);
            }
            
            var tipo_InmuebleExistente = repositorio_Tipo_Inmueble.ObtenerPorID(id);
            if (tipo_InmuebleExistente == null) return NotFound();

            repositorio_Tipo_Inmueble.Modificacion(tipo_Inmueble);
            logger.LogInformation($"Se modificó correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Obtener todos
        public IActionResult Indice() { return View(repositorio_Tipo_Inmueble.ObtenerTodos()); }

        // Obtener por nombre
        public IActionResult Nombre(string nombre) {
            if (string.IsNullOrEmpty(nombre)) return RedirectToAction(nameof(Indice));

            var tipo_Inmueble = repositorio_Tipo_Inmueble.ObtenerPorNombre(nombre);
            if (tipo_Inmueble == null) return NotFound();

            return View(tipo_Inmueble);
        }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var tipo_Inmueble = repositorio_Tipo_Inmueble.ObtenerPorID(id);
            if (tipo_Inmueble == null) return NotFound();
            return View(tipo_Inmueble);
        }
    }
}