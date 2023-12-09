namespace JobHubBot.Services.CacheServices
{
    public static class MenuNames
    {
        internal static readonly Dictionary<string, Array> Menus = new()
        {
            {
                "main", new string[,]
                {
                    { "Ishlarni ko'rish", "Ko'nikmalarim", "Fikr-Mulohoza qoldirish", "Sozlamalar", "Aloqa" },
                    { "Viewing jobs", "My skills", "Leaving feedback", "Settings", "Contact" },
                    {"Просмотр вакансий", "Мои навыки", "Оставление отзыва", "Настройки", "Контакт" }
                }
            },
            {
                "back", new string[] {"Orqaga", "Back", "Назад" }
            },
            {
                "next", new string[]{"Keyingi", "Next", "Следующий" }
            },
            {
                "former", new string[]{"Avvalgisi", "Former", "Бывший" }
            },
            {
                "settings", new string[,]
                {
                    {"Ism-Familyani o'zgaritish", "Telefon raqamni o'zgartirish", "Tilni o'zgaritish", "Orqaga" },
                    {"Changing fullname", "Changing phone number", "Changing Language", "Back" },
                    {"Изменение полного имени", "Изменение номера телефона", "Изменение языка", "Назад" }
                }
            },
            {
                "skills", new string[,]
                {
                    {"Ko'nikmalarimni ko'rish", "Yangi ko'nikma qo'shish", "Ko'nikmani o'chirish", "Orqaga" },
                    {"Viewing my skills", "Adding new skills", "Removing skills", "Back" },
                    {"Просмотр моих навыков", "Добавить новые навыки", "Удаление навыков", "Назад" }
                }
            },
        };

        internal static readonly Dictionary<string, Array> Texts = new()
        {
            {"hello", new string[] {"Assalomu aleykum, Botimizga xush kelibsiz!", "Hello, welcome to our bot!", "Здравствуйте, добро пожаловать в наш бот!" } },
            {"choose_language", new string[] {"Tilni tanlang: ", "Choose a language:", "Выберите язык:" } },
            {"fullname", new string[] {"To'liq ismingizni kiriting:", "Enter your fullname", "Введите свое полное имя" } },
            {"phone", new string[] {"Kontagingizni jo'nating", "Send your contact", "Отправьте свой контакт" } },
            {"choose_command", new string[] {"Kerakli bo'limni tanlang!", "Select the desired section", "Выберите нужный раздел" } }

        };
    }
}
