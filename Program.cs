using LibreriaApi.Repositories;
using LibreriaApi.Repositories.Interfaces;
using LibreriaApi.Services;
using LibreriaApi.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "API para gestión de librería",
    });
});

// CORS AMPLIO para permitir acceso desde cualquier IP
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Dependency Injection
builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();

builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>(); 

// Configurar para escuchar en todas las IPs
builder.WebHost.UseUrls("https://*:7100", "http://*:5196");

var app = builder.Build();

// Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Endpoints de información
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => new {
    Status = "OK",
    Timestamp = DateTime.UtcNow,
    Server = Environment.MachineName,
    Message = "Library API funcionando correctamente"
});

Console.WriteLine(" Library API iniciándose...");
Console.WriteLine(" Accesible desde cualquier IP en:");
Console.WriteLine(" HTTPS: https://[TU-IP]:7100");
Console.WriteLine(" HTTP:  http://[TU-IP]:5196");
Console.WriteLine(" Swagger: https://[TU-IP]:7100/swagger");

app.Run();