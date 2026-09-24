using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        public IActionResult Indice() {
            var pagos = repositorio_Pago.ObtenerTodos();
            return View(pagos);
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Reservas = new SelectList(repositorio_Reserva.ObtenerTodos(), "id", "id" );
            return View();
        }

        // Crear
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago) {
            pago.Estado = "1";
            pago.Fecha_Anulacion = null;
            pago.ID_Usuario_Creador = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
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

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);

            if (pago == null) return NotFound();
            if (pago.Estado == "0") {
                TempData["Mensaje"] = "No se puede modificar un pago anulado.";
                return RedirectToAction(nameof(Indice));
            }

            return View(pago);
        }

        // Modificar
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Modificar(Pago pago) {
            ModelState.Remove(nameof(pago.ID_Usuario_Creador));
            ModelState.Remove(nameof(pago.ID_Usuario_Finalizador));
            ModelState.Remove(nameof(pago.Fecha_Anulacion));
            ModelState.Remove(nameof(pago.Estado));
            ModelState.Remove(nameof(pago.Fecha_Pago));
            ModelState.Remove(nameof(pago.Importe));
            ModelState.Remove(nameof(pago.ID_Reserva));

            if (!ModelState.IsValid) return View(pago);

            var pagoExistente = repositorio_Pago.ObtenerPorID(pago.id);

            if (pagoExistente == null) return NotFound();
            if (pagoExistente.Estado == "0") {
                TempData["Mensaje"] = "No se puede modificar un pago anulado.";
                return RedirectToAction(nameof(Indice));
            }

            pago.ID_Usuario_Creador = pagoExistente.ID_Usuario_Creador;
            pago.ID_Usuario_Finalizador = pagoExistente.ID_Usuario_Finalizador;
            pago.Fecha_Anulacion = pagoExistente.Fecha_Anulacion;
            pago.Estado = pagoExistente.Estado;
            pago.Fecha_Pago = pagoExistente.Fecha_Pago;
            pago.Importe = pagoExistente.Importe;
            pago.ID_Reserva = pagoExistente.ID_Reserva;

            repositorio_Pago.Modificacion(pago);

            logger.LogInformation($"Se modificó correctamente el Pago con ID: {pago.id}");
            TempData["Mensaje"] = "El pago se modificó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Ver detalles
        [HttpGet]
        public IActionResult Detalles(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // Eliminar
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        // Eliminar
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarEliminar(int id) {
            var pago = repositorio_Pago.ObtenerPorID(id);

            if (pago == null) return NotFound();
            if (pago.Estado == "0") {
                TempData["Mensaje"] = "El pago ya se encuentra anulado.";
                return RedirectToAction(nameof(Indice));
            }

            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUsuarioClaim == null) return Unauthorized();

            int idUsuario = int.Parse(idUsuarioClaim);

            repositorio_Pago.Baja(id, idUsuario);

            logger.LogInformation($"Se anuló correctamente el Pago con el ID: {id}");
            TempData["Mensaje"] = "El pago se anuló correctamente.";

            return RedirectToAction(nameof(Indice));
        }
    }
}