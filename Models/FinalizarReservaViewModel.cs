using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models {
    public class FinalizarReservaViewModel {
        public int ID_Reserva { get; set; }

        [Required(ErrorMessage = "Seleccioná la fecha de finalización.")]
        [Display(Name = "Fecha de finalización efectiva")]
        public DateTime? Fecha_Fin_Efectiva { get; set; }

        [Display(Name = "Confirmo que recibí el pago de la multa")]
        public bool ConfirmarPagoMulta { get; set; }

        // Información para mostrar en pantalla.
        public Reserva? Reserva { get; set; }

        public decimal MultaCalculada { get; set; }

        public bool CalculoRealizado { get; set; }
    }
}