using JobHubBot.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace JobHubBot.Services.SendMessageServices
{
    public class SendAdvertise
    {
        private readonly IAppDbContext _context;
        private readonly ITelegramBotClient _client;

        public SendAdvertise(IAppDbContext context, ITelegramBotClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task SendAll(Message message, CancellationToken cancellationToken)
        {
            var usersId = await _context.Users.Select(x => x.Id).ToListAsync(cancellationToken);
            foreach (var Id in usersId)
            {
                try
                {
                    await _client.ForwardMessageAsync(
                        chatId: Id,
                        fromChatId: message.Chat.Id,
                        messageId: message.MessageId,
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging purposes
                    Console.WriteLine($"Failed to send message to user {Id}: {ex.Message}");
                }
            }
        }
    }
}
