using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios;

public interface IRepositorio_Imagen_Inmueble {
    int Alta (Imagen imagen);
    int Baja (int id);
    int Modificacion (Imagen imagen);

    IList<Imagen> ObtenerTodos ();
    Imagen? ObtenerPorID (int id);
}