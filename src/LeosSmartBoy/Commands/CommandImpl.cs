using LeosSmartBoy.Services;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Commands
{
    public abstract class CommandImpl : ICommandInterface
    {
        protected CommandImpl(string command)
        {
            BotService.RegisterService(command, Process);
        }
        public abstract void Process(BotContext context);
    }
}