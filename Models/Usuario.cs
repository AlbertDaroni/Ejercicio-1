using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
    public class Usuario {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio"), EmailAddress]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Avatar { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Rol { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Estado { get; set; }

        public override string ToString() {
            return @$"
                {Avatar}
                {Apellido} {Nombre}
                Correo: {Correo}
                Rol: {Rol}
            ";
        }
    }
}