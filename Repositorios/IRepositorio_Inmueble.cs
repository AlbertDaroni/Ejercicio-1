using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios;

public interface IRepositorio_Inmueble {
    int Alta (Inmueble inmueble);
    int Baja (int id);
    int Modificacion (Inmueble inmueble);

    IList<Inmueble> ObtenerTodos ();
    Inmueble? ObtenerPorID (int id);
    IList<Inmueble> ObtenerPorDireccion (string direccion);
}