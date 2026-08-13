using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria_.Net_Core.Models {
    public class Propietario {
        [Key]
        [Display(Name = "Código Int.")]
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

        [Display(Name = "ID_Propietario")]
        public int id_propietario { get; set; }
        [ForeignKey("id_propietario")]

        public override string ToString() { return $"Propietario: {Nombre} {Apellido}, DNI: {DNI}"; }
    }
}