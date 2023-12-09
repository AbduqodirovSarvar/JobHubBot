using Telegram.Bot.Types.ReplyMarkups;

namespace JobHubBot.Services.KeyboardServices
{
    public static class KeyboardsMaster
    {
        public static InlineKeyboardMarkup CreateInlineKeyboardMarkup(Dictionary<string, string> options, int? rows = 2)
        {
            List<InlineKeyboardButton[]> buttonRows = new();
            List<InlineKeyboardButton> buttons = new();
            foreach (var item in options)
            {
                if (buttons.Count == rows)
                {
                    buttonRows.Add(buttons.ToArray());
                    buttons.Clear();
                }
                buttons.Add(InlineKeyboardButton.WithCallbackData(item.Key.ToString(), item.Value.ToLower()));
            }
            if (buttons.Count > 0)
            {
                buttonRows.Add(buttons.ToArray());
            }
            return new InlineKeyboardMarkup(buttonRows.ToArray());
        }

        public static ReplyKeyboardMarkup CreateReplyKeyboardMarkup(List<string> names, int? rows = 2)
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

        public static ReplyKeyboardMarkup CreateContactRequestKeyboardMarkup(string keyboardName)
        {
            var contactRequestKeyboardMarkup = new ReplyKeyboardMarkup(
                          new KeyboardButton(keyboardName) { RequestContact = true })
            { ResizeKeyboard = true };

            return contactRequestKeyboardMarkup;
        }

        public static ReplyKeyboardMarkup CreateLocationRequestKeyboardMarkup(string keyboardName)
        {
            var locationRequestKeyboardMarkup = new ReplyKeyboardMarkup(
                           new KeyboardButton(keyboardName) { RequestLocation = true })
            { ResizeKeyboard = true };

            return locationRequestKeyboardMarkup;
        }
    }
}
