using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;


namespace Inmobiliaria_.Net_Core.Controllers {
    public class CuentaController : Controller {
        private readonly IRepositorio_Usuario repositorio;

        public CuentaController(IRepositorio_Usuario repositorio) {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public IActionResult Login() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login){
            if (!ModelState.IsValid) return View(login);

            var usuario = repositorio.ObtenerPorCorreo(login.Correo);

            if (usuario == null){
                ModelState.AddModelError("", "El correo es incorrescto.");
                return View(login);
            }

            if (usuario.Estado != "1") {
                ModelState.AddModelError("", "El usuario se encuentra inactivo.");
                return View(login);
            }

            if (usuario.Contraseña != login.Contraseña){
                ModelState.AddModelError("", "La contraseña es incorrecta.");
                return View(login);
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home_");
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccesoDenegado() { return View(); }
    }
}