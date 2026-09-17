using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models {
    public class LoginViewModel {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = string.Empty;
    }
}