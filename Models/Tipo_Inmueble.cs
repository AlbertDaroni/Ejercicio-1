using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
    public class Tipo_Inmueble {
        [Key]
        public int id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        public override string ToString() {
            return @$"
                Nombre: {Nombre}
                Descripción: {Descripcion}
            ";
        }
    }
}