namespace Inmobiliaria_.Net_Core.Models {
    public class InmuebleMasReservadoViewModel {
        public int ID_Inmueble { get; set; }

        public string Direccion { get; set; } = string.Empty;

        public string Propietario { get; set; } = string.Empty;

        public int CantidadReservas { get; set; }
    }
}