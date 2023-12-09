using JobHubBot.Services.Enums;

namespace JobHubBot.Interfaces.IDbInterfaces
{
    public interface IStateManagementService
    {
        Task SetUserState(long chatId, StateList state);

        Task DeleteState(long Id);

        StateList? GetUserState(long Id);
    }
}
