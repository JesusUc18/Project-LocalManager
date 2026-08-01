using Microsoft.AspNetCore.Builder;
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

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── Servicios MVC ───
builder.Services.AddControllersWithViews();

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
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();