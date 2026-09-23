using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

namespace Inmobiliaria_.Net_Core.Controllers {
    public class Cuenta_Controller : Controller {
        private readonly IRepositorio_Usuario repositorio_Usuario;

        public Cuenta_Controller(IRepositorio_Usuario repositorio) {
            this.repositorio_Usuario = repositorio;
        }

        [HttpGet]
        public IActionResult Login() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login){
            if (!ModelState.IsValid) return View(login);

            var usuario = repositorio_Usuario.ObtenerPorCorreo(login.Correo);
            if (usuario == null) {
                ModelState.AddModelError("", "El correo es incorrecto.");
                return View(login);
            }
            if (usuario.Estado != "1") {
                ModelState.AddModelError("", "El usuario se encuentra inactivo.");
                return View(login);
            }
            if (usuario.Contraseña != login.Contraseña) {
                ModelState.AddModelError("", "La contraseña es incorrecta.");
                return View(login);
            }

            await AutenticarUsuario(usuario);

            return RedirectToAction("Index", "Home_");
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult SignUp () { return View(); }

        [HttpPost]
        public async Task<IActionResult> SignUp (SignUpViewModel signup) {
            if (!ModelState.IsValid) return View(signup);

            var usuario = repositorio_Usuario.ObtenerPorCorreo(signup.Correo);
            if (usuario != null) {
                ModelState.AddModelError("", "Ese correo ya existe. Intentá con otro.");
                return View(signup);
            }

            Usuario aux = new Usuario {
                Nombre = signup.Nombre,
                Apellido = signup.Apellido,
                Contraseña = signup.Contraseña,
                Correo = signup.Correo,
                Avatar = "./wwwroot/uploads/profile_picture/image.png",
                Rol = "Usuario"
            };
            int nuevoID = repositorio_Usuario.Alta(aux);
            aux.id = nuevoID;
            await AutenticarUsuario(aux);

            return RedirectToAction("Index", "Home_");
        }

        [HttpGet]
        public IActionResult AccesoDenegado() { return View(); }

        private async Task AutenticarUsuario (Usuario usuario) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Uri, usuario.Avatar),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}