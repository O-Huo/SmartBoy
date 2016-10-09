using System.Collections.Generic;
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

        public BillCommand(IStorageManager storageManager) : base("/bill")
        {
            this.storageManager = storageManager;
        }

        public override void Process(BotContext context)
        {
            var message = context.Message;
            var chat = message.Chat;
            if (chat.Type != ChatType.Group && chat.Type != ChatType.Supergroup) return;

            var client = context.BotClient;
            var userList = storageManager.GetChatUsers(chat);
            var keyboardButtons = new List<InlineKeyboardButton>();
            foreach (var user in userList)
            {
                var button = new InlineKeyboardButton();
                button.Text = user.FirstName + " " + user.LastName;
                button.CallbackData = user.Id.ToString();
                keyboardButtons.Add(button);
            }

            client.SendTextMessageAsync(chat.Id, "Select User", false, false, 0,
                new InlineKeyboardMarkup(keyboardButtons.ToArray()));
        }
    }


}