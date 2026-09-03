using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using System.Security.Claims;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorio_Reserva repositorio_Reserva;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Inquilino repositorio_Inquilino;
        private readonly ILogger<ReservaController> logger;

        public ReservaController(
            IRepositorio_Reserva repositorio_Reserva,
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Inquilino repositorio_Inquilino,
            ILogger<ReservaController> logger)
        {
            this.repositorio_Reserva = repositorio_Reserva;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Inquilino = repositorio_Inquilino;
            this.logger = logger;
        }

        // =========================
        // CREAR
        // =========================

        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Inmuebles = new SelectList(
                repositorio_Inmueble.ObtenerTodos(),
                "id",
                "Direccion"
            );

            var inquilinos = repositorio_Inquilino.ObtenerTodos()
                .Select(i => new
                {
                    i.id,
                    NombreCompleto = $"{i.Apellido} {i.Nombre}"
                });

            ViewBag.Inquilinos = new SelectList(
                inquilinos,
                "id",
                "NombreCompleto"
            );

            return View();
        }

        [HttpPost]
        public IActionResult Crear(Models.Reserva reserva)
        {
            ModelState.Remove(nameof(reserva.Fecha_Creacion));
            ModelState.Remove(nameof(reserva.Estado));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));

            if (!ModelState.IsValid)
            {
                ViewBag.Inmuebles = new SelectList(
                    repositorio_Inmueble.ObtenerTodos(),
                    "id",
                    "Direccion"
                );

                var inquilinos = repositorio_Inquilino.ObtenerTodos()
                    .Select(i => new
                    {
                        i.id,
                        NombreCompleto = $"{i.Apellido} {i.Nombre}"
                    });

                ViewBag.Inquilinos = new SelectList(
                    inquilinos,
                    "id",
                    "NombreCompleto"
                );

                return View(reserva);
            }

            reserva.Fecha_Creacion = DateTime.Now;
            reserva.Estado = "1";

            int idUsuarioActual = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1"
            );

            reserva.ID_Usuario_Creador = idUsuarioActual;
            reserva.ID_Usuario_Finalizador = idUsuarioActual;

            int id = repositorio_Reserva.Alta(reserva);

            logger.LogInformation(
                $"Se registró correctamente la Reserva con el ID: {id}"
            );

            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ELIMINAR
        // =========================

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var reserva = repositorio_Reserva.ObtenerPorID(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            var reserva = repositorio_Reserva.ObtenerPorID(id);

            if (reserva == null)
            {
                return NotFound();
            }

            repositorio_Reserva.Baja(id);

            logger.LogInformation(
                $"Se eliminó correctamente la Reserva con el ID: {id}"
            );

            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // MODIFICAR
        // =========================

        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var reserva = repositorio_Reserva.ObtenerPorID(id);

            if (reserva == null)
            {
                return NotFound();
            }

            ViewBag.Inmuebles = new SelectList(
                repositorio_Inmueble.ObtenerTodos(),
                "id",
                "Direccion",
                reserva.ID_Inmueble
            );

            var inquilinos = repositorio_Inquilino.ObtenerTodos()
                .Select(i => new
                {
                    i.id,
                    NombreCompleto = $"{i.Apellido} {i.Nombre}"
                });

            ViewBag.Inquilinos = new SelectList(
                inquilinos,
                "id",
                "NombreCompleto",
                reserva.ID_Inquilino
            );

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Models.Reserva reserva)
        {
            if (id != reserva.id)
            {
                return BadRequest();
            }

            ModelState.Remove(nameof(reserva.Fecha_Creacion));

            if (!ModelState.IsValid)
            {
                ViewBag.Inmuebles = new SelectList(
                    repositorio_Inmueble.ObtenerTodos(),
                    "id",
                    "Direccion",
                    reserva.ID_Inmueble
                );

                var inquilinos = repositorio_Inquilino.ObtenerTodos()
                    .Select(i => new
                    {
                        i.id,
                        NombreCompleto = $"{i.Apellido} {i.Nombre}"
                    });

                ViewBag.Inquilinos = new SelectList(
                    inquilinos,
                    "id",
                    "NombreCompleto",
                    reserva.ID_Inquilino
                );

                return View(reserva);
            }

            var reservaExistente =
                repositorio_Reserva.ObtenerPorID(id);

            if (reservaExistente == null)
            {
                return NotFound();
            }

            reserva.ID_Usuario_Finalizador = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1"
            );

            reserva.Fecha_Creacion =
                reservaExistente.Fecha_Creacion;

            repositorio_Reserva.Modificacion(reserva);

            logger.LogInformation(
                $"Se modificó correctamente la Reserva con el ID: {id}"
            );

            TempData["Mensaje"] =
                "Se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // LISTAR
        // =========================

        public IActionResult Index()
        {
            return View(
                repositorio_Reserva.ObtenerTodos()
            );
        }

        // =========================
        // DETALLES
        // =========================

        public IActionResult Detalles(int id)
        {
            var reserva =
                repositorio_Reserva.ObtenerPorID(id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }
    }
}