using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using System.Security.Claims; // Para la autentucación.
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList.
using Microsoft.AspNetCore.Authorization; // Para usar autorización.

namespace Inmobiliaria_.Net_Core.Controllers {
    [Authorize]
    public class Reserva_Controller : Controller {
        private readonly IRepositorio_Reserva repositorio_Reserva;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly ILogger<Reserva_Controller> logger;

        public Reserva_Controller(
            IRepositorio_Reserva repositorio_Reserva,
            IRepositorio_Inmueble repositorio_Inmueble,
            ILogger<Reserva_Controller> logger
        ) {
            this.repositorio_Reserva = repositorio_Reserva;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.logger = logger;
        }

        // Crear
        [HttpGet]
        public IActionResult Crear(int? ID_Inmueble) {
            if (ID_Inmueble == null) return RedirectToAction("Indice", "Inmueble_");

            var inmueble = repositorio_Inmueble.ObtenerPorID(ID_Inmueble.Value);
            if (inmueble == null) return NotFound();

            var reserva = new Reserva {
                ID_Inmueble = inmueble.id,
                Monto_Dia = inmueble.Precio_Dia,
                Inmueble = inmueble
            };

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Crear(Reserva reserva) {
            ModelState.Remove(nameof(reserva.Estado));
            ModelState.Remove(nameof(reserva.ID_Inquilino));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));

            if (!ModelState.IsValid) retornar();
            if (reserva.Fecha_Inicio < DateTime.Now) retornar();
            if (reserva.Fecha_Inicio >= reserva.Fecha_Fin_Original) retornar();
            if (reserva.Fecha_Inicio <= reserva.Fecha_Fin_Efectiva) retornar();
            if (reserva.Fecha_Fin_Original > reserva.Fecha_Fin_Efectiva) retornar();

            int IDUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            reserva.ID_Usuario_Creador = IDUsuarioActual;
            reserva.ID_Inquilino = IDUsuarioActual;

            var inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
            if (inmueble == null) return NotFound();

            reserva.Monto_Dia = inmueble.Precio_Dia;

            repositorio_Reserva.Alta(reserva);

            logger.LogInformation($"Se registró correctamente la Reserva con el ID: {reserva.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Indice));

            IActionResult retornar() {
                reserva.Inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
                return View(reserva);
            }
        }

        // Eliminar
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarEliminar(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();

            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUsuarioClaim == null) return Unauthorized();

            int idUsuario = int.Parse(idUsuarioClaim);

            repositorio_Reserva.Baja(id, idUsuario);

            logger.LogInformation($"Se finalizó correctamente la Reserva con el ID: {id}");
            TempData["Mensaje"] = "La reserva se finalizó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            var inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);

            if (reserva == null || inmueble == null) return NotFound();
            if (inmueble != null) {
                var nuevaReserva = new Reserva {
                    id = reserva.id,
                    Fecha_Creacion = reserva.Fecha_Creacion,
                    Fecha_Inicio = reserva.Fecha_Inicio,
                    Fecha_Fin_Original = reserva.Fecha_Fin_Original,
                    Fecha_Fin_Efectiva = reserva.Fecha_Fin_Efectiva,
                    Monto_Dia = reserva.Monto_Dia,
                    Multa = reserva.Multa,
                    Estado = reserva.Estado,
                    ID_Inquilino = reserva.ID_Inquilino,
                    ID_Inmueble = reserva.ID_Inmueble,
                    ID_Usuario_Creador = reserva.ID_Usuario_Creador,
                    ID_Usuario_Finalizador = reserva.ID_Usuario_Finalizador,
                    Inmueble = inmueble
                };

                return View(nuevaReserva);
            }

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Reserva reserva) {
            if (id != reserva.id) return BadRequest();

            ModelState.Remove(nameof(reserva.Fecha_Creacion));
            ModelState.Remove(nameof(reserva.ID_Inquilino));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));

            if (!ModelState.IsValid) {
                reserva.Inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
                return View(reserva);
            }

            var reservaExistente = repositorio_Reserva.ObtenerPorID(id);
            if (reservaExistente == null) return NotFound();

            reserva.Fecha_Creacion = reservaExistente.Fecha_Creacion;
            reserva.ID_Inquilino = reservaExistente.ID_Inquilino;
            reserva.ID_Usuario_Creador = reservaExistente.ID_Usuario_Creador;
            reserva.ID_Usuario_Finalizador = reservaExistente.ID_Usuario_Finalizador;
            
            repositorio_Reserva.Modificacion(reserva);

            logger.LogInformation($"Se modificó correctamente la Reserva con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Obtener todos
        public IActionResult Indice(bool todos = false) {
            int IDUsuarioActual = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            IList<Reserva> reservas;

            if (User.IsInRole("Administrador") && todos) {
                reservas = repositorio_Reserva.ObtenerTodos();
                ViewBag.Todas = true;
            } else {
                reservas = repositorio_Reserva.MisReservas(IDUsuarioActual);
                ViewBag.Todas = false;
            }

            return View(reservas);
        }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }
    }
}