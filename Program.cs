using Swashbuckle.AspNetCore.SwaggerGen;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Ajout des services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo() 
            {
                Title="TennisApi", 
                Version="v1",
                Description = "API pour la gestion des statistiques de joueurs de tennis"
            }
            );
        });

        // Injection des dépendances
        builder.Services.AddScoped<IPlayerService, PlayerService>();
        builder.Services.AddSingleton(builder.Environment);

        // Configuration CORS (important pour le déploiement)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure l'HTTP pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        // Endpoint racine pour tester
        app.MapGet("/", () => Results.Ok(new
        {
            message = "Tennis API is running!",
            endpoints = new[] {
    "GET /api/players",
    "GET /api/players/{id}",
    "GET /api/players/statistics",
    "POST /api/players"
}
        }));

        app.Run();
    }
}