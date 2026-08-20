/*public class Persona {
    [Key]
    [Display(Name = "Código")]
    public int id { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    public string Apellido { get; set; } = string.Empty;
    [Required]
    public string DNI { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;
}*/

using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Persona
    {
        [Key]
        [Display(Name = "Código")]
        public int id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        public string DNI { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;
    }
}