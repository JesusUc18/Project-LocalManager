using Microsoft.AspNetCore.Builder;
using LocalManager.Application.Services;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;
using LocalManager.Infrastructure.Repositories;

// =============================================================================
// ADR-03: Arquitectura en Capas / Clean Architecture
// =============================================================================
// Regla de Dependencia: Las dependencias apuntan hacia el centro (Domain)
//
//   Presentation → Application → Domain
//        ↓            ↓
//   Infrastructure ←──┘
//
// Flujo de una petición (Vista de Procesos ADR-02):
// 1. HTTP → Controller (Presentation)
// 2. Controller → IService (Application)
// 3. Service → IRepository (Domain)
// 4. Repository → JsonDbContext (Infrastructure)
// 5. JsonDbContext → Archivos JSON (persistencia temporal)
// =============================================================================

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios MVC ───
builder.Services.AddControllersWithViews();

// ─── CAPA INFRASTRUCTURE: Contexto de datos temporal ───
builder.Services.AddSingleton<JsonDbContext>(provider =>
    new JsonDbContext(builder.Configuration.GetValue<string>("JsonDatabase:DataPath") ?? "Data"));

// ─── CAPA INFRASTRUCTURE: Repositorios (implementan interfaces de Domain) ───
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICajaRepository, CajaRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

// ─── CAPA APPLICATION: Servicios de negocio (dependen de interfaces de Domain) ───
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICajaService, CajaService>();
builder.Services.AddScoped<IVentaService, VentaService>();

// ─── EF Core (preparado para SQL Server - descomentar cuando se migre) ───
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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
