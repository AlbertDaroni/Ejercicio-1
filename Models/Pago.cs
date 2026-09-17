using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
    public class Pago {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(50)]
        public string Concepto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        public DateTime Fecha_Pago { get; set; }

        public DateTime? Fecha_Anulacion { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio")]
        public decimal Importe { get; set; }

        public string Estado { get; set; } = "1";

        [Required(ErrorMessage = "La reserva es obligatoria")]
        public int ID_Reserva { get; set; }

        public int? ID_Usuario_Creador { get; set; }

        public int? ID_Usuario_Finalizador { get; set; }

        public override string ToString() {
            return $@"
                Concepto: {Concepto}
                Fecha de pago: {Fecha_Pago}
                Importe: {Importe}
                Estado: {Estado}
                Reserva: {ID_Reserva}
            ";
        }
    }
}