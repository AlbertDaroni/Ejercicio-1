using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria_.Net_Core.Models {
    public class Propietario : Persona {
        public override string ToString() { return $"Propietario: {Nombre} {Apellido} - {DNI}"; }
    }
}