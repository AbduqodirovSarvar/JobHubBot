using Telegram.Bot.Types;

namespace JobHubBot.Interfaces.IHandlerServiceInterfaces
{
    public interface IChannelMessageServiceHandler
    {
        Task ForwardMessageToAllUsersAsync(Message message, CancellationToken cancellationToken);
        Task ForwardJobMessageForUserAsync(Message message, CancellationToken cancellationToken);
    }
}
