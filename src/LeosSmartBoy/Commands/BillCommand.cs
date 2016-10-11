using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using LeosSmartBoy.Helpers;
using LeosSmartBoy.Managers;
using LeosSmartBoy.Models;
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

            var result = message.Text?.Split(new[] {' '}, 2);
            var amountNumber = -1f;
            if (result?.Length == 2)
            {
                var amount = result[1];
                try
                {
                    amountNumber = float.Parse(amount);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            var bill = new Bill
            {
                Id = message.MessageId,
                ChatId = chat.Id,
                CreatedBy = message.From.Id,
                CurrentStatus = Bill.Status.SetAmountIntegeral,
                SharedWith = new List<int>()
            };

            if (amountNumber > 0)
            {
                bill.Amount = amountNumber;
                bill.CurrentStatus = Bill.Status.SetSharedWith;
            }

            var client = context.BotClient;
            //var userList = storageManager.GetChatUsers(chat);

            client.SendTextMessageAsync(chat.Id, "Set Amount", false, false, 0,
                KeyboardMarkupHelpers.CreateDigitInlineKeyboardMarkup("123123"));
            //client.SendTextMessageAsync(chat.Id, "Select User", false, false, 0,
            //    new InlineKeyboardMarkup(userList.Select(user => new InlineKeyboardButton
            //    {
            //        Text = user.FirstName + " " + user.LastName, CallbackData = user.Id.ToString()
            //    }).ToArray()));
        }

    }
}