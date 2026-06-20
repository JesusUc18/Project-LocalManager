using Microsoft.AspNetCore.Builder;
using System.Reflection;
using LocalManager.Application.Services;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;
using LocalManager.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

// =============================================================================
// ADR-04: Incorporación de API REST
// =============================================================================

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── Servicios API ───
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ─── Swagger / OpenAPI ───
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Local Manager API",
        Version = "v1",
        Description = "API REST para la gestión de negocios locales. Ventas, inventario, clientes, caja y reportes.",
        Contact = new OpenApiContact
        {
            Name = "Jesús Uc",
            Email = "jesus.uc@example.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ─── CAPA INFRASTRUCTURE: Contexto de datos temporal ───
builder.Services.AddSingleton<JsonDbContext>(provider =>
    new JsonDbContext(builder.Configuration.GetValue<string>("JsonDatabase:DataPath") ?? "Data"));

// ─── CAPA INFRASTRUCTURE: Repositorios ───
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

// ─── CORS ───
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();

// ─── Middleware ───
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Local Manager API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();