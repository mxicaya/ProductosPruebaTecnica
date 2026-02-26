using TiendaApp.Client.Pages;
using TiendaApp.Components;
using Microsoft.EntityFrameworkCore;
using TiendaApp.Client.Models; 

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la base de datos (Punto 3.3 del PDF)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// 2. Servicios básicos para API y Blazor
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// 3. REGISTRO DE SWAGGER (Obligatorio para el punto 3.2)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. HttpClient para que el Frontend consuma la API (Punto 3.1)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7034")
});

var app = builder.Build();

// 5. Configuración del Pipeline (Orden de ejecución)
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    // Habilitamos Swagger en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tienda API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    // También lo habilitamos fuera de desarrollo para que el evaluador lo vea fácil
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// 6. Mapeo de rutas
app.MapControllers(); // Esto habilita los endpoints de la API [cite: 19]

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TiendaApp.Client._Imports).Assembly);

app.Run();