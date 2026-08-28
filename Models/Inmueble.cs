using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
    public class Inmueble {
        [Key]
        public int id { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        public int Cupo { get; set; }

        [Required]
        public decimal Latitud { get; set; }

        [Required]
        public decimal Longitud { get; set; }

        [Required]
        public decimal Precio_Dia { get; set; }

        [Required]
        public decimal Porcentaje_Seña { get; set; }

        [Required]
        public string Estado { get; set; }

        [Required]
        public int ID_Propietario { get; set; }

        [Required]
        public int ID_Tipo { get; set; }

        public override string ToString() {
            return @$"
                Dirección: {Direccion}
                Cupo: {Cupo}
                Latitud: {Latitud}
                Longitud: {Longitud}
                Precio de hoy: {Precio_Dia}
                Porcentaje de la seña: {Porcentaje_Seña}
            ";
        }
    }
}