using JobHubBot.Interfaces;
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

        public UpdateHandlers(ILogger<UpdateHandlers> logger, ITelegramBotClient client, IAppDbContext dbContext, RegisterService registerService)
        {
            _logger = logger;
            _client = client;
            _context = dbContext;
            _registerService = registerService;
        }

        public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
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
                { Message: { Chat: { Type: ChatType.Channel } } channelMessage } => BotOnChannelMessageReceived(channelMessage, cancellationToken),
                { ChannelPost: { } channelMessage } => BotOnChannelMessageReceived(channelMessage, cancellationToken),
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update)
            };

            await handler;
        }

        private async Task BotOnChannelMessageReceived(Message channelMessage, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(chatId: 636809820, text: $"{channelMessage.Chat.Title} dan xabar keldi:\n{channelMessage.Text}", cancellationToken: cancellationToken);
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation(message: $"Unknown message type is {update.Type}");
            return Task.CompletedTask;
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == callbackQuery.From.Id, cancellationToken);
            if(user == null)
            {
            }
            throw new NotImplementedException();
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == message.Chat.Id, cancellationToken);
            await _client.SendTextMessageAsync(chatId: message!.Chat.Id, text: message.Text!, cancellationToken: cancellationToken);
        }
    }
}
