using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class Tipo_InmuebleController : Controller
    {
        private readonly IRepositorio_Tipo_Inmueble repositorio;
        private readonly ILogger<Tipo_InmuebleController> logger;

        public Tipo_InmuebleController(
            IRepositorio_Tipo_Inmueble repositorio,
            ILogger<Tipo_InmuebleController> logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Crear (dar de alta)
        [HttpGet]
        public IActionResult Crear() { return View(); }

        [HttpPost]
        public IActionResult Crear(Tipo_Inmueble tipo_Inmueble)
        {
            if (!ModelState.IsValid) return View(tipo_Inmueble);

            int id = repositorio.Alta(tipo_Inmueble);
            logger.LogInformation($"Se registró correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Eliminar (dar de baja)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var tipo = repositorio.ObtenerPorID(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminar(int id)
        {
            var tipo = repositorio.ObtenerPorID(id);

            if (tipo == null)
            {
                return NotFound();
            }

            repositorio.Baja(id);
            logger.LogInformation($"Se eliminó correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Modificar (Modificación)
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var tipo = repositorio.ObtenerPorID(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        // Modificar
        [HttpPost]
        public IActionResult Modificar(int id, Tipo_Inmueble tipo_Inmueble)
        {
            // Verifica que el ID recibido sea el mismo del objeto
            if (id != tipo_Inmueble.id)
            {
                return BadRequest();
            }

            // Verifica que los datos del formulario sean válidos
            if (!ModelState.IsValid)
            {
                return View(tipo_Inmueble);
            }

            // Busca el Tipo de Inmueble en la base de datos
            var tipo = repositorio.ObtenerPorID(id);

            // Si no existe, devuelve error 404
            if (tipo == null)
            {
                return NotFound();
            }

            // Modifica el Tipo de Inmueble
            repositorio.Modificacion(tipo_Inmueble);

            logger.LogInformation($"Se modificó correctamente el Tipo de Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Obtener todos
        public IActionResult Index()
        {
            return View(repositorio.ObtenerTodos());
        }
        // Obtener por nombre
        public IActionResult Nombre(string nombre) { return View(repositorio.ObtenerPorNombre(nombre)); }

        // Obtener por ID
        public IActionResult Detalles(int id)
        {
            var tipo = repositorio.ObtenerPorID(id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }
    }
}