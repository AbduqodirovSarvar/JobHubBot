using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Interfaces.IHandlerServiceInterfaces;
using JobHubBot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using JobHubBot.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using JobHubBot.Services.KeyboardServices;
using JobHubBot.Db.Enums;
using User = JobHubBot.Db.Entities.User;
using JobHubBot.Db.Entities;
using System.Globalization;

namespace JobHubBot.Services.HandleServices
{
    public class SettingsServiceHandler : ISettingsServiceHandler
    {
        private readonly IAppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IMenuServiceHandler _menuServiceHandler;
        private readonly IStateManagementService _stateManagementService;
        private readonly ICacheDbService _cacheDbService;
        public SettingsServiceHandler(
            IAppDbContext dbContext,
            ITelegramBotClient botClient, 
            IStringLocalizer<Messages> localizer,
            IMenuServiceHandler menuServiceHandler,
            IStateManagementService stateManagementService,
            ICacheDbService cacheDbService
            )
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _cacheDbService = cacheDbService;
            _localizer = localizer;
            _stateManagementService = stateManagementService;
            _menuServiceHandler = menuServiceHandler;
        }

        public async Task ClickChangeLanguageAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localizer["choose_language"],
                replyMarkup: KeyboardsMaster.CreateReplyKeyboardMarkup(new List<string>() { Language.uz.ToString(), Language.en.ToString(), Language.ru.ToString()}),
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, StateList.settings_change_language);
            return;
        }

        public async Task ReceivedNewLanguageAsync(Message message, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);
                return;
            }

            Language language = message.Text switch
            {
                "en" => Language.en,
                "uz" => Language.uz,
                _ => Language.ru,
            };
            user.Language = language;

            var languageCode = user.Language switch
            {
                Language.uz => "uz-UZ",
                Language.en => "en-US",
                Language.ru => "ru-RU",
                _ => "uz-UZ"
            };

            Thread.CurrentThread.CurrentCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageCode);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);
            await _cacheDbService.SetObjectAsync(message.Chat.Id.ToString(), user);
            return;
        }

        public async Task ClickChangeFullNameButtonAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localizer["enter_fullname"],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, StateList.settings_change_fullname);
            return;
        }

        public async Task ReceivedNewFullNameAsync(Message message, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);
                return;
            }
            user.FullName = message.Text!;
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);

            await _cacheDbService.SetObjectAsync(message.Chat.Id.ToString(), user);
            return;
        }

        public async Task ClickChangePhoneNumberAsync(Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localizer["send-contact"],
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, StateList.settings_change_phone);
            return;
        }

        public async Task ReceivedNewPhoneNumberAsync(Message message, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);
                return;
            }
            user.Phone = message.Text!;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);
            return;
        }

        public async Task ClickChangeSkillsButtonAsync(Message message, CancellationToken cancellationToken)
        {
            await ShowAllSkillsAsync(message, cancellationToken);

            await _stateManagementService.SetUserState(message.Chat.Id, StateList.settings_change_skills);
            return;
        }

        public async Task ReceivedSkillForSettingAsync(Message message, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken);
            if (user == null)
            {
                await _menuServiceHandler.RedirectToSettingsMenuAsync(message, cancellationToken);
                return;
            }

            var skill = user.Skills.FirstOrDefault(x => x.Name == message.Text);
            if (skill == null)
            {
                var theskill = await _dbContext.Skills.FirstOrDefaultAsync(x => x.Name.ToLower() == message.Text!.ToLower(), cancellationToken);
                if(theskill == null)
                {
                    theskill = new Skill()
                    {
                        Name = message.Text!
                    };
                    await _dbContext.Skills.AddAsync(theskill, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                
                user.Skills.Add(theskill);
            }
            else
            {
                user.Skills.Remove(skill);
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            await ShowAllSkillsAsync(message, cancellationToken);
            return;
        }

        public async Task ShowAllSkillsAsync(Message message, CancellationToken cancellationToken)
        {
            var skills = (await _dbContext.Users.Include(x => x.Skills).FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, cancellationToken))?.Skills;
            var keyboardNames = skills?.Select(x => x.Name).ToList() ?? new List<string>();
            keyboardNames.Add(_localizer["back_button"]);

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: _localizer["skill_setting"],
                replyMarkup: KeyboardsMaster.CreateReplyKeyboardMarkup(keyboardNames),
                cancellationToken: cancellationToken);

            return;
        }
    }
}
