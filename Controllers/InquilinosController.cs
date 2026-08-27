using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class InquilinosController : Controller {
        private readonly IRepositorioInquilino repositorio;
        private readonly ILogger<InquilinosController> logger;

        public InquilinosController(IRepositorioInquilino repositorio, ILogger<InquilinosController> logger) {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // ==========================================
        // LISTAR INQUILINOS
        // GET: /Inquilinos
        // ==========================================
        public IActionResult Index() {
            var inquilinos = repositorio.ObtenerTodos();

            return View(inquilinos);
        }

        // ==========================================
        // MOSTRAR FORMULARIO DE ALTA
        // GET: /Inquilinos/Create
        // ==========================================
        [HttpGet]
        public IActionResult Create() {
            return View();
        }

        // ==========================================
        // GUARDAR NUEVO INQUILINO
        // POST: /Inquilinos/Create
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino) {
            if (!ModelState.IsValid) { return View(inquilino); }

            repositorio.Alta(inquilino);

            logger.LogInformation("Se registró correctamente el inquilino con ID {Id}", inquilino.id);

            TempData["Mensaje"] = "El inquilino fue registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MOSTRAR FORMULARIO DE EDICIÓN
        // GET: /Inquilinos/Edit/5
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id) {
            var inquilino = repositorio.ObtenerPorId(id);

            if (inquilino == null) { return NotFound(); }

            return View(inquilino);
        }

        // ==========================================
        // GUARDAR MODIFICACIÓN
        // POST: /Inquilinos/Edit/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino) {
            if (id != inquilino.id) { return BadRequest(); }

            if (!ModelState.IsValid) { return View(inquilino); }

            var inquilinoExistente = repositorio.ObtenerPorId(id);

            if (inquilinoExistente == null) { return NotFound(); }

            repositorio.Modificacion(inquilino);

            logger.LogInformation("Se actualizó correctamente el inquilino con ID {Id}", inquilino.id);

            TempData["Mensaje"] = "El inquilino fue actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        // GET: /Inquilinos/Delete/5
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id) {
            var inquilino = repositorio.ObtenerPorId(id);

            if (inquilino == null) { return NotFound(); }

            return View(inquilino);
        }

        // ==========================================
        // ELIMINAR INQUILINO
        // POST: /Inquilinos/Delete/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) {
            var inquilino = repositorio.ObtenerPorId(id);

            if (inquilino == null) { return NotFound(); }

            repositorio.Baja(id);

            logger.LogInformation("Se eliminó correctamente el inquilino con ID {Id}", id);

            TempData["Mensaje"] = "El inquilino fue eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}