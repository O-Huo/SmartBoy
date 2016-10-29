using System.Collections.Generic;
using LeosSmartBoy.Callbacks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LeosSmartBoy.Helpers
{
    public class KeyboardMarkupHelpers
    {
        private delegate string FillCallbackStringData(string data);
        private delegate string FillCallbackButtonData(Button data);
        public static InlineKeyboardMarkup CreateDigitInlineKeyboardMarkup(string command)
        {
            FillCallbackStringData fillCallbackStringData = data => new InlineKeyboardCallback(command, data).ToString();
            FillCallbackButtonData fillCallbackButtonData = data => new InlineKeyboardCallback(command, data).ToString();
            var buttons = new List<InlineKeyboardButton[]>();
            for (var row = 0; row < 3; row++)
            {
                var rowButtons = new List<InlineKeyboardButton>();
                for (var col = 0; col < 3; col++)
                {
                    var index = row*3 + col + 1;
                    rowButtons.Add(new InlineKeyboardButton(index.ToString(), fillCallbackStringData(index.ToString())));
                }
                buttons.Add(rowButtons.ToArray());
            }
            buttons.Add(new []
            {
                new InlineKeyboardButton("0", fillCallbackStringData("0")), 
                new InlineKeyboardButton(".", fillCallbackButtonData(Button.Dot)),
                new InlineKeyboardButton("\u2190", fillCallbackButtonData(Button.Back))
            });
            buttons.Add(new []
            {
                new InlineKeyboardButton("OK", fillCallbackButtonData(Button.Ok))
            });
            return new InlineKeyboardMarkup(buttons.ToArray());
        }

        private delegate string UserNameBuilder(User user);
        public static InlineKeyboardMarkup CreateUserSelectionKeyboardMarkup(string command, IList<User> users, ISet<int> selectedUser)
        {
            FillCallbackStringData fillCallbackStringData = data => new InlineKeyboardCallback(command, data).ToString();
            FillCallbackButtonData fillCallbackButtonData = data => new InlineKeyboardCallback(command, data).ToString();
            UserNameBuilder userNameBuilder = (user) =>
            {
                var userName = "";
                if (selectedUser.Contains(user.Id))
                {
                    userName += "\u2705";
                }
                userName += user.FirstName + " " + user.LastName;
                return userName;
            };
            var buttons = new List<InlineKeyboardButton[]>();
            var rowButton = new List<InlineKeyboardButton>();
            foreach (var user in users)
            {
                rowButton.Add(new InlineKeyboardButton(userNameBuilder(user), fillCallbackStringData(user.Id.ToString())));
                if (rowButton.Count != 3) continue;
                buttons.Add(rowButton.ToArray());
                rowButton = new List<InlineKeyboardButton>();
            }
            if (rowButton.Count > 0)
            {
                buttons.Add(rowButton.ToArray());
            }
            buttons.Add(new []
            {
                new InlineKeyboardButton("OK", fillCallbackButtonData(Button.Ok))
            });
            return new InlineKeyboardMarkup(buttons.ToArray());
        }
    }
}