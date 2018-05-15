using SmartBoy.Services;
using Telegram.Bot.Args;

namespace SmartBoy.Commands
{
    public abstract class CommandImpl : ICommandInterface
    {
        protected CommandImpl(string command)
        {
            BotService.RegisterMessageEventService(command, Process);
            BotService.RegisterCallbackQueryEventService(command, Callback);
        }
        public abstract void Process(BotContext context, MessageEventArgs args);

        public virtual void Callback(BotContext context, CallbackQueryEventArgs args) { }
    }
}