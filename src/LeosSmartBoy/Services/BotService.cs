using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeosSmartBoy.Callbacks;
using LeosSmartBoy.Commands;
using LeosSmartBoy.Managers;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Services
{
    public class BotService
    {
        public delegate void MessageEventHandler(BotContext context, MessageEventArgs args);
        public delegate void CallbackQueryEventHandler(BotContext context, CallbackQueryEventArgs args);

        private static readonly IDictionary<string, MessageEventHandler> MessageEventHandlers = new Dictionary<string, MessageEventHandler>();
        private static readonly IDictionary<string, CallbackQueryEventHandler> CallbackQueryEventHandlers = new Dictionary<string, CallbackQueryEventHandler>();
        private readonly IStorageManager storageManager = new MemoryStorageManager();

        public ITelegramBotClient BotClient { get; }

        public GithubBotService GithubBot;

        public BotService(string clientSecret, string githubID, string githubSecret)
        {
            BotClient = new TelegramBotClient(clientSecret);
            GithubBot = new GithubBotService(githubID, githubSecret);
        }

        public async Task Run()
        {
            BotClient.OnMessage += BotMessageReceived;
            BotClient.OnCallbackQuery += BotCallbackQueryReceived;
            BotClient.OnInlineQuery += BotInlineQueryReceived;

            ReigsterCommands();

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
            var message = args.Message?.Text;
            var result = message?.Split();
            var key = result?.Length > 0 ? result[0] : null;
            if (key == null) return;


            // Console.WriteLine(message);

            // var kb = new ReplyKeyboardMarkup();
            // kb.Keyboard=
            //     new KeyboardButton[][]
            //     {
            //         new KeyboardButton[]
            //         {
            //             new KeyboardButton("花了一些钱"+"\u261d"),
            //             new KeyboardButton("还了一些钱"+"\u2615")
            //         },
            //         new KeyboardButton[]
            //         {
            //             new KeyboardButton("本月日志"+"\u3299")
            //         } 
            //     };

            // BotClient.SendTextMessageAsync(args.Message.Chat.Id, "hhh", false, false, 0, kb);


            if (MessageEventHandlers.ContainsKey(key))
            {
                MessageEventHandlers[key](new BotContext
                {
                    BotClient = BotClient,
                    GithubBot = GithubBot
                }, args);
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
                    GithubBot = GithubBot
                }, args);
            }
        }

        private void BotInlineQueryReceived(object obj, InlineQueryEventArgs args)
        {
            Console.WriteLine(args);
        }


        private void ReigsterCommands()
        {
            BillCommand.BuildCommand(storageManager);
            RegisterCommand.BuildCommand(storageManager);
            GithubBot.AddStorageManager(storageManager);
        }
    }
}
