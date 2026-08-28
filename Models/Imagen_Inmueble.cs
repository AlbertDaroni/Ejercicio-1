using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models {
	public class Imagen_Inmueble {
		[Key]
		public int id { get; set; }

		[Required]
		public string URL { get; set; } = string.Empty;

		[Required]
		public int esPortada { get; set; }

		[Required]
		public int orden { get; set; }

		[Required]
		public int id_inmueble { get; set; }

		[NotMapped] // El archivo cargado se marca como No Mapeado en la base de datos
		public IFormFile? Archivo { get; set; } = null;

		public override string toString() {
			return @$"
				URL: {URL}
				Orden: {orden}
			";
		}
	}
}