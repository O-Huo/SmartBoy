using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace LeosSmartBoy.Services
{
    public class BotService
    {
        public delegate void MessageEventHandler(string text);
        private ITelegramBotClient botClient;
        private static readonly IDictionary<string, MessageEventHandler> Handlers = new Dictionary<string, MessageEventHandler>();

        public BotService(string clientSecrete)
        {
            botClient = new TelegramBotClient(clientSecrete);
        }

        public async Task Run()
        {
            var result = botClient.GetMeAsync().Result;

            botClient.OnMessage += BotMessageReceived;
            botClient.OnUpdate += BotUpdatesReceived;
            botClient.OnCallbackQuery += BotCallbackQueryReceived;
            botClient.OnInlineQuery += BotInlineQueryReceived;

            botClient.StartReceiving();
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
            var message = args.Message.Text;
            var result = message.Split(new[] {' '}, 2);
            var key = result.Length > 0 ? result[0] : null;
            var data = result.Length > 1 ? result[1] : null;
            if (key == null) return;

            if (Handlers.ContainsKey(key))
            {
                Handlers[key](data);
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
    }
}