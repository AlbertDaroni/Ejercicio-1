using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Reserva {
        int Alta(Reserva reserva);
        int Baja(int id, int idUsuario);
        int FinalizarConPago(int idReserva, DateTime fechaEfectiva, int idUsuario, bool confirmarPagoMulta);
        int Modificacion(Reserva reserva);

        IList<Reserva> ObtenerTodos();
        IList<Reserva> MisReservas(int id);
        Reserva? ObtenerPorID(int id);

        bool ExisteSuperposicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idReservaExcluir = null);

        IList<Reserva> ObtenerVigentesEntreFechas(
        DateTime desde,
        DateTime hasta,
        int pagina = 1,
        int tamPagina = 10
        );

        int ContarVigentesEntreFechas(
        DateTime desde,
        DateTime hasta
        );

        IList<Reserva> ObtenerProximasAFinalizar(
        DateTime desde,
        DateTime hasta,
        int pagina = 1,
        int tamPagina = 10
        );

        int ContarProximasAFinalizar(
            DateTime desde,
            DateTime hasta
        );

    }
}