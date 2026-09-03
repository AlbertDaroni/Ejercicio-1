using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Propietarios_Controller : Controller {
        private readonly IRepositorioPropietario repositorio;
        private readonly ILogger<Propietarios_Controller> logger;

        public Propietarios_Controller(IRepositorioPropietario repositorio, ILogger<Propietarios_Controller> logger) {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // ==========================================
        // LISTAR PROPIETARIOS
        // GET: /Propietarios
        // ==========================================
        public IActionResult Index() {
            var propietarios = repositorio.ObtenerTodos();

            return View(propietarios);
        }

        // ==========================================
        // DETALLE DE UN PROPIETARIO
        // GET: /Propietarios/Details/5
        // ==========================================
        public IActionResult Details(int id) {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null) { return NotFound(); }

            return View(propietario);
        }

        // ==========================================
        // MOSTRAR FORMULARIO DE ALTA
        // GET: /Propietarios/Create
        // ==========================================
        [HttpGet]
        public IActionResult Create() { return View(); }

        // ==========================================
        // GUARDAR NUEVO PROPIETARIO
        // POST: /Propietarios/Create
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario) {
            if (!ModelState.IsValid) { return View(propietario); }

            repositorio.Alta(propietario);

            logger.LogInformation("Se registró correctamente el propietario con ID {Id}", propietario.id);

            TempData["Mensaje"] = "El propietario fue registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MOSTRAR FORMULARIO DE MODIFICACIÓN
        // GET: /Propietarios/Edit/5
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id) {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null) { return NotFound(); }

            return View(propietario);
        }

        // ==========================================
        // GUARDAR MODIFICACIÓN
        // POST: /Propietarios/Edit/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario) {
            if (id != propietario.id) { return BadRequest(); }

            if (!ModelState.IsValid) { return View(propietario); }

            var propietarioExistente = repositorio.ObtenerPorId(id);

            if (propietarioExistente == null) { return NotFound(); }

            repositorio.Modificacion(propietario);

            logger.LogInformation("Se actualizó correctamente el propietario con ID {Id}", propietario.id);

            TempData["Mensaje"] = "El propietario fue actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        // GET: /Propietarios/Delete/5
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id) {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null) { return NotFound(); }

            return View(propietario);
        }

        // ==========================================
        // ELIMINAR PROPIETARIO
        // POST: /Propietarios/Delete/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null) { return NotFound(); }

            repositorio.Baja(id);

            logger.LogInformation("Se eliminó correctamente el propietario con ID {Id}",id);

            TempData["Mensaje"] = "El propietario fue eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}