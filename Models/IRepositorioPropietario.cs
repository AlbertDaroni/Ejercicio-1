using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		Propietario? ObtenerPorEmail(string email);
		IList<Propietario> BuscarPorNombre(string nombre);
	}
}*/

namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioPropietario
    {
        int Alta(Propietario propietario);
        int Baja(int id);
        int Modificacion(Propietario propietario);

        IList<Propietario> ObtenerTodos();
        Propietario? ObtenerPorId(int id);

        Propietario? ObtenerPorEmail(string email);
        IList<Propietario> BuscarPorNombre(string nombre);
    }
}