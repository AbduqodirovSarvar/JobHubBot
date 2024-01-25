using JobHubBot.Db.DbContexts;
using JobHubBot.Interfaces;
using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Models.Telegram;
using JobHubBot.Services.CacheServices;
using JobHubBot.Services.HandleServices;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Globalization;
using Telegram.Bot;

namespace JobHubBot.Services.Configurations
{
    public static class DepencyInjections
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var botConfigurationSection = configuration.GetSection(BotConfiguration.Configuration);
            services.Configure<BotConfiguration>(botConfigurationSection);
            var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

            // Add PostgreSQL DbContext
            /*services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });*/

            // Add SQLite DbContext
            raw.SetProvider(imp: new SQLite3Provider_e_sqlite3());
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("SQLiteConnection"));
            });
            Batteries.Init();

            services.AddScoped<IAppDbContext, AppDbContext>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost"));

            services.AddHttpClient("jobohub")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetRequiredService<IConfiguration>().GetSection(BotConfiguration.Configuration).Get<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

            services.AddHostedService<ConfigureWebhook>();

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("uz-UZ"),
                        new CultureInfo("ru-RU")
                    };

                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            services.AddScoped<UpdateHandlers>();
            services.AddScoped<IRegisterationServiceHandler, RegisterationServiceHandler>();
            services.AddScoped<IMenuServiceHandler, MenuServiceHandler>();
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
