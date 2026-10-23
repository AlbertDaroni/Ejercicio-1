using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using System.Security.Claims; // Para la autentucación.
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList.
using Microsoft.AspNetCore.Authorization; // Para usar autorización.

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize]
    public class Reserva_Controller : Controller
    {
        private readonly IRepositorio_Reserva repositorio_Reserva;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Inquilino repositorio_Inquilino;
        private readonly ILogger<Reserva_Controller> logger;

        public Reserva_Controller(
            IRepositorio_Reserva repositorio_Reserva,
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Inquilino repositorio_Inquilino,
            ILogger<Reserva_Controller> logger
        )
        {
            this.repositorio_Reserva = repositorio_Reserva;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Inquilino = repositorio_Inquilino;
            this.logger = logger;
        }

        // Crear
        [HttpGet]
        public IActionResult Crear(int? ID_Inmueble)
        {
            if (ID_Inmueble == null) return RedirectToAction("Indice", "Inmueble_");

            var inmueble = repositorio_Inmueble.ObtenerPorID(ID_Inmueble.Value);
            if (inmueble == null) return NotFound();

            if (inmueble.Estado != "1")
            {
                TempData["Mensaje"] =
                    "El inmueble está suspendido y no admite reservas.";

                return RedirectToAction("Indice", "Inmueble_");
            }

            var reserva = new Reserva
            {
                ID_Inmueble = inmueble.id,
                Monto_Dia = inmueble.Precio_Dia,
                Inmueble = inmueble
            };

            ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(null), "id", "ApellidoYNombre");

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Crear(Reserva reserva)
        {
            ModelState.Remove(nameof(reserva.Estado));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));
            /*
                        if (!ModelState.IsValid) retornar();
                        if (reserva.Fecha_Inicio < DateTime.Now) retornar();
                        if (reserva.Fecha_Inicio >= reserva.Fecha_Fin_Original) retornar();
                        if (reserva.Fecha_Inicio <= reserva.Fecha_Fin_Efectiva) retornar();
                        if (reserva.Fecha_Fin_Original > reserva.Fecha_Fin_Efectiva) retornar();
            */

            if (reserva.ID_Inquilino <= 0 ||
                repositorio_Inquilino.ObtenerPorID(reserva.ID_Inquilino) == null)
            {
                ModelState.AddModelError(
                    nameof(reserva.ID_Inquilino),
                    "Seleccioná un inquilino válido."
                );
            }

            if (!ModelState.IsValid) return retornar();

            if (reserva.Fecha_Inicio < DateTime.Now)
            {
                ModelState.AddModelError(nameof(reserva.Fecha_Inicio), "La fecha de inicio no puede ser anterior a la fecha actual.");
                return retornar();
            }

            if (reserva.Fecha_Inicio >= reserva.Fecha_Fin_Original)
            {
                ModelState.AddModelError(nameof(reserva.Fecha_Fin_Original), "La fecha de finalización debe ser posterior a la fecha de inicio.");
                return retornar();
            }

            reserva.Fecha_Fin_Efectiva = reserva.Fecha_Fin_Original;

            int IDUsuarioActual = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            reserva.ID_Usuario_Creador = IDUsuarioActual;

            var inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
            if (inmueble == null) return NotFound();

            if (inmueble.Estado != "1")
            {
                TempData["Mensaje"] =
                    "El inmueble está suspendido y no admite reservas.";

                return RedirectToAction("Indice", "Inmueble_");
            }

            reserva.Monto_Dia = inmueble.Precio_Dia;

            bool existeSuperposicion = repositorio_Reserva.ExisteSuperposicion(
                reserva.ID_Inmueble,
                reserva.Fecha_Inicio,
                reserva.Fecha_Fin_Efectiva
            );

            if (existeSuperposicion)
            {
                ModelState.AddModelError("", "El inmueble ya posee una reserva que se superpone con las fechas seleccionadas.");
                return retornar();
            }

            reserva.Fecha_Creacion = DateTime.Now;
            repositorio_Reserva.Alta(reserva);

            logger.LogInformation($"Se registró correctamente la Reserva con el ID: {reserva.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Indice));

            IActionResult retornar()
            {
                reserva.Inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
                ViewBag.Inquilinos = new SelectList(repositorio_Inquilino.ObtenerTodos(null), "id", "ApellidoYNombre", reserva.ID_Inquilino);

                return View(reserva);
            }
        }

        private decimal CalcularMulta(Reserva reserva, DateTime fechaEfectiva)
        {
            if (fechaEfectiva < reserva.Fecha_Inicio || fechaEfectiva > reserva.Fecha_Fin_Original)
            {
                throw new ArgumentException("La fecha efectiva debe estar entre el inicio y el fin original.");
            }

            if (fechaEfectiva == reserva.Fecha_Fin_Original) return 0m;

            decimal duracionOriginal = (decimal)(reserva.Fecha_Fin_Original - reserva.Fecha_Inicio).TotalDays;

            if (duracionOriginal <= 0m) throw new ArgumentException("La reserva debe tener una duración mayor que cero.");

            decimal tiempoCumplido = (decimal)(fechaEfectiva - reserva.Fecha_Inicio).TotalDays;
            decimal diasRestantes = (decimal)(reserva.Fecha_Fin_Original - fechaEfectiva).TotalDays;
            decimal porcentaje = tiempoCumplido < duracionOriginal / 2m ? 0.50m : 0.25m;
            decimal importeRestante = diasRestantes * reserva.Monto_Dia;

            return Math.Round(importeRestante * porcentaje, 2, MidpointRounding.AwayFromZero);
        }

        // Eliminar
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();
            if (reserva.Estado != "1")
            {
                TempData["Mensaje"] = "La reserva ya está finalizada.";
                return RedirectToAction(nameof(Indice));
            }

            var fecha = DateTime.Now;

            if (fecha < reserva.Fecha_Inicio) fecha = reserva.Fecha_Inicio;
            if (fecha > reserva.Fecha_Fin_Original) fecha = reserva.Fecha_Fin_Original;

            return View(new FinalizarReservaViewModel
            {
                ID_Reserva = reserva.id,
                Fecha_Fin_Efectiva = fecha,
                Reserva = reserva
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarEliminar(FinalizarReservaViewModel modelo)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int idUsuario)) return Unauthorized();

            var reserva = repositorio_Reserva.ObtenerPorID(modelo.ID_Reserva);

            if (reserva == null) return NotFound();

            ModelState.Remove(nameof(modelo.Reserva));
            ModelState.Remove(nameof(modelo.MultaCalculada));
            ModelState.Remove(nameof(modelo.CalculoRealizado));

            modelo.Reserva = reserva;
            modelo.CalculoRealizado = false;
            modelo.MultaCalculada = 0m;

            if (reserva.Estado != "1") ModelState.AddModelError("", "La reserva ya está finalizada.");
            if (ModelState.IsValid && modelo.Fecha_Fin_Efectiva.HasValue)
            {
                try
                {
                    modelo.MultaCalculada = CalcularMulta(reserva, modelo.Fecha_Fin_Efectiva.Value);
                    modelo.CalculoRealizado = true;

                    if (modelo.MultaCalculada > 0m && !modelo.ConfirmarPagoMulta)
                    {
                        ModelState.AddModelError(nameof(modelo.ConfirmarPagoMulta), "Confirmá que recibiste el pago de la multa.");
                    }

                    if (ModelState.IsValid)
                    {
                        repositorio_Reserva.FinalizarConPago(reserva.id, modelo.Fecha_Fin_Efectiva.Value, idUsuario, modelo.ConfirmarPagoMulta);
                        TempData["Mensaje"] = "La reserva se finalizó correctamente.";
                        return RedirectToAction(nameof(Indice));
                    }
                }
                catch (ArgumentException error)
                {
                    ModelState.AddModelError(nameof(modelo.Fecha_Fin_Efectiva), error.Message);
                }
                catch (InvalidOperationException error)
                {
                    modelo.CalculoRealizado = false;
                    ModelState.AddModelError("", error.Message);
                }
            }

            return View("Eliminar", modelo);
        }

        // Modificar
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            var inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);

            if (reserva == null || inmueble == null) return NotFound();
            if (inmueble != null)
            {
                var nuevaReserva = new Reserva
                {
                    id = reserva.id,
                    Fecha_Creacion = reserva.Fecha_Creacion,
                    Fecha_Inicio = reserva.Fecha_Inicio,
                    Fecha_Fin_Original = reserva.Fecha_Fin_Original,
                    Fecha_Fin_Efectiva = reserva.Fecha_Fin_Efectiva,
                    Monto_Dia = reserva.Monto_Dia,
                    Multa = reserva.Multa,
                    Estado = reserva.Estado,
                    ID_Inquilino = reserva.ID_Inquilino,
                    ID_Inmueble = reserva.ID_Inmueble,
                    ID_Usuario_Creador = reserva.ID_Usuario_Creador,
                    ID_Usuario_Finalizador = reserva.ID_Usuario_Finalizador,
                    Inmueble = inmueble
                };

                return View(nuevaReserva);
            }

            return View(reserva);
        }

        [HttpPost]
        public IActionResult Modificar(int id, Reserva reserva)
        {
            if (id != reserva.id) return BadRequest();

            ModelState.Remove(nameof(reserva.Fecha_Creacion));
            ModelState.Remove(nameof(reserva.ID_Inquilino));
            ModelState.Remove(nameof(reserva.ID_Usuario_Creador));
            ModelState.Remove(nameof(reserva.ID_Usuario_Finalizador));

            if (!ModelState.IsValid)
            {
                reserva.Inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
                return View(reserva);
            }

            var reservaExistente = repositorio_Reserva.ObtenerPorID(id);
            if (reservaExistente == null) return NotFound();

            bool existeSuperposicion = repositorio_Reserva.ExisteSuperposicion(
                reserva.ID_Inmueble,
                reserva.Fecha_Inicio,
                reserva.Fecha_Fin_Efectiva,
                reserva.id
            );

            if (existeSuperposicion)
            {
                ModelState.AddModelError("", "El inmueble ya posee una reserva que se superpone con las fechas seleccionadas.");
                reserva.Inmueble = repositorio_Inmueble.ObtenerPorID(reserva.ID_Inmueble);
                return View(reserva);
            }

            reserva.Fecha_Creacion = reservaExistente.Fecha_Creacion;
            reserva.ID_Inquilino = reservaExistente.ID_Inquilino;
            reserva.ID_Usuario_Creador = reservaExistente.ID_Usuario_Creador;
            reserva.ID_Usuario_Finalizador = reservaExistente.ID_Usuario_Finalizador;

            repositorio_Reserva.Modificacion(reserva);

            logger.LogInformation($"Se modificó correctamente la Reserva con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Obtener todos
        public IActionResult Indice(bool todos = false)
        {
            int IDUsuarioActual = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            IList<Reserva> reservas;

            if (User.IsInRole("Administrador") && todos)
            {
                reservas = repositorio_Reserva.ObtenerTodos();
                ViewBag.Todas = true;
            }
            else
            {
                reservas = repositorio_Reserva.MisReservas(IDUsuarioActual);
                ViewBag.Todas = false;
            }

            return View(reservas);
        }

        // Obtener por ID
        public IActionResult Detalles(int id)
        {
            var reserva = repositorio_Reserva.ObtenerPorID(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpGet]
        public IActionResult Renovar(int id)
        {
            var original = repositorio_Reserva.ObtenerPorID(id);
            if (original == null) return NotFound();

            var inmueble = repositorio_Inmueble.ObtenerPorID(
                original.ID_Inmueble
            );

            if (inmueble == null) return NotFound();
            if (inmueble.Estado != "1")
            {
                TempData["Mensaje"] = "El inmueble está suspendido y no admite nuevas reservas.";
                return RedirectToAction(nameof(Indice));
            }

            var inicio = original.Fecha_Fin_Efectiva;

            if (inicio < DateTime.Now) inicio = DateTime.Now.AddHours(1);

            return View(new RenovarReservaViewModel
            {
                ID_ReservaOriginal = original.id,
                Fecha_Inicio = inicio,
                Fecha_Fin = inicio.AddDays(1),
                Monto_Dia = inmueble.Precio_Dia,
                ReservaOriginal = original
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Renovar(RenovarReservaViewModel modelo)
        {
            var original = repositorio_Reserva.ObtenerPorID(modelo.ID_ReservaOriginal);

            if (original == null) return NotFound();

            var inmueble = repositorio_Inmueble.ObtenerPorID(original.ID_Inmueble);

            if (inmueble == null) return NotFound();

            ModelState.Remove(nameof(modelo.ReservaOriginal));
            modelo.ReservaOriginal = original;

            if (inmueble.Estado != "1") ModelState.AddModelError("", "El inmueble está suspendido y no admite nuevas reservas.");

            if (modelo.Fecha_Inicio.HasValue)
            {
                if (modelo.Fecha_Inicio.Value < DateTime.Now)
                {
                    ModelState.AddModelError(
                        nameof(modelo.Fecha_Inicio),
                        "El inicio no puede ser anterior al momento actual."
                    );
                }

                if (modelo.Fecha_Inicio.Value < original.Fecha_Fin_Efectiva)
                {
                    ModelState.AddModelError(
                        nameof(modelo.Fecha_Inicio),
                        "La renovación debe comenzar desde el fin efectivo de la reserva original."
                    );
                }
            }

            if (modelo.Fecha_Inicio.HasValue &&
                modelo.Fecha_Fin.HasValue &&
                modelo.Fecha_Fin.Value <= modelo.Fecha_Inicio.Value)
            {
                ModelState.AddModelError(
                    nameof(modelo.Fecha_Fin),
                    "El fin debe ser posterior al inicio."
                );
            }

            if (!ModelState.IsValid) return View(modelo);

            bool superpuesta = repositorio_Reserva.ExisteSuperposicion(
                original.ID_Inmueble,
                modelo.Fecha_Inicio!.Value,
                modelo.Fecha_Fin!.Value
            );

            if (superpuesta)
            {
                ModelState.AddModelError("", "El inmueble ya está reservado en esas fechas.");
                return View(modelo);
            }

            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int idUsuario)) return Unauthorized();

            var nueva = new Reserva
            {
                Fecha_Creacion = DateTime.Now,
                Fecha_Inicio = modelo.Fecha_Inicio.Value,
                Fecha_Fin_Original = modelo.Fecha_Fin.Value,
                Fecha_Fin_Efectiva = modelo.Fecha_Fin.Value,
                Monto_Dia = modelo.Monto_Dia!.Value,
                Multa = 0m,
                Estado = "1",
                ID_Inquilino = original.ID_Inquilino,
                ID_Inmueble = original.ID_Inmueble,
                ID_Usuario_Creador = idUsuario,
                ID_Usuario_Finalizador = null
            };

            repositorio_Reserva.Alta(nueva);

            TempData["Mensaje"] = $"Se creó la reserva #{nueva.id} como renovación de la #{original.id}.";

            return RedirectToAction(nameof(Detalles), new { id = nueva.id });
        }

    [HttpGet, Authorize(Roles = "Administrador")]
    public IActionResult VigentesEntreFechas(
        DateTime? desde,
        DateTime? hasta,
        int pagina = 1)
    {
        DateTime fechaDesde = (desde ?? DateTime.Today).Date;
        DateTime fechaHasta = (hasta ?? DateTime.Today.AddDays(30)).Date;

        if (fechaHasta < fechaDesde)
        {
            ModelState.AddModelError(
                "",
                "La fecha final no puede ser anterior a la fecha inicial."
            );

            return View(new List<Reserva>());
        }

        // Sumamos un día para incluir toda la fecha final.
        DateTime hastaExclusivo = fechaHasta.AddDays(1);

        const int tamPagina = 10;

        int total = repositorio_Reserva.ContarVigentesEntreFechas(
            fechaDesde,
            hastaExclusivo
        );

        int totalPaginas = Math.Max(
            1,
            (int)Math.Ceiling(total / (double)tamPagina)
        );

        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var reservas = repositorio_Reserva.ObtenerVigentesEntreFechas(
            fechaDesde,
            hastaExclusivo,
            pagina,
            tamPagina
        );

        ViewBag.Desde = fechaDesde;
        ViewBag.Hasta = fechaHasta;
        ViewBag.Total = total;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;

        return View(reservas);
    }


    }
}