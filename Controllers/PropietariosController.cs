/*using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Inmobiliaria_.Net_Core.Models {
    [ApiController]
    [Route("api/[controller]")]
    public class Propietario : ControllerBase {
        private static readonly List<Propietario> propietarios = new List<Propietario>();
        private static int nextId = 1;

        [HttpGet] // Obtiene todos
        public ActionResult<IEnumerable<Propietario>> GetAll() { return Ok(propietarios); }

        [HttpGet("{id}")] // Obtiene uno por ID
        public ActionResult<Propietario> GetByID(int id) {
            var propietario = propietarios.FirstOrDefault(p => p.id == id);
            if (propietario == null) return NotFound("Propietario no encontrado");
            return propietario;
        }

        [HttpPost] // Crear
        public ActionResult<Propietario> Create([FromBody] Propietario nuevoPropietario) {
            nuevoPropietario.GetByID = nextId++;
            propietarios.Add(nuevoPropietario);
            return CreatedAtAction(nameof(GetByID), new { id = nuevoPropietario.id }, nuevoPropietario);
        }

        [HttpPut("{id}")] // Actualizar
        public IActionResult Update(int id, [FromBody] Propietario propietarioActualizado) {
            var propietarioExistente = propietarios.FirstOrDefault(p => p.id == id);
            if (propietarioExistente == null) NotFound("Propietario no encontrado");

            propietarioExistente.Nombre = propietarioActualizado.Nombre;
            propietarioExistente.Apellido = propietarioActualizado.Apellido;
            propietarioExistente.DNI = propietarioActualizado.DNI;
            propietarioExistente.Telefono = propietarioActualizado.Telefono;
            propietarioExistente.Correo = propietarioActualizado.Correo;

            return NoContent();
        }

        [HttpDelete("{id}")] // Eliminar
        public IActionResult Delete(int id) {
            var propietario = propietarios.FirstOrDefault(p => p.id == id);
            if (propietarioExistente == null) NotFound("Propietario no encontrado");

            propietarios.Remove(propietario);
            return NoContent();
        }
    }
}*/

/* Lo comento por ahora! 

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropietariosController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public PropietariosController(ApplicationDbContext context) { _context = context; }

        [HttpGet]
        public ActionResult<IEnumerable<Propietario>> GetAll()
        {
            return Ok(propietarios);
        }

        [HttpGet("{id}")]
        public ActionResult<Propietario> GetByID(int id)
        {
            var propietario = propietarios.FirstOrDefault(p => p.id == id);

            if (propietario == null)
                return NotFound("Propietario no encontrado");

            return Ok(propietario);
        }

        [HttpPost]
        public ActionResult<Propietario> Create(
            [FromBody] Propietario nuevoPropietario)
        {
            nuevoPropietario.id = nextId++;

            propietarios.Add(nuevoPropietario);

            return CreatedAtAction(
                nameof(GetByID),
                new { id = nuevoPropietario.id },
                nuevoPropietario
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(
            int id,
            [FromBody] Propietario propietarioActualizado)
        {
            var propietarioExistente =
                propietarios.FirstOrDefault(p => p.id == id);

            if (propietarioExistente == null)
                return NotFound("Propietario no encontrado");

            propietarioExistente.Nombre =
                propietarioActualizado.Nombre;

            propietarioExistente.Apellido =
                propietarioActualizado.Apellido;

            propietarioExistente.DNI =
                propietarioActualizado.DNI;

            propietarioExistente.Telefono =
                propietarioActualizado.Telefono;

            propietarioExistente.Correo =
                propietarioActualizado.Correo;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var propietario =
                propietarios.FirstOrDefault(p => p.id == id);

            if (propietario == null)
                return NotFound("Propietario no encontrado");

            propietarios.Remove(propietario);

            return NoContent();
        }
    }
}*/

using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorio;

        public PropietariosController(IRepositorioPropietario repositorio)
        {
            this.repositorio = repositorio;
        }

        // ==========================================
        // LISTAR PROPIETARIOS
        // GET: /Propietarios
        // ==========================================
        public IActionResult Index()
        {
            var propietarios = repositorio.ObtenerTodos();

            return View(propietarios);
        }

        // ==========================================
        // DETALLE DE UN PROPIETARIO
        // GET: /Propietarios/Details/5
        // ==========================================
        public IActionResult Details(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // ==========================================
        // MOSTRAR FORMULARIO DE ALTA
        // GET: /Propietarios/Create
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ==========================================
        // GUARDAR NUEVO PROPIETARIO
        // POST: /Propietarios/Create
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            repositorio.Alta(propietario);

            TempData["Mensaje"] = "El propietario fue registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MOSTRAR FORMULARIO DE MODIFICACIÓN
        // GET: /Propietarios/Edit/5
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // ==========================================
        // GUARDAR MODIFICACIÓN
        // POST: /Propietarios/Edit/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            var propietarioExistente = repositorio.ObtenerPorId(id);

            if (propietarioExistente == null)
            {
                return NotFound();
            }

            repositorio.Modificacion(propietario);

            TempData["Mensaje"] = "El propietario fue actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        // GET: /Propietarios/Delete/5
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // ==========================================
        // ELIMINAR PROPIETARIO
        // POST: /Propietarios/Delete/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);

            if (propietario == null)
            {
                return NotFound();
            }

            repositorio.Baja(id);

            TempData["Mensaje"] = "El propietario fue eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}