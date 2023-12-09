using JobHubBot.Db.DbContexts;
using JobHubBot.Interfaces;
using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.CacheServices;
using JobHubBot.Services.HandleServices;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
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
            services.AddSingleton<IConnectionMultiplexer>
                (ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost"));

            services.AddHttpClient("jobohub")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetRequiredService<IConfiguration>().GetSection(BotConfiguration.Configuration).Get<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

            services.AddHostedService<ConfigureWebhook>();

            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("uz-UZ");
                options.AddSupportedUICultures("uz-UZ", "en-US", "ru-RU");
                options.FallBackToParentUICultures = true;
                options.RequestCultureProviders.Clear();
            });

            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.AddScoped<UpdateHandlers>();
            services.AddScoped<IRegisterationServiceHandler, RegisterationServiceHandler>();
            services.AddScoped<IMenuServiceHandler,MenuServiceHandler>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IChannelMessageServiceHandler, ChannelMessageServiceHandler>();
            services.AddScoped<IFeedbackServiceHandler, FeedbackServiceHandler>();
            services.AddScoped<ISettingsServiceHandler, SettingsServiceHandler>();
            services.AddScoped<IStateManagementService, StateManagementService>();
            services.AddScoped<ICacheDbService, RedisService>();

            return services;
        }
    }
}
