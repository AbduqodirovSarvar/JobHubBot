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
        private readonly IStringLocalizer<Messages> _stringLocalizer;
        private readonly IStateManagementService _stateManagementService;
        private readonly ICacheDbService _cacheDbService;
        public MenuServiceHandler(
            ITelegramBotClient botClient,
            IStringLocalizer<Messages> stringLocalizer,
            IStateManagementService stateManagementService,
            ICacheDbService cacheDbService)
        {
            _botClient = botClient;
            _stringLocalizer = stringLocalizer;
            _stateManagementService = stateManagementService;
            _cacheDbService = cacheDbService;
        }

        public async Task ClickStartCommand(long Id, User? user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                var keyboardMarkup = KeyboardsMaster.CreateReplyKeyboardMarkup(new List<string>() 
                                            { Language.uz.ToString(), Language.en.ToString(), Language.ru.ToString() });
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
                _stringLocalizer["feedback_button"],
                _stringLocalizer["setting_button"],
                _stringLocalizer["contact_button"]
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
                _stringLocalizer["setting_change_name_button"],
                _stringLocalizer["setting_change_language_button"],
                _stringLocalizer["setting_change_phone_number_button"],
                _stringLocalizer["setting_change_skill_button"],
                _stringLocalizer["back_button"]
            });
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["choose_option"],
                replyMarkup: keyboardMarkup,
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.settings);
            return;
        }

        public async Task RedirectToFeedbackMenuAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["choose_option"],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.feedback);
            return;
        }

        public async Task RedirectToContactMenuAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _stringLocalizer["for_contact"],
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, Enums.StateList.contact);

            await RedirectToMainMenuAsync(message.Chat.Id, cancellationToken);
            return;
        }
    }
}
