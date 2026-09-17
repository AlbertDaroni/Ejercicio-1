using Inmobiliaria_.Net_Core.Models;

namespace Inmobiliaria_.Net_Core.Repositorios {
    public interface IRepositorio_Pago {
        int Alta(Pago pago);
        int Modificacion(Pago pago);
        int Baja(int id);
        
        IList<Pago> ObtenerTodos();
        Pago? ObtenerPorID(int id);
        IList<Pago> ObtenerPorReserva(int idReserva);
    }
}