using LeosSmartBoy.Services;

namespace LeosSmartBoy.Commands
{
    public abstract class CommandImpl : ICommandInterface
    {
        protected CommandImpl(string command)
        {
            BotService.RegisterService(command, Process);
        }
        public abstract void Process(string data);
    }
}