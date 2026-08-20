using Inmobiliaria_.Net_Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Agrega soporte para MVC: controladores + vistas
builder.Services.AddControllersWithViews();

// Inyección de dependencia del repositorio de Propietarios
builder.Services.AddScoped<
    IRepositorioPropietario,
    RepositorioPropietarioMySql
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
