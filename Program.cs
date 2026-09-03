using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Agrega soporte para MVC: controladores + vistas
builder.Services.AddControllersWithViews();

// repositorio de Propietarios
builder.Services.AddScoped<
    IRepositorio_Propietario,
    Repositorio_PropietarioMySQL
>();

// repositorio de Inquilinos
builder.Services.AddScoped<
    IRepositorio_Inquilino,
    Repositorio_InquilinoMySQL
>();

// Repositorio Tipo de Inmueble
builder.Services.AddScoped<
    IRepositorio_Tipo_Inmueble,
    RepositorioTipoInmuebleMySql
>();

// Repositorio de Inmueble
builder.Services.AddScoped<
    IRepositorio_Inmueble,
    Repositorio_InmuebleMySQL
>();


var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Ruta MVC por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
