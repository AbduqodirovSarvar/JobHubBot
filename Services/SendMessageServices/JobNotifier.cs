using JobHubBot.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobHubBot.Services.SendMessageServices
{
    public class JobNotifier
    {
        private readonly IAppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;

        public JobNotifier(IAppDbContext dbContext, ITelegramBotClient botClient)
        {
            _dbContext = dbContext;
            _botClient = botClient;
        }

        public async Task NotifyUsersWithJob(Message message, CancellationToken cancellationToken)
        {
            var userIds = await _dbContext.Users
                .Include(user => user.Jobs)
                .Where(user => user.Jobs.Any(job => message.Text.Contains(job.Name)))
                .Select(user => user.Id)
                .ToListAsync(cancellationToken);

            foreach (var userId in userIds)
            {
                try
                {
                    await _botClient.ForwardMessageAsync(
                        chatId: userId,
                        fromChatId: message.Chat.Id,
                        messageId: message.MessageId,
                        cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}
