using Telegram.Bot.Types.ReplyMarkups;

namespace JobHubBot.Services.KeyboardServices
{
    public class InlineKeyboardService
    {
        public static InlineKeyboardMarkup MakingInlineKeyboard(List<string> names, int? rows = 3)
        {
            List<InlineKeyboardButton[]> buttonRows = new();
            List<InlineKeyboardButton> buttons = new();
            foreach (var name in names)
            {
                if (buttons.Count == rows)
                {
                    buttonRows.Add(buttons.ToArray());
                    buttons.Clear();
                }
                buttons.Add(InlineKeyboardButton.WithCallbackData(name.ToString(), name.ToLower()));
            }
            if (buttons.Count > 0)
            {
                buttonRows.Add(buttons.ToArray());
            }
            return new InlineKeyboardMarkup(buttonRows.ToArray());
        }
    }
}