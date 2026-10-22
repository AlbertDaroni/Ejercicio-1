using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models {
    public class RenovarReservaViewModel {
        public int ID_ReservaOriginal { get; set; }

        [Required(ErrorMessage = "Ingresá la fecha de inicio.")]
        [Display(Name = "Nuevo inicio")]
        public DateTime? Fecha_Inicio { get; set; }

        [Required(ErrorMessage = "Ingresá la fecha de finalización.")]
        [Display(Name = "Nuevo fin")]
        public DateTime? Fecha_Fin { get; set; }

        [Required(ErrorMessage = "Ingresá el monto diario.")]
        [Range(typeof(decimal), "0.01", "99999999.99",
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "El monto diario debe ser mayor que cero y no superar 99999999,99.")]
        [Display(Name = "Nuevo monto diario")]
        public decimal? Monto_Dia { get; set; }

        public Reserva? ReservaOriginal { get; set; }
    }
}