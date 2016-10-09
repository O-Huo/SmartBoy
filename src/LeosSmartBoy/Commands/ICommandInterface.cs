using LeosSmartBoy.Services;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Commands
{
    public interface ICommandInterface
    {
        void Process(BotContext context);
    }
}