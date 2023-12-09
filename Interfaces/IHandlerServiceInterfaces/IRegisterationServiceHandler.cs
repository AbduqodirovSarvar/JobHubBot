using Telegram.Bot.Types;

namespace JobHubBot.Interfaces.IHandlerServiceInterfaces
{
    public interface IRegisterationServiceHandler
    {
        Task ReceivedLanguageAsync(Message message, CancellationToken cancellationToken);
        Task ReceivedFullNameAsync(Message message, CancellationToken cancellationToken);
        Task ReceivedUserContactAsync(Message message, CancellationToken cancellationToken);
    }
}
