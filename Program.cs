using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Agrega soporte para MVC: controladores + vistas
builder.Services.AddControllersWithViews();
// Agrega soporte para autenticación con cookies.
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
    )
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    });

// Inyección de dependencia de los repositorios
builder.Services.AddScoped<IRepositorio_Propietario, Repositorio_PropietarioMySQL>();
builder.Services.AddScoped<IRepositorio_Inquilino, Repositorio_InquilinoMySQL>();
builder.Services.AddScoped<IRepositorio_Inmueble, Repositorio_InmuebleMySQL>();
builder.Services.AddScoped<IRepositorio_Imagen_Inmueble, Repositorio_Imagen_InmuebleMySQL>();
builder.Services.AddScoped<IRepositorio_Reserva, Repositorio_ReservaMySQL>();
builder.Services.AddScoped<IRepositorio_Tipo_Inmueble, Repositorio_Tipo_InmuebleMySQL>();
builder.Services.AddScoped<IRepositorio_Usuario, Repositorio_UsuarioMySQL>();
builder.Services.AddScoped<IRepositorio_Pago, Repositorio_PagoMySQL>();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home_/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

// Ruta MVC por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home_}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();