using Telegram.Bot;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Services
{
    public class BotContext
    {
        public ITelegramBotClient BotClient;
        public Message Message;
    }
}