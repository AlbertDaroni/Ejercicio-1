using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Imagen_Inmueble {
        int Alta (Imagen_Inmueble imagen_Inmueble);
        int Baja (int id);
        int Modificacion (Imagen_Inmueble imagen_Inmueble);

        IList<Imagen_Inmueble> ObtenerTodos ();
        Imagen_Inmueble? ObtenerPorID (int id);
    }
}