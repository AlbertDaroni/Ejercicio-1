using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios
{
    public interface IRepositorio_Inmueble
    {
        int Alta(Inmueble inmueble);
        int Baja(int id);
        int Modificacion(Inmueble inmueble);

        IList<Inmueble> ObtenerTodos();
        IList<Inmueble> ObtenerFiltrados(
            string? estado,
            int pagina = 1,
            int tamPagina = 10,
            int? idPropietario = null
        );

        int ContarFiltrados(string? estado, int? idPropietario = null);
        Inmueble? ObtenerPorID(int id);
        IList<Inmueble> ObtenerPorDireccion(string direccion);
        IList<InmuebleMasReservadoViewModel> ObtenerMasReservados(
            DateTime desde,
            DateTime hasta,
            int pagina = 1,
            int tamPagina = 10
        );

        int ContarMasReservados(DateTime desde, DateTime hasta);

        IList<Inmueble> ObtenerSinReservas(
        DateTime desde,
        DateTime hasta,
        int pagina = 1,
        int tamPagina = 10
        );

        int ContarSinReservas(
            DateTime desde,
            DateTime hasta
        );

    }

    
}