using JobHubBot.Db.Enums;
using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Resources;
using JobHubBot.Services.KeyboardServices;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Services.HandleServices
{
    public class MenuServiceHandler : IMenuServiceHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IStringLocalizer<BotLocalizer> _stringLocalizer;
        private readonly IStateManagementService _stateManagementService;
        public MenuServiceHandler(
            ITelegramBotClient botClient,
            IStringLocalizer<BotLocalizer> stringLocalizer,
            IStateManagementService stateManagementService)
        {
            _botClient = botClient;
            _stringLocalizer = stringLocalizer;
            _stateManagementService = stateManagementService;
        }

        public async Task ClickStartCommand(long Id, User? user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                var keyboardMarkup = KeyboardsMaster.CreateReplyKeyboardMarkup(new List<string>() { Language.uz.ToString(), Language.en.ToString(), Language.ru.ToString() });
                await _botClient.SendTextMessageAsync(
                    chatId: Id,
                    text: _stringLocalizer["choose_language"],
                    replyMarkup: keyboardMarkup,
                    cancellationToken: cancellationToken);

                await _stateManagementService.SetUserState(Id, Enums.StateList.register_language);
                return;
            }

            await RedirectToMainMenuAsync(Id, cancellationToken);
            return;
        }

        public async Task RedirectToMainMenuAsync(long Id, CancellationToken cancellationToken)
        {
            var keyboardMarkup = KeyboardsMaster.CreateReplyKeyboardMarkup(new List<string>()
            {
                _stringLocalizer["search"],
                _stringLocalizer["skills"],
                _stringLocalizer["feedback"],
                _stringLocalizer["settings"],
                _stringLocalizer["contact"]
            });
            await _botClient.SendTextMessageAsync(
                chatId: Id,
                text: _stringLocalizer["choose_option"],
                replyMarkup: keyboardMarkup,
                cancellationToken: cancellationToken);

            await _stateManagementService.DeleteState(Id);
            return;
        }

        public async Task RedirectToSettingsMenuAsync(Message message, CancellationToken cancellationToken)
        {
            var keyboardMarkup = KeyboardsMaster.CreateReplyKeyboardMarkup(new List<string>()
            {
                _stringLocalizer["change_name"],
                _stringLocalizer["change_phone"],
                _stringLocalizer["change_language"],
                _stringLocalizer["back"]
            });
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["choose_options"],
                replyMarkup: keyboardMarkup,
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.settings);
            return;
        }

        public async Task RedirectToFeedbackMenuAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["choose_options"],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.feedback);
            return;
        }

        public async Task RedirectToSkillsMenuAsync(Message message, CancellationToken cancellationToken)
        {
            var keyboardMarkup = KeyboardsMaster.CreateReplyKeyboardMarkup(new List<string>()
            {
                _stringLocalizer["add_skill"],
                _stringLocalizer["remove_skill"],
                _stringLocalizer["all_skill"],
                _stringLocalizer["back"]
            });
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["choose_options"],
                replyMarkup: keyboardMarkup,
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.register_language);
            return;
        }

        public async Task RedirectToContactMenuAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["information"],
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.contact);

            await RedirectToMainMenuAsync(message.Chat.Id, cancellationToken);
            return;
        }
        
        public async Task RedirectToSearchMenuAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Not Implemented",
                cancellationToken: cancellationToken);
            return;
        }
    }
}
