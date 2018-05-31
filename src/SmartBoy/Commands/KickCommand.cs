using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartBoy.Callbacks;
using SmartBoy.Helpers;
using SmartBoy.Managers;
using SmartBoy.Services;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartBoy.Commands
{
    public class KickCommand : CommandImpl
    {
        private readonly IStorageManager storageManager;
        private static string command = "/kick";

        private KickCommand(IStorageManager storageManager) : base(command)
        {
            this.storageManager = storageManager;
        }

        public static void BuildCommand(IStorageManager storageManager)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new KickCommand(storageManager);
        }

        public override async void Process(BotContext context, MessageEventArgs args)
        {
            var message = args.Message;
            var chat = message.Chat;
            if (chat.Type != ChatType.Group && chat.Type != ChatType.Supergroup) return;
            if (message.Entities.Length != 2) return;
            var entityType = message.Entities[1].Type;
            User user = null;
            if (entityType == MessageEntityType.TextMention)
            {
                user = message.Entities[1].User;
            }
            else if (entityType == MessageEntityType.Mention)
            {
                user = storageManager.FindUser(message.EntityValues.ElementAt(1));
            }
            if (user != null && user.IsBot == false)
            {
                try
                {
                    await context.BotClient.KickChatMemberAsync(chat.Id, user.Id);
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException)
                {

                }
            }
        }

        public override void Callback(BotContext context, CallbackQueryEventArgs args) { }
    }
}