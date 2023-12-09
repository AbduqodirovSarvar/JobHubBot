using Telegram.Bot.Types;

namespace JobHubBot.Interfaces.IHandlerServiceInterfaces
{
    public interface ISettingsServiceHandler
    {
        Task ClickChangeFullNameButtonAsync(Message message, CancellationToken cancellationToken);
        Task ReceivedNewFullNameAsync(Message message, CancellationToken cancellationToken);
        Task ClickChangePhoneNumberAsync(Message message, CancellationToken cancellationToken);
        Task ReceivedNewPhoneNumberAsync(Message message, CancellationToken cancellationToken);
        Task ClickChangeLanguageAsync(Message message, CancellationToken cancellationToken);
        Task ReceivedNewLanguageAsync(Message message, CancellationToken cancellationToken);
        Task ClickChangeSkillsButtonAsync(Message message, CancellationToken cancellationToken);
        Task ReceivedSkillForSettingAsync(Message message, CancellationToken cancellationToken);
        Task ShowAllSkillsAsync(Message message, CancellationToken cancellationToken);
    }
}
