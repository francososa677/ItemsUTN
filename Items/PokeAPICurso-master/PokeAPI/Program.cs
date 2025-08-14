using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region CorsRules

var CorsRules = "CorsRules";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsRules,
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

#endregion

// Add services to the container.
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Pokemon API",
            Description = "Una aplicación simple para mostrar el funcionamiento de las APIs",
            Version = "v1",
            TermsOfService = null,
            Contact = new OpenApiContact
            {
                // Opcional
            },
            License = new OpenApiLicense
            {
                // Opcional
            }
        });

    // Incluir comentarios XML para documentar endpoints y modelos
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokemon API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors(CorsRules); // Aplicar política CORS

app.UseAuthorization();

app.MapControllers();

app.Run();
