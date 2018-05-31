using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartBoy.Callbacks;
using SmartBoy.Commands;
using SmartBoy.Managers;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using System.Linq;

namespace SmartBoy.Services
{
    public class BotService
    {
        public delegate void MessageEventHandler(BotContext context, MessageEventArgs args);
        public delegate void CallbackQueryEventHandler(BotContext context, CallbackQueryEventArgs args);

        private static readonly IDictionary<string, MessageEventHandler> MessageEventHandlers = new Dictionary<string, MessageEventHandler>();
        private static readonly IDictionary<string, CallbackQueryEventHandler> CallbackQueryEventHandlers = new Dictionary<string, CallbackQueryEventHandler>();
        private readonly IStorageManager storageManager = new MemoryStorageManager();

        public ITelegramBotClient BotClient { get; }

        public BotService(string clientSecrete)
        {
            BotClient = new TelegramBotClient(clientSecrete);
        }

        public async Task Run()
        {
            BotClient.OnMessage += BotMessageReceived;
            BotClient.OnCallbackQuery += BotCallbackQueryReceived;
            BotClient.OnInlineQuery += BotInlineQueryReceived;

            RegisterCommands();

            BotClient.StartReceiving();
            var task = new Task(() =>
            {
                while (true) {}
                // ReSharper disable once FunctionNeverReturns
            });
            await task;
        }

        public static void RegisterMessageEventService(string command, MessageEventHandler handler)
        {
            if (MessageEventHandlers.ContainsKey(command))
            {
                MessageEventHandlers[command] += handler;
            }
            else
            {
                MessageEventHandlers.Add(command, handler);
            }
        }

        public static void RegisterCallbackQueryEventService(string command, CallbackQueryEventHandler handler)
        {
            if (CallbackQueryEventHandlers.ContainsKey(command))
            {
                CallbackQueryEventHandlers[command] += handler;
            }
            else
            {
                CallbackQueryEventHandlers.Add(command, handler);
            }
        }

        private void BotMessageReceived(object obj, MessageEventArgs args)
        {
            CheckUser(args);

            var message = args.Message;
            var text = message?.Text;
            var result = text?.Split();
            var key = result?.Length > 0 ? result[0] : null;
            if (key == null) return;

            if (MessageEventHandlers.ContainsKey(key))
            {
                MessageEventHandlers[key](new BotContext
                {
                    BotClient = BotClient,
                }, args);
            }
        }

        private void CheckUser(MessageEventArgs args) {
            var message = args.Message;
            var chat = message?.Chat;
            var user = message?.From;
            if (user != null) {
                var result = storageManager.AddUsersToChat(chat.Id, new List<User> { user });
                if (result.Any()) {
                    var users = String.Join(",", result.Select(x => x.FirstName));
                    BotClient.SendTextMessageAsync(chat.Id, $"Hello {users}, welcome onboard!", false, false,
                        message.MessageId);
                }
            }
        }

        private void BotCallbackQueryReceived(object obj, CallbackQueryEventArgs args)
        {
            Console.WriteLine(args);
            var callback = JsonConvert.DeserializeObject<InlineKeyboardCallback>(args.CallbackQuery.Data);
            if (MessageEventHandlers.ContainsKey(callback?.Command))
            {
                CallbackQueryEventHandlers[callback?.Command](new BotContext
                {
                    BotClient = BotClient,
                }, args);
            }
        }

        private void BotInlineQueryReceived(object obj, InlineQueryEventArgs args)
        {
            Console.WriteLine(args);
        }


        private void RegisterCommands()
        {
            KickCommand.BuildCommand();
        }
    }
}