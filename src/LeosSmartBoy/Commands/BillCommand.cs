using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using LeosSmartBoy.Helpers;
using LeosSmartBoy.Managers;
using LeosSmartBoy.Models;
using LeosSmartBoy.Services;
using Telegram.Bot;
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
            var amountNumber = .0f;
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
                SharedWith = new HashSet<int> { message.From.Id },
                Amount = amountNumber
            };

            if (amountNumber > 0)
            {
                bill.CurrentStatus = Bill.Status.SetSharedWith;
            }

            var client = context.BotClient;
            var users = storageManager.GetChatUsers(chat);

            if (bill.CurrentStatus == Bill.Status.SetAmountIntegeral)
            {
                client.SendTextMessageAsync(chat.Id, "Amount: " + bill.Amount, false, false, 0,
                    KeyboardMarkupHelpers.CreateDigitInlineKeyboardMarkup());
            }
            else
            {
                client.SendTextMessageAsync(chat.Id, "Amount: " + bill.Amount + "\nSelected Users:", false, false, 0,
                    KeyboardMarkupHelpers.CreateUserSelectionKeyboardMarkup(users, bill.SharedWith));
            }
        }

        private void GenerateResponse(Bill bill, ITelegramBotClient botClient)
        {
            switch (bill.CurrentStatus)
            {
                case Bill.Status.SetAmountIntegeral:
                    break;
                case Bill.Status.SetAmountFractional:
                    break;
                case Bill.Status.SetSharedWith:
                    break;
                case Bill.Status.Sealed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}