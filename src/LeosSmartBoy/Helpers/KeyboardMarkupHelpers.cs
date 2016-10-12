using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LeosSmartBoy.Helpers
{
    public class KeyboardMarkupHelpers
    {
        public static InlineKeyboardMarkup CreateDigitInlineKeyboardMarkup()
        {
            var buttons = new List<InlineKeyboardButton[]>();
            for (var row = 0; row < 3; row++)
            {
                var rowButtons = new List<InlineKeyboardButton>();
                for (var col = 0; col < 3; col++)
                {
                    var index = row*3 + col + 1;
                    rowButtons.Add(new InlineKeyboardButton(index.ToString(), index.ToString()));
                }
                buttons.Add(rowButtons.ToArray());
            }
            buttons.Add(new []
            {
                new InlineKeyboardButton("0", "0"), 
                new InlineKeyboardButton(".", "."), 
                new InlineKeyboardButton("\u2190", "back")
            });
            buttons.Add(new []
            {
                new InlineKeyboardButton("OK", "OK")
            });
            return new InlineKeyboardMarkup(buttons.ToArray());
        }

        private delegate string UserNameBuilder(User user);
        public static InlineKeyboardMarkup CreateUserSelectionKeyboardMarkup(IList<User> users, ISet<int> selectedUser)
        {
            UserNameBuilder userNameBuilder = (User user) =>
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
                rowButton.Add(new InlineKeyboardButton(userNameBuilder(user), user.Id.ToString()));
                if (rowButton.Count != 3) continue;
                buttons.Add(rowButton.ToArray());
                rowButton = new List<InlineKeyboardButton>();
            }
            if (rowButton.Count > 0)
            {
                buttons.Add(rowButton.ToArray());
            }
            return new InlineKeyboardMarkup(buttons.ToArray());
        }
    }
}