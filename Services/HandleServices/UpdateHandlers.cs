using JobHubBot.Interfaces;
using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Resources;
using JobHubBot.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Services.HandleServices
{
    public class UpdateHandlers
    {
        private readonly ILogger<UpdateHandlers> _logger;
        private readonly IAppDbContext _dbContext;
        private readonly IRegisterationServiceHandler _registerationService;
        private readonly IMenuServiceHandler _menuServiceHandler;
        private readonly ICacheDbService _cacheDbService;
        private readonly IStateManagementService _stateManagementService;
        private readonly IChannelMessageServiceHandler _channelMessageServiceHandler;
        private readonly ISettingsServiceHandler _settingsServiceHandler;
        private readonly IFeedbackServiceHandler _feedbackService;

        public UpdateHandlers(
            ILogger<UpdateHandlers> logger,
            IAppDbContext dbContext,
            IRegisterationServiceHandler registerationService,
            IMenuServiceHandler menuServiceHandler,
            ICacheDbService cacheDbService,
            IChannelMessageServiceHandler channelMessageServiceHandler,
            IStateManagementService stateManagementService,
            ISettingsServiceHandler settingsServiceHandler,
            IFeedbackServiceHandler feedbackServiceHandler
            )
        {
            _logger = logger;
            _dbContext = dbContext;
            _registerationService = registerationService;
            _menuServiceHandler = menuServiceHandler;
            _stateManagementService = stateManagementService;
            _cacheDbService = cacheDbService;
            _settingsServiceHandler = settingsServiceHandler;
            _channelMessageServiceHandler = channelMessageServiceHandler;
            _cacheDbService = cacheDbService;
            _feedbackService = feedbackServiceHandler;
        }

        public Task HandleErrorAsync(Exception exception)
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
                { Message: { Chat.Type: ChatType.Channel } channelMessage } => BotOnChannelMessageReceived(channelMessage, cancellationToken),
                { ChannelPost: { } channelMessage } => BotOnChannelMessageReceived(channelMessage, cancellationToken),
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update)
            };

            await handler;
        }

        private async Task BotOnChannelMessageReceived(Message channelMessage, CancellationToken cancellationToken)
        {
            try
            {
                var channel = channelMessage.Chat.Title switch
                {
                    "Advertisements" => _channelMessageServiceHandler.ForwardMessageToAllUsersAsync(channelMessage, cancellationToken),
                    _ => _channelMessageServiceHandler.ForwardJobMessageForUserAsync(channelMessage, cancellationToken)
                };

                await channel;
            }
            catch (Exception exception)
            {
                _logger.LogInformation("Error while coming channel message: ", exception.Message.ToString());
            }
            return;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            Task forward;
            var state = _stateManagementService.GetUserState(message.Chat.Id);

            var user = await _cacheDbService.GetObjectAsync<User>(message.Chat.Id.ToString());
            if (user == null)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
                if (message.Text == "/start" || message.Text == "/help")
                {
                    await _menuServiceHandler.ClickStartCommand(message.Chat.Id, user, cancellationToken);
                    return;
                }
                if (user == null)
                {
                    if(state == null)
                    {
                        await _menuServiceHandler.ClickStartCommand(message.Chat.Id, user, cancellationToken);
                        return;
                    }
                    forward = state switch
                    {
                        StateList.register_language => _registerationService.ReceivedLanguageAsync(message, cancellationToken),
                        StateList.register_fullname => _registerationService.ReceivedFullNameAsync(message, cancellationToken),
                        StateList.register_contact => _registerationService.ReceivedUserContactAsync(message, cancellationToken),
                        _ => _menuServiceHandler.ClickStartCommand(message.Chat.Id, user, cancellationToken)
                    };
                    await forward;
                    return;
                }
                await _cacheDbService.SetObjectAsync(message.Chat.Id.ToString(), user);
            }

            

            forward = state switch
            {
                
                StateList.settings when message.Text == "change_fullname" => _settingsServiceHandler.ClickChangeFullNameButtonAsync(message, cancellationToken),
                StateList.settings when message.Text == "change_phone" => _settingsServiceHandler.ClickChangePhoneNumberAsync(message, cancellationToken),
                StateList.settings when message.Text == "change_language" => _settingsServiceHandler.ClickChangeLanguageAsync(message, cancellationToken),
                StateList.settings when message.Text == "change_skill" => _settingsServiceHandler.ClickChangeSkillsButtonAsync(message, cancellationToken),
                StateList.settings when message.Text == "back" => _menuServiceHandler.RedirectToMainMenuAsync(message.Chat.Id, cancellationToken),
                StateList.settings_change_language => _settingsServiceHandler.ReceivedNewLanguageAsync(message, cancellationToken),
                StateList.settings_change_phone => _settingsServiceHandler.ReceivedNewPhoneNumberAsync(message, cancellationToken),
                StateList.settings_change_fullname => _settingsServiceHandler.ReceivedNewFullNameAsync(message, cancellationToken),
                StateList.settings_change_skills when message.Text == "back" => _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken),
                StateList.settings_change_skills => _settingsServiceHandler.ReceivedSkillForSettingAsync(message, cancellationToken),
                StateList.feedback => _feedbackService.ReceivedFeedbackAsync(message, cancellationToken),
                _ when message.Text == "Settings" => _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken),
                _ when message.Text == "Contact" => _menuServiceHandler.RedirectToContactMenuAsync(message, cancellationToken),
                _ when message.Text == "Feedback" => _menuServiceHandler.RedirectToFeedbackMenuAsync(message, cancellationToken),
                _ => _menuServiceHandler.RedirectToMainMenuAsync(message.Chat.Id, cancellationToken)
            };
            await forward;
            return;
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation(message: "Unknown message type is {}", update.Type);
            return Task.CompletedTask;
        }
    }
}
