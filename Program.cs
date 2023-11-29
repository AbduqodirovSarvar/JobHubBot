using JobHubBot.Controllers;
using JobHubBot.Db.DbContexts;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.Configurations;
using Microsoft.EntityFrameworkCore;

var applicationBuilder = WebApplication.CreateBuilder(args);

// Services are added to the container here.
applicationBuilder.Services.AddControllers();
applicationBuilder.Services.Services(applicationBuilder.Configuration);
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen();

var application = applicationBuilder.Build();

application.UseCors(options =>
{
    options.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

// HTTP request pipeline configuration.
if (application.Environment.IsDevelopment())
{
    application.UseSwagger();
    application.UseSwaggerUI();
}

application.UseRouting();

application.UseEndpoints(endpoints =>
{
    var controllerName = typeof(BotController).Name.Replace("Controller", "", StringComparison.Ordinal);
    var actionName = typeof(BotController).GetMethods()[0].Name;

    string? pattern = applicationBuilder.Configuration.GetSection(BotConfiguration.RouteSection).Value;

    endpoints.MapControllerRoute(
            name: "jobohub",
            pattern: pattern ?? "/api/bot",
            defaults: new { controller = controllerName, action = actionName });

    endpoints.MapControllers();
});

using (var scope = application.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    // Apply all pending migrations
    context.Database.Migrate();
}


application.UseHttpsRedirection();

application.MapControllers();

application.Run();
