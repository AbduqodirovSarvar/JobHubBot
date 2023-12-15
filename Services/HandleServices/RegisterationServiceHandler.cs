using JobHubBot.Db.Enums;
using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Resources;
using JobHubBot.Services.Enums;
using JobHubBot.Services.KeyboardServices;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Services.HandleServices
{
    public class RegisterationServiceHandler : IRegisterationServiceHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAppDbContext _dbContext;
        private readonly IMenuServiceHandler _menuServiceHandler;
        private readonly IStringLocalizer<BotLocalizer> _stringLocalizer;
        private readonly IStateManagementService _stateManagementService;
        public RegisterationServiceHandler(
            ITelegramBotClient botClient,
            IAppDbContext dbContext,
            IMenuServiceHandler menuServiceHandler,
            IStringLocalizer<BotLocalizer> stringLocalizer,
            IStateManagementService stateManagementService
            )
        {
            _botClient = botClient;
            _dbContext = dbContext;
            _menuServiceHandler = menuServiceHandler;
            _stringLocalizer = stringLocalizer;
            _stateManagementService = stateManagementService;
        }

        private static User? UserObject { get; set; } = null;

        public async Task ReceivedLanguageAsync(Message message, CancellationToken cancellationToken)
        {
            Language language = message.Text switch
            {
                "en" => Language.en,
                "uz" => Language.uz,
                _ => Language.ru,
            };

            UserObject = new User()
            {
                TelegramId = message.Chat.Id,
                Language = language
            };

            await _botClient.SendTextMessageAsync(
                chatId: UserObject.TelegramId,
                text: _stringLocalizer["fullName"],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken
            );

            await _stateManagementService.SetUserState(UserObject.TelegramId, StateList.register_fullname);
            return;
        }

        public async Task ReceivedFullNameAsync(Message message, CancellationToken cancellationToken)
        {
            if (UserObject == null)
            {
                await _menuServiceHandler.ClickStartCommand(message.Chat.Id, UserObject, cancellationToken);
                return;
            }
            UserObject.FullName = message.Text!;

            await _botClient.SendTextMessageAsync(
                chatId: UserObject.TelegramId,
                text: _stringLocalizer["Send Contact"],
                replyMarkup: KeyboardsMaster.CreateContactRequestKeyboardMarkup("Send Contact"),
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(UserObject.TelegramId, StateList.register_contact);
        }



        public async Task ReceivedUserContactAsync(Message message, CancellationToken cancellationToken)
        {
            if (UserObject == null)
            {
                await _menuServiceHandler.ClickStartCommand(message.Chat.Id, UserObject, cancellationToken);
                return;
            }
            UserObject.Phone = message.Contact!.PhoneNumber!;

            await _dbContext.Users.AddAsync(UserObject, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _botClient.SendTextMessageAsync(
                chatId: UserObject.TelegramId,
                text: _stringLocalizer["Congratulations"],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken
            );

            await _menuServiceHandler.RedirectToMainMenuAsync(message.Chat.Id, cancellationToken);

            UserObject = null;

            return;
        }
    }
}
