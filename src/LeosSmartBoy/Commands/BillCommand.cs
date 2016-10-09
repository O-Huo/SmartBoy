using System.Linq;
using System.Net.Http;
using LeosSmartBoy.Helpers;
using LeosSmartBoy.Managers;
using LeosSmartBoy.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LeosSmartBoy.Commands
{
    public class BillCommand : CommandImpl
    {
        private readonly IStorageManager storageManager;

        private BillCommand(IStorageManager storageManager) : base("/bill")
        {
            this.storageManager = storageManager;
        }

        public static void BuildCommand(IStorageManager storageManager)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new BillCommand(storageManager);
        }

        public override void Process(BotContext context)
        {
            var message = context.Message;
            var chat = message.Chat;
            if (chat.Type != ChatType.Group && chat.Type != ChatType.Supergroup) return;

            var client = context.BotClient;
            //var userList = storageManager.GetChatUsers(chat);

            client.SendTextMessageAsync(chat.Id, "Set Amount", false, false, 0,
                KeyboardMarkupHelpers.CreateDigitInlineKeyboardMarkup("id"));
            //client.SendTextMessageAsync(chat.Id, "Select User", false, false, 0,
            //    new InlineKeyboardMarkup(userList.Select(user => new InlineKeyboardButton
            //    {
            //        Text = user.FirstName + " " + user.LastName, CallbackData = user.Id.ToString()
            //    }).ToArray()));
        }

    }
}