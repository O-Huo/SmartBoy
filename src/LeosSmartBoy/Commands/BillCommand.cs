using System;
using System.Collections.Generic;
using System.Linq;
using LeosSmartBoy.Helpers;
using LeosSmartBoy.Managers;
using LeosSmartBoy.Models;
using LeosSmartBoy.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using LeosSmartBoy.Callbacks;
using Newtonsoft.Json;
using Telegram.Bot.Args;

namespace LeosSmartBoy.Commands
{
    public class BillCommand : CommandImpl
    {
        private readonly IStorageManager storageManager;
        private static string command = "/bill";

        private BillCommand(IStorageManager storageManager) : base(command)
        {
            this.storageManager = storageManager;
        }

        public static void BuildCommand(IStorageManager storageManager)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new BillCommand(storageManager);
        }

        public override async void Process(BotContext context, MessageEventArgs args)
        {
            var message = args.Message;
            var chat = message.Chat;
//if (chat.Type != ChatType.Group && chat.Type != ChatType.Supergroup) return;
            Console.WriteLine(message);

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
                ChatId = chat.Id,
                CreatedBy = message.From.Id,
                CurrentStatus = Bill.Status.SetAmountIntegeral,
                SharedWith = new HashSet<int> {message.From.Id},
            };
            bill.AmountString.Append(amountNumber);

            if (amountNumber > 0)
            {
                bill.CurrentStatus = Bill.Status.SetSharedWith;
            }

            var client = context.BotClient;
            var response = await GenerateResponse(bill, client);
            bill.MessageId = response.MessageId;
            storageManager.SaveBill(bill);
        }

        public override async void Callback(BotContext context, CallbackQueryEventArgs args)
        {
            var message = args.CallbackQuery.Message;
            var callbcakData = JsonConvert.DeserializeObject<InlineKeyboardCallback>(args.CallbackQuery.Data);

            var bill = storageManager.GetBillWIthMessageId(message.MessageId);
            if (bill?.CreatedBy != args.CallbackQuery.From.Id) return;

            Button button;
            var input = (string) callbcakData.Data;

            if (Enum.IsDefined(typeof(Button), input))
            {
                Enum.TryParse(input, false, out button);
                UpdateBill(bill, button, context);
            }
            else
            {
                switch (bill.CurrentStatus)
                {
                    case Bill.Status.SetAmountIntegeral:
                    case Bill.Status.SetAmountFractional:
                        HandleDigitInput(bill, input);
                        break;
                    case Bill.Status.SetSharedWith:
                        HandleUserSelection(bill, input);
                        break;
                    case Bill.Status.Sealed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            await GenerateResponse(bill, context.BotClient, true);
        }

        private void HandleUserSelection(Bill bill, string userId)
        {
            var id = int.Parse(userId);
            if (bill.SharedWith.Contains(id))
            {
                bill.SharedWith.Remove(id);
            }
            else
            {
                bill.SharedWith.Add(id);
            }
        }

        private void HandleDigitInput(Bill bill, string input)
        {
            var digit = Convert.ToInt32(input);
            bill.AmountString.Append(digit);
            while (bill.AmountString.Length > 1 && bill.AmountString[0] == '0' && bill.AmountString[1] != '.')
            {
                bill.AmountString.Remove(0, 1);
            }
        }

        private void UpdateBill(Bill bill, Button button, BotContext context)
        {
            var githubBot = context.GithubBot;
            switch (button)
            {
                case Button.Dot:
                    if (bill.CurrentStatus == Bill.Status.SetAmountIntegeral)
                    {
                        if (bill.AmountString.Length == 0) bill.AmountString.Append('0');
                        bill.AmountString.Append('.');
                        bill.CurrentStatus = Bill.Status.SetAmountFractional;
                    }
                    break;
                case Button.Back:
                    var amout = bill.AmountString;
                    if (amout.Length > 0)
                    {
                        if (amout[amout.Length - 1] == '.')
                        {
                            bill.CurrentStatus = Bill.Status.SetAmountIntegeral;
                        }
                        amout.Remove(amout.Length - 1, 1);
                    }
                    break;
                case Button.Ok:
                    switch (bill.CurrentStatus)
                    {
                        case Bill.Status.SetAmountIntegeral:
                        case Bill.Status.SetAmountFractional:
                            bill.CurrentStatus = Bill.Status.SetSharedWith;
                            break;
                        case Bill.Status.SetSharedWith:
                            bill.CurrentStatus = Bill.Status.Sealed;
                            githubBot.WriteBill(bill);
                            break;
                        case Bill.Status.Sealed:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }

        private delegate Task<Message> MessageResponseDelegate(string message, IReplyMarkup repleyMarkup);

        private async Task<Message> GenerateResponse(Bill bill, ITelegramBotClient client,
            bool modifyExistingMessage = false)
        {
            MessageResponseDelegate responseDelegate = async (message, replyMarkup) =>
            {
                if (!modifyExistingMessage)
                {
                    return await client.SendTextMessageAsync(bill.ChatId, message, false, false, 0, replyMarkup);
                }
                try
                {
                    return
                        await
                            client.EditMessageTextAsync(bill.ChatId, bill.MessageId, message, ParseMode.Default, false,
                                replyMarkup);

                }
                catch (Exception)
                {
                    return null;
                }
            };
            var users = storageManager.GetChatUsers(bill.ChatId);
            switch (bill.CurrentStatus)
            {
                case Bill.Status.SetAmountIntegeral:
                case Bill.Status.SetAmountFractional:
                    return await responseDelegate("Amount: " + bill.AmountString, KeyboardMarkupHelpers.CreateDigitInlineKeyboardMarkup(command));
                case Bill.Status.SetSharedWith:
                    return await responseDelegate("Amount: " + bill.Amount+ "\nSelected Users:",
                        KeyboardMarkupHelpers.CreateUserSelectionKeyboardMarkup(command, users, bill.SharedWith));
                case Bill.Status.Sealed:
                    return await responseDelegate("Amount: " + bill.AmountString + "\nSelected Users: " + string.Join(", ", users.Where(u => bill.SharedWith.Contains(u.Id)).Select(u => u.LastName + ' ' + u.FirstName)) + "\nTestIssue: \n https://github.com/RoommateX/test/issues/16", null);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
