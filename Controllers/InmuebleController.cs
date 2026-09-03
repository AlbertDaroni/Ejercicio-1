using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorio_Inmueble repositorio;
        private readonly ILogger<InmuebleController> logger;

        public InmuebleController(IRepositorio_Inmueble repositorio, ILogger<InmuebleController> logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        // Crear (dar de Alta)
        [HttpGet]
        public IActionResult Crear() { return View(); }

        [HttpPost]
        public IActionResult Crear(Models.Inmueble inmueble)
        {
            if (!ModelState.IsValid) return View(inmueble);

            int id = repositorio.Alta(inmueble);
            logger.LogInformation($"Se registró correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Eliminar (dar de Baja)
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorio.ObtenerPorID(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public IActionResult ConfirmarEliminar(int id)
        {
            var inmueble = repositorio.ObtenerPorID(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            repositorio.Baja(id);
            logger.LogInformation($"Se eliminó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Modificar (Modificación)
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var inmueble = repositorio.ObtenerPorID(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Models.Inmueble inmueble)
        {
            if (id != inmueble.id) return BadRequest();
            if (!ModelState.IsValid) return View(inmueble);
            var inmuebleExistente = repositorio.ObtenerPorID(id);

            if (inmuebleExistente == null)
            {
                return NotFound();
            }

            repositorio.Modificacion(inmueble);
            logger.LogInformation($"Se modificó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Obtener todos
        public IActionResult Index() { return View(repositorio.ObtenerTodos()); }

        // Obtener por ID
       public IActionResult Detalles(int id)
        {
            var inmueble = repositorio.ObtenerPorID(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        // Obtener por dirección
        public IActionResult Direccion(string direccion) { return View(repositorio.ObtenerPorDireccion(direccion)); }
    }
}