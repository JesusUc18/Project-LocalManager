using LocalManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register JSON database services (temporary database)
builder.Services.AddSingleton<JsonDatabaseService>();
builder.Services.AddSingleton<ProductoService>();
builder.Services.AddSingleton<CategoriaService>();
builder.Services.AddSingleton<ClienteService>();
builder.Services.AddSingleton<VentaService>();
builder.Services.AddSingleton<CajaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
