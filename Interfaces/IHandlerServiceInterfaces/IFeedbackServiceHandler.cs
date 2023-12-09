using Telegram.Bot.Types;

namespace JobHubBot.Interfaces.IHandlerServiceInterfaces
{
    public interface IFeedbackServiceHandler
    {
        Task ReceivedFeedbackAsync(Message message, CancellationToken cancellationToken);
    }
}
