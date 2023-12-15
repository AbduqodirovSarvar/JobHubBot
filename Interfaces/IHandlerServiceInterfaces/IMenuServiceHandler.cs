using Telegram.Bot.Types;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Interfaces.IHandlerServiceInterfaces
{
    public interface IMenuServiceHandler
    {
        Task ClickStartCommand(long Id, User? user, CancellationToken cancellationToken);
        Task RedirectToMainMenuAsync(long Id, CancellationToken cancellationToken);
        Task RedirectToSettingsMenuAsync(Message message, CancellationToken cancellationToken);
        Task RedirectToFeedbackMenuAsync(Message message, CancellationToken cancellationToken);
        Task RedirectToSkillsMenuAsync(Message message, CancellationToken cancellationToken);
        Task RedirectToContactMenuAsync(Message message, CancellationToken cancellationToken);
    }
}
