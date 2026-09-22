using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering; // Para usar SelectList.
using Microsoft.AspNetCore.Authorization; // Para usar autorizacion.

namespace Inmobiliaria_.Net_Core.Controllers {
    [Authorize]
    public class Inmueble_Controller : Controller {
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorio_Inmueble repositorio_Inmueble;
        private readonly IRepositorio_Imagen_Inmueble repositorio_Imagen_Inmueble;
        private readonly IRepositorio_Propietario repositorio_Propietario;
        private readonly IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble;
        private readonly ILogger<Inmueble_Controller> logger;

        public Inmueble_Controller(
            IWebHostEnvironment environment,
            IRepositorio_Inmueble repositorio_Inmueble,
            IRepositorio_Propietario repositorio_Propietario,
            IRepositorio_Imagen_Inmueble repositorio_Imagen_Inmueble,
            IRepositorio_Tipo_Inmueble repositorio_Tipo_Inmueble,
            ILogger<Inmueble_Controller> logger
        ) {
            this.environment = environment;
            this.repositorio_Inmueble = repositorio_Inmueble;
            this.repositorio_Imagen_Inmueble = repositorio_Imagen_Inmueble;
            this.repositorio_Propietario = repositorio_Propietario;
            this.repositorio_Tipo_Inmueble = repositorio_Tipo_Inmueble;
            this.logger = logger;
        }

        // Crear
        [HttpGet]
        public IActionResult Crear() {
            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "ApellidoYNombre");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Inmueble inmueble, List<IFormFile> ArchivosImagenes) {
            if (!ModelState.IsValid) {
                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "ApellidoYNombre");

                return View(inmueble);
            }

            repositorio_Inmueble.Alta(inmueble);

            if (ArchivosImagenes != null && ArchivosImagenes.Count > 0) {
                string carpetaUploads = Path.Combine(environment.WebRootPath, "uploads", "inmuebles");
                if (!Directory.Exists(carpetaUploads)) Directory.CreateDirectory(carpetaUploads);

                foreach (var archivo in ArchivosImagenes) {
                    if (archivo.Length > 0) {
                        string nombreArchivoUnico = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                        string rutaCompletaDisco = Path.Combine(carpetaUploads, nombreArchivoUnico);

                        using (var stream = new FileStream(rutaCompletaDisco, FileMode.Create)) { await archivo.CopyToAsync(stream); }

                        var nuevaImagen = new Imagen_Inmueble {
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
        [HttpGet, Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Administrador")]
        public IActionResult ConfirmarEliminar(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();

            var imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id);
            if (imagenes != null) {
                foreach (var imagen in imagenes) { 
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
        [HttpGet]
        public IActionResult Modificar(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();

            ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
            ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "ApellidoYNombre");
            ViewBag.Imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id);

            return View(inmueble);
        }

        [HttpPost]
        public async Task<IActionResult> Modificar(int id, Inmueble inmueble, List<IFormFile> ArchivosImagenes) {
            if (id != inmueble.id) return BadRequest();
            if (!ModelState.IsValid) {
                ViewBag.Tipo_Inmuebles = new SelectList(repositorio_Tipo_Inmueble.ObtenerTodos(), "id", "Nombre");
                ViewBag.Propietarios = new SelectList(repositorio_Propietario.ObtenerTodos(), "id", "ApellidoYNombre");
                ViewBag.Imagenes = repositorio_Imagen_Inmueble.ObtenerTodos(id);

                return View(inmueble);
            }
            
            var inmuebleExistente = repositorio_Inmueble.ObtenerPorID(id);
            if (inmuebleExistente == null) return NotFound();

            if (ArchivosImagenes != null && ArchivosImagenes.Count > 0) {
                string carpetaUploads = Path.Combine(environment.WebRootPath, "uploads", "inmuebles");
                if (!Directory.Exists(carpetaUploads)) Directory.CreateDirectory(carpetaUploads);

                foreach (var archivo in ArchivosImagenes) {
                    if (archivo.Length > 0) {
                        string nombreArchivoUnico = Guid.NewGuid().ToString() + "_" + Path.GetFileName(archivo.FileName);
                        string rutaCompletaDisco = Path.Combine(carpetaUploads, nombreArchivoUnico);

                        using (var stream = new FileStream(rutaCompletaDisco, FileMode.Create)) { await archivo.CopyToAsync(stream); }

                        var nuevaImagen = new Imagen_Inmueble {
                            ID_Inmueble = id,
                            EsPortada = 0,
                            Orden = 0,
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

        // Obtener todos
        [AllowAnonymous]
        public IActionResult Indice() { return View(repositorio_Inmueble.ObtenerTodos()); }

        // Obtener por ID
        public IActionResult Detalles(int id) {
            var inmueble = repositorio_Inmueble.ObtenerPorID(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        // Obtener por dirección
        public IActionResult Direccion(string direccion) { return View(repositorio_Inmueble.ObtenerPorDireccion(direccion)); }
    }
}