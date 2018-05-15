using System.Collections.Generic;
using SmartBoy.Managers;
using SmartBoy.Services;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace SmartBoy.Commands
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

        public override void Process(BotContext context, MessageEventArgs args)
        {
            var message = args.Message;
            var chat = message.Chat;
            var user = message.From;
            storageManager.AddUsersToChat(chat.Id, new List<User> { user });

            context.BotClient.SendTextMessageAsync(chat.Id, "Register Succeeded", false, false, message.MessageId);
        }
    }
}