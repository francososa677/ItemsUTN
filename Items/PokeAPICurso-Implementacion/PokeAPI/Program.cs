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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Pokemon API",
            Description = "Una aplicacion simple para mostrar el funcionamiento de las APIs",
            Version = "v1",
            TermsOfService = null,
            Contact = new OpenApiContact
            {
                // Check for optional parameters
            },
            License = new OpenApiLicense
            {
                // Optional Example
                // Name = "Proprietary",
                // Url = new Uri("https://someURLToLicenseInfo.com")
            }
        });
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
     $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service Manager API V1");
    });
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.Run();
