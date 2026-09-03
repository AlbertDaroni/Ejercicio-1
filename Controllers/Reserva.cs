using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Reserva : Controller {
        private readonly IRepositorio_Reserva repositorio_Reserva;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Inquilino repositorio_Inquilino;
        private readonly ILogger<Reserva> logger;

        public Reserva(
            IRepositorio_Reserva repositorio_Reserva,
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Inquilinos repositorio_Inquilino,
            ILogger<Reserva> logger
        ) {
            this.repositorio_Reserva = repositorio_Reserva;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Inquilino = repositorio_Inquilino;
            this.logger = logger;
        }

        // Crear (dar de alta)
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion");
            ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "DNI", $"{Apellido} {Nombre}");

            return View();
        }

        [HttpPost]
        public IActionResult Crear(Reserva reserva) {
            ModelState.Remove(nameof(reserva.Fecha_Creacion));
            ModelState.Remove(nameof(reserva.Estado));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));

            if (!ModelState.IsValid) {
                ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion");
                ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "DNI", $"{Apellido} {Nombre}");
                
                return View(reserva);
            }

            reserva.Fecha_Creacion = DateTime.Now;
            reserva.Estado = "1";
            int idUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1"); // Valor "1" como respaldo temporal
            
            reserva.ID_Usuario_Creador = idUsuarioActual;
            reserva.ID_Usuario_Finalizador = idUsuarioActual;

            repositorio_Reserva.Alta(reserva);
            logger.LogInformation($"Se registró correctamente la Reserva con el ID: {id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Eliminar (dar de baja)
        [HttpGet]
        public IActionResult Eliminar(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();

            repositorio_Reserva.Baja(id);
            logger.LogInformation($"Se eliminó correctamente la Reserva con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar (Modificación)
        [HttpGet]
        public IActionResult Modificar(int id) {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();

            ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion", reserva.ID_Inmueble);
            ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "id", $"{Apellido} {Nombre}", reserva.ID_Inquilino);

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Reserva reserva) {
            if (id != reserva.id) return BadRequest();

            ModelState.Remove(nameof(reserva.Fecha_Creacion));
            if (!ModelState.IsValid) {
                ViewBag.Inmuebles = new SelectList(repositorio_Inmueble.ObtenerTodos(), "id", "Direccion", reserva.ID_Inmueble);
                ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(), "id", $"{Apellido} {Nombre}", reserva.ID_Inquilino);

                return View(reserva);
            }

            var reservaExistente = repositorio_Reserva.ObtenerPorID(id);
            if (reservaExistente == null) return NotFound();

            reserva.ID_Usuario_Finalizador = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");
            reserva.Fecha_Creacion = reservaExistente.Fecha_Creacion;

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