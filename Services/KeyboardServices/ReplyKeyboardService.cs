using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace JobHubBot.Services.KeyboardServices
{
    public class ReplyKeyboardService
    {
        public static ReplyKeyboardMarkup MakeReplyKeyboard(List<string> names, int? rows = 2)
        {
            List<KeyboardButton[]> buttonRows = new();
            List<KeyboardButton> buttons = new();
            foreach (var name in names)
            {
                if (buttons.Count == rows)
                {
                    buttonRows.Add(buttons.ToArray());
                    buttons.Clear();
                }
                buttons.Add(new KeyboardButton(name.ToString()));
            }
            if (buttons.Count > 0)
            {
                buttonRows.Add(buttons.ToArray());
            }
            return new ReplyKeyboardMarkup(buttonRows.ToArray()) { ResizeKeyboard = true };
        }
    }
}