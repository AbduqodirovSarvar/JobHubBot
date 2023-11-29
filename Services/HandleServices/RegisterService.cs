using JobHubBot.Db.Domains;
using JobHubBot.Db.Enums;
using JobHubBot.Interfaces;
using JobHubBot.Services.CacheServices;
using JobHubBot.Services.KeyboardServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = JobHubBot.Db.Domains.User;

namespace JobHubBot.Services.HandleServices
{
    public class RegisterService
    {
        private readonly ITelegramBotClient _client;
        private readonly StateMemoryService _stateMemory;
        private readonly IAppDbContext _context;
        private readonly MainServices _main;
        public RegisterService(
            ITelegramBotClient client,
            StateMemoryService stateMemoryService,
            IAppDbContext appDbContext,
            MainServices main)
        {
            _client = client;
            _stateMemory = stateMemoryService;
            _context = appDbContext;
            _main = main;
        }

        private static User? UserObject { get; set; } = null;

        public async Task ReceivedMessageFromUnregistered(Message message, CancellationToken cancellationToken)
        {
            var state = _stateMemory.GetUserState(message.Chat.Id);
            Console.WriteLine($"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA_____________{state}____________AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            var st = state switch
            {
                "language" => ReceivedLanguage(UserObject!, message, cancellationToken),
                "contact" => ReceivedContact(UserObject!, message, cancellationToken),
                _ => ReceivedStartCommand(message, cancellationToken)
            };
            await st;

            return;
        }

        private async Task ReceivedStartCommand(Message message, CancellationToken cancellationToken)
        {
            UserObject = new User
            {
                Id = message.Chat.Id,
                FirstName = message.From?.FirstName ?? "empty",
                LastName = message.From?.LastName ?? "empty"
            };

            var languageKeyboard = ReplyKeyboardService.MakeReplyKeyboard(new List<string> { "en", "uz", "ru" });

            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Assalomu aleykum, Ro'yhatdan o'tish uchun tilni tanlang:",
                replyMarkup: languageKeyboard,
                cancellationToken: cancellationToken);

            await _stateMemory.SetUserState(UserObject.Id, "language");

            return;
        }

        private async Task ReceivedLanguage(User user, Message message, CancellationToken cancellationToken)
        {
            Language lang = message.Text switch
            {
                "en" => Language.en,
                "uz" => Language.uz,
                _ => Language.ru,
            };

            user.Language = lang;

            var contactKeyboard = new ReplyKeyboardMarkup(
                new KeyboardButton("Send contact") { RequestContact = true }
                ) {ResizeKeyboard = true };

            await _client.SendTextMessageAsync(
                chatId: user.Id,
                text: "Kontak jo'nating",
                replyMarkup: contactKeyboard,
                cancellationToken: cancellationToken
                );

            await _stateMemory.SetUserState(user.Id, "contact");

            return;
        }

        private async Task ReceivedContact(User user, Message message, CancellationToken cancellationToken)
        {
            try
            {
                user.Phone = message.Contact?.PhoneNumber ?? "+998932340316";
            }
            catch(Exception)
            {
                await ReceivedStartCommand(message, cancellationToken);
            }

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _stateMemory.DeleteState(user.Id);

            UserObject = null;// destroy user object or make null

            await _client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Ro'yhatdan muvaffaqiyatli o'tdingiz!",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await _main.SendMainMenu(message, cancellationToken);

            return;
        }

    }
}
