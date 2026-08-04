using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using LocalManager.Application.Services;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;
using LocalManager.Infrastructure.Repositories;

// =============================================================================
// ADR-03: Arquitectura en Capas / Clean Architecture
// ADR-05: Patrones GOF — Repository + Strategy
// =============================================================================
// PATRÓN STRATEGY: Program.cs es el selector de estrategia de persistencia.
// Cambiar "UseJsonPersistence" en appsettings.json intercambia el motor de datos
// sin modificar ningún repositorio, servicio ni controlador.
// =============================================================================

// Npgsql (driver de PostgreSQL) exige por defecto que los DateTime guardados en columnas
// "timestamp with time zone" vengan en UTC. Como el proyecto usa DateTime.Now (hora local)
// en varias entidades (Venta.Fecha, Caja.FechaApertura, etc.), se restaura el comportamiento
// anterior de Npgsql para aceptar DateTime con Kind=Local/Unspecified sin lanzar excepción.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── Servicios MVC ───
// Login simple con 2 cuentas fijas (ver AccountController) para restringir el acceso
// mientras la app se expone en la nube (Cloudflare Tunnel) antes de tener auth real.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// ─── PATRÓN STRATEGY: Selector de estrategia de persistencia ───
// true  → JsonDbContext  (desarrollo, sin SQL Server)
// false → SqlDbContext   (producción, SQL Server con EF Core) [pendiente de implementar]
bool usarJson = builder.Configuration.GetValue<bool>("UseJsonPersistence");

if (usarJson)
{
    builder.Services.AddSingleton<IDbContext, JsonDbContext>(provider =>
        new JsonDbContext(builder.Configuration.GetValue<string>("JsonDatabase:DataPath") ?? "Data"));
}
else
{
    // PATRÓN STRATEGY: SqlDbContext (PostgreSQL con EF Core / Npgsql)
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IDbContext, SqlDbContext>();
}

// ─── PATRÓN REPOSITORY: Repositorios reciben IDbContext (no JsonDbContext) ───
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICajaRepository, CajaRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

// ─── CAPA APPLICATION: Servicios de negocio ───
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICajaService, CajaService>();
builder.Services.AddScoped<IVentaService, VentaService>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // En producción sí forzamos HTTPS. En Development lo omitimos para poder exponer
    // la app por HTTP a través de un túnel (Cloudflare Tunnel, ngrok, etc.) para demos,
    // ya que ese túnel apunta al puerto HTTP local (62565) y no al HTTPS local (62564),
    // que no es accesible desde fuera de la máquina.
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();