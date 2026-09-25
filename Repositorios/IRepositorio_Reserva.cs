using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Reserva {
        int Alta (Reserva reserva);
        int Baja(int id, int idUsuario);
        int Modificacion (Reserva reserva);

        IList<Reserva> ObtenerTodos ();
        IList<Reserva> MisReservas (int id);
        Reserva? ObtenerPorID (int id);
    }
}