using System.Collections.Generic;
using LeosSmartBoy.Managers;
using LeosSmartBoy.Services;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Commands
{
    public class RegisterCommand : CommandImpl
    {
        private readonly IStorageManager storageManager;
        private RegisterCommand(IStorageManager storageManager) : base("/register")
        {
            this.storageManager = storageManager;
        }

        public static void BuildCommand(IStorageManager storageManager)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new RegisterCommand(storageManager);
        }

        public override void Process(BotContext context)
        {
            var message = context.Message;
            var chat = message.Chat;
            var user = message.From;
            storageManager.AddUsersToChat(chat, new List<User> { user });

            context.BotClient.SendTextMessageAsync(chat.Id, "Register Succeeded", false, false, message.MessageId);
        }
    }
}