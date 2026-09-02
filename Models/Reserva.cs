using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
    public class Reserva {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Fecha_Creacion { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Fecha_Inicio { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Fecha_Fin_Original { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public DateTime Fecha_Fin_Efectiva { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public decimal Monto_Dia { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public decimal Multa { get; set; } = 0;

        [Required(ErrorMessage = "Este campo es obligatorio")]
        public string Estado { get; set; } = "1";

        [Required]
        public int ID_Inquilino { get; set; }

        [Required]
        public int ID_Inmueble { get; set; }

        [Required]
        public int ID_Usuario_Creador { get; set; }

        [Required]
        public int ID_Usuario_Finalizador { get; set; }

        public override string ToString() {
            return @$"
                Fecha de creación: {Fecha_Creacion}
                Fecha de inicio: {Fecha_Inicio}
                Fecha fin original: {Fecha_Fin_Original}
                Fecha fin efectiva: {Fecha_Fin_Efectiva}
                Monto del día: {Monto_Dia}
                Multa: ${Multa}
            ";
        }
    }
}