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
}