using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Agrega soporte para MVC: controladores + vistas
builder.Services.AddControllersWithViews();

// Inyección de dependencia de los repositorios
builder.Services.AddScoped<IRepositorio_Propietario, Repositorio_PropietarioMySQL>();
builder.Services.AddScoped<IRepositorio_Inquilino, Repositorio_InquilinoMySQL>();
builder.Services.AddScoped<IRepositorio_Inmueble, Repositorio_InmuebleMySQL>();
builder.Services.AddScoped<IRepositorio_Imagen_Inmueble, Repositorio_Imagen_InmuebleMySQL>();
builder.Services.AddScoped<IRepositorio_Reserva, Repositorio_ReservaMySQL>();
builder.Services.AddScoped<IRepositorio_Tipo_Inmueble, Repositorio_Tipo_InmuebleMySQL>();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home_/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

// Ruta MVC por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home_}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();