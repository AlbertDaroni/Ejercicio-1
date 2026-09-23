using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models {
    public class SignUpViewModel {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Escribí un correo válido")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatorio")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; } = null!;
    }
}