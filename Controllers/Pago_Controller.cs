using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class Pago_Controller : Controller
    {
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

        // Listar pagos
        [HttpGet]
        public IActionResult Index()
        {
            var pagos = repositorio_Pago.ObtenerTodos();

            return View(pagos);
        }

        // Crear pago
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Reservas = new SelectList(
                repositorio_Reserva.ObtenerTodos(),
                "id",
                "id"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago)
        {
            // Estos datos los controla el sistema
            pago.Estado = "1";
            pago.Fecha_Anulacion = null;

            // Temporal hasta integrar autenticación
            pago.ID_Usuario_Creador = 1;
            pago.ID_Usuario_Finalizador = null;

            // No validamos estos campos porque no los completa el formulario
            ModelState.Remove(nameof(pago.Estado));
            ModelState.Remove(nameof(pago.ID_Usuario_Creador));
            ModelState.Remove(nameof(pago.ID_Usuario_Finalizador));
            ModelState.Remove(nameof(pago.Fecha_Anulacion));

            if (!ModelState.IsValid)
            {
                ViewBag.Reservas = new SelectList(
                    repositorio_Reserva.ObtenerTodos(),
                    "id",
                    "id"
                );

                return View(pago);
            }

            repositorio_Pago.Alta(pago);

            logger.LogInformation(
                $"Se registró correctamente el Pago con ID: {pago.id}"
            );

            TempData["Mensaje"] = "El pago se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

    }
}
