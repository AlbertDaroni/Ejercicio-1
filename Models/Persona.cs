using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Persona
    {
        [Key]
        [Display(Name = "Código")]
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20, MinimumLength = 7,
            ErrorMessage = "El DNI debe tener entre 7 y 20 caracteres.")]
        [RegularExpression(@"^[0-9]+$",
            ErrorMessage = "El DNI solamente puede contener números.")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(30,
            ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(100,
            ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;
    }
}