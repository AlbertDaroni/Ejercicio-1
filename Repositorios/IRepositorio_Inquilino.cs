using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Inquilino {
        int Alta(Inquilino inquilino);
        int Baja(int id);
        int Modificacion(Inquilino inquilino);

        IList<Inquilino> ObtenerTodos();
        Inquilino? ObtenerPorId(int id);

        Inquilino? ObtenerPorEmail(string email);
        IList<Inquilino> BuscarPorNombre(string nombre);
    }
}