using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeosSmartBoy.Commands;
using LeosSmartBoy.Managers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Services
{
    public class BotService
    {
        public delegate void MessageEventHandler(BotContext context);

        private static readonly IDictionary<string, MessageEventHandler> Handlers = new Dictionary<string, MessageEventHandler>();
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
            //BotClient.OnMessage += BotMessageReceived;
            //BotClient.OnCallbackQuery += BotCallbackQueryReceived;
            //BotClient.OnInlineQuery += BotInlineQueryReceived;

            //ReigsterCommands();

            //BotClient.StartReceiving();
            var task = new Task(() =>
            {
                while (true) {}
                // ReSharper disable once FunctionNeverReturns
            });
            await task;
        }

        public static void RegisterService(string command, MessageEventHandler handler)
        {
            if (Handlers.ContainsKey(command))
            {
                Handlers[command] += handler;
            }
            else
            {
                Handlers.Add(command, handler);
            }
        }

        private void BotMessageReceived(object obj, MessageEventArgs args)
        {
            var message = args.Message?.Text;
            var result = message?.Split();
            var key = result?.Length > 0 ? result[0] : null;
            if (key == null) return;

            Console.WriteLine(message);

            var kb = new ReplyKeyboardMarkup();
            kb.Keyboard=
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("花了一些钱"+"\u261d"),
                        new KeyboardButton("还了一些钱"+"\u2615")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("本月日志"+"\u3299")
                    } 
                };

            BotClient.SendTextMessageAsync(args.Message.Chat.Id, "hhh", false, false, 0, kb);

            if (Handlers.ContainsKey(key))
            {
                Handlers[key](new BotContext
                {
                    BotClient = BotClient,
                    Message = args.Message
                });
                Console.WriteLine("handlers contain key");
            }
            
        }

        private void BotCallbackQueryReceived(object obj, CallbackQueryEventArgs args)
        {
            Console.WriteLine(args);
        }

        private void BotInlineQueryReceived(object obj, InlineQueryEventArgs args)
        {
            Console.WriteLine(args);
        }


        private void ReigsterCommands()
        {
            BillCommand.BuildCommand(storageManager);
            RegisterCommand.BuildCommand(storageManager);
        }
    }
}
