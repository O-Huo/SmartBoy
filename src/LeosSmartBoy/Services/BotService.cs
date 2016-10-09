using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeosSmartBoy.Commands;
using LeosSmartBoy.Managers;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace LeosSmartBoy.Services
{
    public class BotService
    {
        public delegate void MessageEventHandler(BotContext context);

        private static readonly IDictionary<string, MessageEventHandler> Handlers = new Dictionary<string, MessageEventHandler>();
        private readonly IStorageManager storageManager = new MemoryStorageManager();

        public ITelegramBotClient BotClient { get; }

        public BotService(string clientSecrete)
        {
            BotClient = new TelegramBotClient(clientSecrete);
        }

        public async Task Run()
        {
            BotClient.OnMessage += BotMessageReceived;
            BotClient.OnUpdate += BotUpdatesReceived;
            BotClient.OnCallbackQuery += BotCallbackQueryReceived;
            BotClient.OnInlineQuery += BotInlineQueryReceived;

            ReigsterCommands();

            BotClient.StartReceiving();
            var task = new Task(() =>
            {
                while (true) {}
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

            if (Handlers.ContainsKey(key))
            {
                Handlers[key](new BotContext
                {
                    BotClient = BotClient,
                    Message = args.Message
                });
            }
        }

        private void BotUpdatesReceived(object obj, UpdateEventArgs args)
        {
            Console.WriteLine(args);
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
            new BillCommand(storageManager);
        }
    }
}