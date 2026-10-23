using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList.
using Microsoft.AspNetCore.Authorization; // Para usar autorización.

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize]
    public class Inmueble_Controller : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Imagen_Inmueble repositorio_Imagen_Inmueble;
        private readonly IRepositorio_Propietario repositorio_Propietario;
        private readonly IRepositorio_Usuario repositorio_Usuario;
        private readonly IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble;
        private readonly ILogger<Inmueble_Controller> logger;

        public Inmueble_Controller(
            IWebHostEnvironment environment,
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Propietario repositorio_Propietario,
            IRepositorio_Usuario repositorio_Usuario,
            IRepositorio_Imagen_Inmueble repositorio_Imagen_Inmueble,
            IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble,
            ILogger<Inmueble_Controller> logger
        )
        {
            this.environment = environment;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Imagen_Inmueble = repositorio_Imagen_Inmueble;
            this.repositorio_Propietario = repositorio_Propietario;
            this.repositorio_Usuario = repositorio_Usuario;
            this.repositorio_Tipo_Inmueble = repositorio_Tipo_Inmueble;
            this.logger = logger;
        }

        // Crear
        [HttpGet]
        public IActionResult Crear()
        {
            if (User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value == "Usuario") return RedirectToAction("Crear", "Propietario_");
            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "Nombre");

            return View(new Inmueble());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Inmueble inmueble, List<IFormFile> ArchivosImagenes)
        {
            //var IDUsuarioActual = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var mensaje in error.Value.Errors)
                    {
                        logger.LogWarning("Error de validación en {Campo}: {Mensaje}", error.Key, mensaje.ErrorMessage);
                    }
                }

                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "Nombre", inmueble.ID_Propietario);

                return View(inmueble);
            }

            //var usuario = repositorio_Usuario.ObtenerPorID(IDUsuarioActual);
            //usuario.Rol = "Propietario";

            repositorio_Inmueble.Alta(inmueble);

            if (ArchivosImagenes != null && ArchivosImagenes.Count > 0)
            {
                string carpetaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "inmuebles");

                Directory.CreateDirectory(carpetaUploads);

                foreach (var archivo in ArchivosImagenes)
                {
                    if (archivo.Length > 0)
                    {
                        string nombreArchivoUnico = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                        string rutaCompletaDisco = Path.Combine(carpetaUploads, nombreArchivoUnico);

                        using (var stream = new FileStream(rutaCompletaDisco, FileMode.Create)) { await archivo.CopyToAsync(stream); }

                        var nuevaImagen = new Imagen_Inmueble
                        {
                            ID_Inmueble = inmueble.id,
                            URL = "/uploads/inmuebles/" + nombreArchivoUnico
                        };

                        repositorio_Imagen_Inmueble.Alta(nuevaImagen);
                    }
                }
            }

            logger.LogInformation($"Se registró correctamente el Inmueble con el ID: {inmueble.id}");
            TempData["Mensaje"] = "Se registró correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Eliminar
        [HttpGet, Authorize(Roles = "Administrador, Propietario")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador, Propietario")]
        public IActionResult ConfirmarEliminar(int id)
        {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();

            var imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id);
            if (imagenes != null)
            {
                foreach (var imagen in imagenes)
                {
                    string rutaRelativaLimpia = imagen.URL.TrimStart('/');
                    string rutaFisicaArchivo = Path.Combine(environment.WebRootPath, rutaRelativaLimpia);

                    if (System.IO.File.Exists(rutaFisicaArchivo)) System.IO.File.Delete(rutaFisicaArchivo);

                    repositorio_Imagen_Inmueble.Baja(imagen.id);
                }
            }

            repositorio_Inmueble.Baja(id);

            logger.LogInformation($"Se eliminó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se eliminó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        // Modificar
        [HttpGet, Authorize(Roles = "Administrador, Propietario")]
        public IActionResult Modificar(int id)
        {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();

            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "ApellidoYNombre", inmueble.ID_Propietario);
            ViewBag.Imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id);

            return View(inmueble);
        }

        [HttpPost, Authorize(Roles = "Administrador, Propietario")]
        public async Task<IActionResult> Modificar(int id, Inmueble inmueble, List<IFormFile> ArchivosImagenes)
        {
            if (id != inmueble.id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "ApellidoYNombre", inmueble.ID_Propietario);
                ViewBag.Imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id);

                return View(inmueble);
            }

            var inmuebleExistente = repositorio_Inmueble.ObtenerPorID(id);
            if (inmuebleExistente == null) return NotFound();

            if (ArchivosImagenes != null && ArchivosImagenes.Count > 0)
            {
                string carpetaUploads = Path.Combine(environment.WebRootPath, "uploads", "inmuebles");
                if (!Directory.Exists(carpetaUploads)) Directory.CreateDirectory(carpetaUploads);

                foreach (var archivo in ArchivosImagenes)
                {
                    if (archivo.Length > 0)
                    {
                        string nombreArchivoUnico = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                        string rutaCompletaDisco = Path.Combine(carpetaUploads, nombreArchivoUnico);

                        using (var stream = new FileStream(rutaCompletaDisco, FileMode.Create)) { await archivo.CopyToAsync(stream); }

                        var nuevaImagen = new Imagen_Inmueble
                        {
                            ID_Inmueble = id,
                            URL = "/uploads/inmuebles/" + nombreArchivoUnico
                        };

                        repositorio_Imagen_Inmueble.Alta(nuevaImagen);
                    }
                }
            }

            repositorio_Inmueble.Modificacion(inmueble);
            logger.LogInformation($"Se modificó correctamente el Inmueble con el ID: {id}");
            TempData["Mensaje"] = "Se modificó correctamente.";

            return RedirectToAction(nameof(Indice));
        }

        [HttpGet]
        public IActionResult BuscarPropietarios(string busqueda = "", int pagina = 1)
        {
            const int tamPagina = 5;

            pagina = Math.Max(1, pagina);

            var propietarios = repositorio_Propietario.BuscarPaginados(
                busqueda,
                pagina,
                tamPagina
            );

            var resultado = propietarios.Select(p => new
            {
                id = p.id,
                texto = $"{p.Nombre} {p.Apellido} - DNI: {p.DNI}"
            });

            return Json(new { resultados = resultado, hayMas = propietarios.Count == tamPagina });
        }

        // Obtener todos
        // Obtener todos
        [AllowAnonymous]
        public IActionResult Indice(
            string? estado,
            int pagina = 1,
            int? idPropietario = null
        )
        {
            if (string.IsNullOrWhiteSpace(estado))
                estado = null;

            if (estado != null && estado != "0" && estado != "1")
                return BadRequest("Estado inválido.");

            Propietario? propietario = null;

            if (idPropietario.HasValue)
            {
                if (idPropietario.Value <= 0)
                    return BadRequest("Propietario inválido.");

                propietario = repositorio_Propietario.ObtenerPorID(
                    idPropietario.Value
                );

                if (propietario == null) return NotFound();
            }

            const int tamPagina = 10;

            int total = repositorio_Inmueble.ContarFiltrados(
                estado,
                idPropietario
            );

            int totalPaginas = Math.Max(
                1,
                (int)Math.Ceiling(total / (double)tamPagina)
            );

            pagina = Math.Clamp(pagina, 1, totalPaginas);

            var inmuebles = repositorio_Inmueble.ObtenerFiltrados(
                estado,
                pagina,
                tamPagina,
                idPropietario
            );

            ViewBag.Estado = estado;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Total = total;
            ViewBag.IdPropietario = idPropietario;
            ViewBag.NombrePropietario = propietario?.ApellidoYNombre;

            return View(inmuebles);
        }

        // Obtener por ID
        [AllowAnonymous]
        public IActionResult Detalles(int id)
        {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            inmueble.Imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id).ToList();
            return View(inmueble);
        }

        // Obtener por dirección
        public IActionResult Direccion(string direccion) { return View(repositorio_Inmueble.ObtenerPorDireccion(direccion)); }

        [HttpGet, Authorize]
        public IActionResult MasReservados(int pagina = 1)
        {
            DateTime hasta = DateTime.Now;
            DateTime desde = hasta.AddDays(-365);

            const int tamPagina = 10;

            int total = repositorio_Inmueble.ContarMasReservados(
                desde,
                hasta
            );

            int totalPaginas = Math.Max(
                1,
                (int)Math.Ceiling(total / (double)tamPagina)
            );

            pagina = Math.Clamp(pagina, 1, totalPaginas);

            var resultados = repositorio_Inmueble.ObtenerMasReservados(
                desde,
                hasta,
                pagina,
                tamPagina
            );

            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(resultados);
        }
    }
}