using JobHubBot.Controllers;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.Configurations;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Services(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(options =>
{
    options.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    var controllerName = typeof(BotController).Name.Replace("Controller", "", StringComparison.Ordinal);
    var actionName = typeof(BotController).GetMethods()[0].Name;

    string? pattern = builder.Configuration.GetSection(BotConfiguration.RouteSection).Value;

    endpoints.MapControllerRoute(
            name: "jobohub",
            pattern: pattern ?? "/api/bot",
            defaults: new { controller = controllerName, action = actionName });

    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
