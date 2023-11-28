namespace JobHubBot.Models.Telegram
{
    public class BotConfiguration
    {
        public static readonly string Configuration = "BotConfiguration";
        public static readonly string RouteSection = "BotConfiguration:Route";

        public string BotToken { get; init; } = default!;
        public string HostAddress { get; init; } = default!;
        public string Route { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
    }
}
