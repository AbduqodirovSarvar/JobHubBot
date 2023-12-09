using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobHubBot.Services.HandleServices
{
    public class FeedbackServiceHandler : IFeedbackServiceHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IMenuServiceHandler _menuServiceHandler;
        private readonly IStringLocalizer<BotLocalizer> _localization;
        public FeedbackServiceHandler(
            ITelegramBotClient telegramBotClient,
            IMenuServiceHandler menuServiceHandler,
            IStringLocalizer<BotLocalizer> stringLocalizer)
        {
            _botClient = telegramBotClient;
            _menuServiceHandler = menuServiceHandler;
            _localization = stringLocalizer;
        }
        public async Task ReceivedFeedbackAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localization["thanks_for_feedback"],
                cancellationToken: cancellationToken);

            await _menuServiceHandler.RedirectToMainMenuAsync(message.Chat.Id, cancellationToken);

            await _botClient.SendTextMessageAsync(
                    chatId: 636809820,
                    text: $"New feedback:\nFrom Id: {message.Chat.Id}",
                    cancellationToken: cancellationToken);

            await _botClient.ForwardMessageAsync(
                    chatId: 636809820,
                    fromChatId: message.Chat.Id,
                    messageId: message.MessageId,
                    cancellationToken: cancellationToken);
            return;
        }
    }
}
