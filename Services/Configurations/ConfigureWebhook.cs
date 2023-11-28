using JobHubBot.Models.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace JobHubBot.Services.Configurations
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BotConfiguration _botConfig;
        private readonly ITelegramBotClient _client;

        public ConfigureWebhook(
            ILogger<ConfigureWebhook> logger,
            IServiceProvider serviceProvider,
            IOptions<BotConfiguration> botOptions,
            ITelegramBotClient client)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botConfig = botOptions.Value;
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = $"{_botConfig.HostAddress}{_botConfig.Route}";
            _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);
            await botClient.SetWebhookAsync(
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                secretToken: _botConfig.SecretKey,
                cancellationToken: cancellationToken);
            await _client.SendTextMessageAsync(chatId: 636809820, text: "Webhook has been set", cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
