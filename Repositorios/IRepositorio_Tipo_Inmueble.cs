using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios;

public interface IRepositorio_Tipo_Inmueble {
    int Alta (Tipo_Inmueble tipo_Inmueble);
    int Baja (int id);
    int Modificacion (Tipo_Inmueble tipo_Inmueble);

    IList<Tipo_Inmueble> ObtenerTodos ();
    IList<Tipo_Inmueble> ObtenerPorNombre (string nombre);
    Tipo_Inmueble? ObtenerPorID (int id);
}