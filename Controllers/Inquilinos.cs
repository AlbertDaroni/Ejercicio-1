using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Inmobiliaria_.Net_Core.Models {
    [ApiController]
    [Route("api/[controller]")]
    public class Inquilino : ControllerBase {
        private static readonly List<Inquilino> inquilinos = new List<Inquilino>();
        private static int nextId = 1;

        [HttpGet] // Obtiene todos
        public ActionResult<IEnumerable<Inquilino>> GetAll() { return Ok(inquilinos); }

        [HttpGet("{id}")] // Obtiene uno por ID
        public ActionResult<Inquilino> GetByID(int id) {
            var inquilino = inquilinos.FirstOrDefault(p => p.id == id);
            if (inquilino == null) return NotFound("Inquilino no encontrado");
            return inquilino;
        }

        [HttpPost] // Crear
        public ActionResult<Inquilino> Create([FromBody] Inquilino nuevoInquilino) {
            nuevoInquilino.GetByID = nextId++;
            inquilinos.Add(nuevoInquilino);
            return CreatedAtAction(nameof(GetByID), new { id = nuevoInquilino.id }, nuevoInquilino);
        }

        [HttpPut("{id}")] // Actualizar
        public IActionResult Update(int id, [FromBody] Inquilino inquilinoActualizado) {
            var inquilinoExistente = inquilino.FirstOrDefault(p => p.id == id);
            if (inquilinoExistente == null) NotFound("Inquilino no encontrado");

            inquilinoExistente.Nombre = inquilinoActualizado.Nombre;
            inquilinoExistente.Apellido = inquilinoActualizado.Apellido;
            inquilinoExistente.DNI = inquilinoActualizado.DNI;
            inquilinoExistente.Telefono = inquilinoActualizado.Telefono;
            inquilinoExistente.Correo = inquilinoActualizado.Correo;

            return NoContent();
        }

        [HttpDelete("{id}")] // Eliminar
        public IActionResult Delete(int id) {
            var inquilino = inquilinos.FirstOrDefault(p => p.id == id);
            if (inquilino == null) NotFound("Inquilino no encontrado");

            inquilinos.Remove(inquilino);
            return NoContent();
        }
    }
}