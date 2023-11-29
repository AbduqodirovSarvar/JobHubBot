using JobHubBot.Db.DbContexts;
using JobHubBot.Interfaces;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.CacheServices;
using JobHubBot.Services.HandleServices;
using JobHubBot.Services.SendMessageServices;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Telegram.Bot;
using IDatabase = StackExchange.Redis.IDatabase;

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
            services.AddScoped<MainServices>();
            services.AddScoped<StateMemoryService>();
            services.AddScoped<JobNotifier>();
            services.AddScoped<SendAdvertise>();
            services.AddScoped<SaveFile>();

            /*services.AddScoped<IConnectionMultiplexer>(provider =>
            {
                var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
                var config = ConfigurationOptions.Parse(redisConnectionString);
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped<IDatabase>(provider =>
            {
                var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                return connectionMultiplexer.GetDatabase();
            });*/

            return services;
        }
    }
}
