using Telegram.Bot;

namespace LeosSmartBoy.Services
{
    public class BotContext
    {
        public ITelegramBotClient BotClient;
        public GithubBotService GithubBot;
    }
}