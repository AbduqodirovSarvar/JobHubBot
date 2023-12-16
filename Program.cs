using JobHubBot.Controllers;
using JobHubBot.Db.DbContexts;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.Services(builder.Configuration); // Assuming there's a method named "Services" in your extension method

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobHubBot API", Version = "v1" });
});

// builder.Host.CreateDefaultBuilder(args)
//     .ConfigureWebHostDefaults(webBuilder =>
//     {
//         webBuilder.UseStartup<Startup>();
//         webBuilder.UseUrls("http://0.0.0.0:80", "https://0.0.0.0:443");
//     });

var app = builder.Build();

// Enable CORS
app.UseCors(options =>
{
    options.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value);

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobHubBot API v1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    // Get controller and action names
    var controllerName = typeof(BotController).Name.Replace("Controller", "", StringComparison.Ordinal);
    var actionName = typeof(BotController).GetMethods()[0].Name;

    // Get route pattern from configuration
    string? pattern = builder.Configuration.GetSection(BotConfiguration.RouteSection).Value;

    // Map custom route
    endpoints.MapControllerRoute(
        name: "jobohub",
        pattern: pattern ?? "/api/bot",
        defaults: new { controller = controllerName, action = actionName });

    endpoints.MapControllers();
});

try
{
    // Apply migrations on startup
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Error applying migrations: {ex.Message}");
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
