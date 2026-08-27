using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Repositorios {
	public abstract class RepositorioBase {
		protected readonly IConfiguration configuration;
		protected readonly string connectionString;

		protected RepositorioBase(IConfiguration configuration) {
			this.configuration = configuration;
			// connectionString = configuration["ConnectionStrings:DefaultConnection"]; <--- Otra opción para la conexión
            connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión DefaultConnection"
                );
			// connectionString = configuration["ConnectionStrings:MySql"];
		}
	}
}