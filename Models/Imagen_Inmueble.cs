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
		public int EsPortada { get; set; }

		[Required]
		public int Orden { get; set; }

		[Required]
		public int ID_Inmueble { get; set; }

		[NotMapped] // El archivo cargado se marca como No Mapeado en la base de datos
		public IFormFile? Archivo { get; set; } = null;

		public override string ToString() {
			return @$"
				URL: {URL}
				Orden: {Orden}
			";
		}
	}
}