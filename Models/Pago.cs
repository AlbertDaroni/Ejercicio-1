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

        [Required]
        public string Concepto { get; set; }

        [Required]
        public DateTime Fecha_Pago { get; set; }

        [Required]
        public DateTime Fecha_Anulacion { get; set; }

        [Required]
        public decimal Importe { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public int ID_Inquilino { get; set; }

        [Required]
        public int ID_Reserva { get; set; }

        [Required]
        public int ID_Usuario_Creador { get; set; }

        [Required]
        public int ID_Usuario_Finalizador { get; set; }

        public override string ToString() {
            return @$"
                Concepto: {Concepto}
                Fecha de pago: {Fecha_Pago}
                Fecha de anulación: {Fecha_Anulacion}
                Importe: {Importe}
            ";
        }
    }
}