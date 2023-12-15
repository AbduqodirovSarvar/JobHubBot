using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobHubBot.Services.HandleServices
{
    public class ChannelMessageServiceHandler : IChannelMessageServiceHandler
    {
        private readonly IAppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<ChannelMessageServiceHandler> _logger;
        public ChannelMessageServiceHandler(
            IAppDbContext dbContext,
            ITelegramBotClient telegramBotClient,
            ILogger<ChannelMessageServiceHandler> logger)
        {
            _dbContext = dbContext;
            _botClient = telegramBotClient;
            _logger = logger;
        }
        public async Task ForwardJobMessageForUserAsync(Message message, CancellationToken cancellationToken)
        {
            var text = message.Text ?? message.Caption;
            if(text == null)
            {
                return;
            }
            var telegramIds = await _dbContext.Users
                                    .Include(x => x.Skills)
                                    .Where(user => user.Skills.Any(skill => text.ToLower().Contains(skill.Name.ToLower())))
                                    .Select(x => x.TelegramId)
                                    .ToListAsync(cancellationToken);

            foreach (var Id in telegramIds)
            {
                try
                {
                    await _botClient.ForwardMessageAsync(
                        chatId: Id,
                        fromChatId: message.Chat.Id,
                        messageId: message.MessageId,
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error in process sending advertisements to {user.Id} : {ex.Message}", message.Chat.Id, ex.Message);
                }
            }
        }

        public async Task ForwardMessageToAllUsersAsync(Message message, CancellationToken cancellationToken)
        {
            var telegramIds = await _dbContext.Users.Include(x => x.Skills).Select(x => x.TelegramId).ToListAsync(cancellationToken);
            foreach (var Id in telegramIds)
            {
                try
                {
                    await _botClient.ForwardMessageAsync(
                        chatId: Id,
                        fromChatId: message.Chat.Id,
                        messageId: message.MessageId,
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error in process sending advertisements to {user.Id} : {ex.Message}", message.Chat.Id, ex.Message);
                }
            }
        }
    }
}
