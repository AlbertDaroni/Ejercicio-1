using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
    public class Inquilino {
        [Key]
        [Display(Name = "Código")]
        public int id { get; set; }

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        public int DNI { get; set; } = "";

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";

        [Required, EmailAddress]
        public string Correo { get; set; } = "";
    }
}