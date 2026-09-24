using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using System.Security.Claims; // Para la autentucación.
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList.
using Microsoft.AspNetCore.Authorization; // Para usar autorisacion.

namespace Inmobiliaria_.Net_Core.Controllers {
    [Authorize]
    public class Reserva_Controller : Controller {
        private readonly IRepositorio_Reserva repositorio_Reserva;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Inquilino repositorio_Inquilino;
        private readonly IRepositorio_Usuario repositorio_Usuario;
        private readonly ILogger<Reserva_Controller> logger;

        public Reserva_Controller(
            IRepositorio_Reserva repositorio_Reserva,
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Inquilino repositorio_Inquilino,
            IRepositorio_Usuario repositorio_Usuario,
            ILogger<Reserva_Controller> logger
        ) {
            this.repositorio_Reserva = repositorio_Reserva;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Inquilino = repositorio_Inquilino;
            this.repositorio_Usuario = repositorio_Usuario;
            this.logger = logger;
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion");
            ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "id", "ApellidoYNombre");

            return View();
        }

        [HttpPost]
        public IActionResult Crear(Reserva reserva) {
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));

            if (!ModelState.IsValid) retornar();
            if (reserva.Fecha_Inicio < DateTime.Now) retornar();
            if (reserva.Fecha_Inicio >= reserva.Fecha_Fin_Original) retornar();
            if (reserva.Fecha_Inicio <= reserva.Fecha_Fin_Efectiva) retornar();
            if (reserva.Fecha_Fin_Original > reserva.Fecha_Fin_Efectiva) retornar();

            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idUsuarioClaim == null) return Unauthorized();
            int idUsuarioActual = int.Parse(idUsuarioClaim);
            reserva.ID_Usuario_Creador = idUsuarioActual;

            repositorio_Reserva.Alta(reserva);

            logger.LogInformation($"Se registró correctamente la Reserva con el ID: {reserva.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Indice));

            IActionResult retornar() {
                ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion");
                ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "id", "ApellidoYNombre");

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
            if (reserva == null) return NotFound();

            ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion", reserva.ID_Inmueble);
            ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "id", "ApellidoYNombre", reserva.ID_Inquilino);
            ViewBag.Usuarios = new SelectList(repositorio_Usuario.ObtenerTodos(), "id", "ApellidoYNombre");

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Reserva reserva) {
            if (id != reserva.id) return BadRequest();

            ModelState.Remove(nameof(reserva.Fecha_Creacion));
            ModelState.Remove(nameof(reserva.Estado));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));
            if (!ModelState.IsValid) {
                ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion", reserva.ID_Inmueble);
                ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "id", "ApellidoYNombre", reserva.ID_Inquilino);
                ViewBag.Usuarios = new SelectList(repositorio_Usuario.ObtenerTodos(), "id", "ApellidoYNombre");

                return View(reserva);
            }

            var reservaExistente = repositorio_Reserva.ObtenerPorID(id);

            if (reservaExistente == null)
                return NotFound();

            reserva.Fecha_Creacion = reservaExistente.Fecha_Creacion;
            reserva.Estado = reservaExistente.Estado;
            reserva.ID_Usuario_Creador = reservaExistente.ID_Usuario_Creador;
            reserva.ID_Usuario_Finalizador = reservaExistente.ID_Usuario_Finalizador;
            
            repositorio_Reserva.Modificacion(reserva);

            logger.LogInformation($"Se modificó correctamente la Reserva con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Obtener todos
        public IActionResult Indice() { return View(repositorio_Reserva.ObtenerTodos()); }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }
    }
}