using System;
using System.Collections.Generic;
using System.Linq;
using SmartBoy.Helpers;
using SmartBoy.Managers;
using SmartBoy.Models;
using SmartBoy.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using SmartBoy.Callbacks;
using Newtonsoft.Json;
using Telegram.Bot.Args;

namespace SmartBoy.Commands
{
    public class KickCommand : CommandImpl
    {
        private readonly IStorageManager storageManager;
        private static string command = "/kick";

        private KickCommand() : base(command)
        {
        }

        public static void BuildCommand()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new KickCommand();
        }

        public override async void Process(BotContext context, MessageEventArgs args)
        {
            var message = args.Message;
            var chat = message.Chat;
            if (chat.Type != ChatType.Group && chat.Type != ChatType.Supergroup) return;
            if (message.Entities.Count != 2) return;
            var entityType = message.Entities[1].Type;
            if (entityType != MessageEntityType.Mention && entityType != MessageEntityType.TextMention) return;
            var user = message.Entities[1].User;
            if (user != null) {
                await context.BotClient.KickChatMemberAsync(chat.Id, user.Id);
            }
        }

        public override async void Callback(BotContext context, CallbackQueryEventArgs args)
        {
        }
    }
}