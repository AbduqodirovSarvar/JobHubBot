using JobHubBot.Db.DbContexts;
using JobHubBot.Interfaces;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.HandleServices;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace JobHubBot.Services.Configurations
{
    public static class DepencyInjections
    {
        public static IServiceCollection Services(this IServiceCollection services, IConfiguration configuration)
        {
            var botConfigurationSection = configuration.GetSection(BotConfiguration.Configuration);
            services.Configure<BotConfiguration>(botConfigurationSection);
            var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IAppDbContext, AppDbContext>();

            services.AddControllers().AddNewtonsoftJson();

            services.AddHttpClient("jobohub")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetRequiredService<IConfiguration>().GetSection(BotConfiguration.Configuration).Get<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

            services.AddHostedService<ConfigureWebhook>();

            services.AddScoped<UpdateHandlers>();
            services.AddScoped<RegisterService>();
            return services;
        }
    }
}
