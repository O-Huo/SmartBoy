using SmartBoy.Services;
using Telegram.Bot.Args;

namespace SmartBoy.Commands
{
    public interface ICommandInterface
    {
        void Process(BotContext context, MessageEventArgs args);

        void Callback(BotContext context, CallbackQueryEventArgs args);
    }
}