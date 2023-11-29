using JobHubBot.Interfaces;
using JobHubBot.Services.SendMessageServices;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace JobHubBot.Services.HandleServices
{
    public class UpdateHandlers 
    {
        private readonly ILogger<UpdateHandlers> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IAppDbContext _context;
        private readonly RegisterService _registerService;
        private readonly MainServices _mainService;
        private readonly SendAdvertise _sendAdvertise;
        private readonly JobNotifier _sendJob;

        public UpdateHandlers(ILogger<UpdateHandlers> logger,
            ITelegramBotClient client,
            IAppDbContext dbContext,
            RegisterService registerService,
            MainServices mainManu,
            SendAdvertise sendAdvertise,
            JobNotifier sendJobToUsers)
        {
            _logger = logger;
            _client = client;
            _context = dbContext;
            _registerService = registerService;
            _mainService = mainManu;
            _sendAdvertise = sendAdvertise;
            _sendJob = sendJobToUsers;
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            var handler = update switch
            {
                { Message: { Chat.Type: ChatType.Channel } channelMessage } => BotOnChannelMessageReceived(channelMessage, cancellationToken),
                { ChannelPost: { } channelMessage } => BotOnChannelMessageReceived(channelMessage, cancellationToken),
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update)
            };

            await handler;
        }

        private async Task BotOnChannelMessageReceived(Message channelMessage, CancellationToken cancellationToken)
        {
            var channel = channelMessage.Chat.Title switch
            {
                "JoboHub" => _sendJob.NotifyUsersWithJob(channelMessage, cancellationToken),
                "Advertisements" => _sendAdvertise.SendAll(channelMessage, cancellationToken),
                _ => _sendJob.NotifyUsersWithJob(channelMessage, cancellationToken)
            };

            await channel;
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(callbackQuery.From.Id, "Callback jonatildi", cancellationToken: cancellationToken);
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.Chat.Id, cancellationToken);
            if(user == null)
            {
                await _registerService.ReceivedMessageFromUnregistered(message, cancellationToken);
                return;
            }
            await _mainService.SendMainMenu(message, cancellationToken);
            //await _client.SendTextMessageAsync(chatId: message!.Chat.Id, text: message.Text!, cancellationToken: cancellationToken);
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation(message: "Unknown message type is {}", update.Type);
            return Task.CompletedTask;
        }
    }
}
