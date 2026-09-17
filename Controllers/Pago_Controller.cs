using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Pago_Controller : Controller {
        private readonly IRepositorio_Pago repositorio_Pago;
        private readonly IRepositorio_Reserva repositorio_Reserva;
        private readonly ILogger<Pago_Controller> logger;

        public Pago_Controller(
            IRepositorio_Pago repositorio_Pago,
            IRepositorio_Reserva repositorio_Reserva,
            ILogger<Pago_Controller> logger)
        {
            this.repositorio_Pago = repositorio_Pago;
            this.repositorio_Reserva = repositorio_Reserva;
            this.logger = logger;
        }

        // Listar
        [HttpGet]
        public IActionResult Index() {
            var pagos = repositorio_Pago.ObtenerTodos();
            return View(pagos);
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Reservas = new SelectList(repositorio_Reserva.ObtenerTodos(), "id", "id" );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago) {
            pago.Estado = "1";
            pago.Fecha_Anulacion = null;
            pago.ID_Usuario_Creador = 1; // <-- Cambiar cuando se agregue la autenticación
            pago.ID_Usuario_Finalizador = null;

            ModelState.Remove(nameof(pago.Estado));
            ModelState.Remove(nameof(pago.ID_Usuario_Creador));
            ModelState.Remove(nameof(pago.ID_Usuario_Finalizador));
            ModelState.Remove(nameof(pago.Fecha_Anulacion));

            if (!ModelState.IsValid) {
                ViewBag.Reservas = new SelectList(repositorio_Reserva.ObtenerTodos(), "id", "id");
                return View(pago);
            }

            repositorio_Pago.Alta(pago);
            logger.LogInformation($"Se registró correctamente el Pago con ID: {pago.id}");
            TempData["Mensaje"] = "El pago se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // Modificar
        [HttpPost]
        public IActionResult Modificar(Pago pago) {
            ModelState.Remove(nameof(pago.ID_Usuario_Creador));
            ModelState.Remove(nameof(pago.ID_Usuario_Finalizador));

            if (!ModelState.IsValid) return View(pago);

            pago.ID_Usuario_Finalizador = 1;
            repositorio_Pago.Modificacion(pago);

            logger.LogInformation($"Se modificó correctamente el Pago con ID: {pago.id}");
            TempData["Mensaje"] = "El pago se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Ver detalles
        [HttpGet]
        public IActionResult Detalles(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminar(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);

            if (pago == null) return NotFound();

            repositorio_Pago.Baja(id);

            logger.LogInformation( $"Se eliminó correctamente el Pago con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

    }
}