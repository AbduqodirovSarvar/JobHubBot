using JobHubBot.Interfaces;
using JobHubBot.Services.KeyboardServices;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using JobHubBot.Db.Domains;

namespace JobHubBot.Services.HandleServices
{
    public class MainServices
    {
        private readonly ITelegramBotClient _client;
        private readonly IAppDbContext _context;
        public MainServices(ITelegramBotClient client, IAppDbContext dbContext)
        {
            _client = client;
            _context = dbContext;
        }

        public async Task SendMainMenu(Message message, CancellationToken cancellationToken)
        {
            var keyboard = ReplyKeyboardService.MakeReplyKeyboard(new List<string> { "my resume", "my skill", "language" });
            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Assalomu aleykum, Botimizga xush kelibsiz",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }

        public async Task AddNewSkill(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == callbackQuery.From.Id, cancellationToken);
            if(user == null)
            {
                return;
            }
            var job = await _context.Jobs.FirstOrDefaultAsync(x => x.Name == callbackQuery.Data, cancellationToken);
            if(job == null)
            {
                return;
            }

            var userjob = new UserJobs()
            {
                UserId = user.Id,
                Job = job,
            };

            await _context.UsersJobs.AddAsync(userjob, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
