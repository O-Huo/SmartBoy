using System.Collections.Generic;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Managers
{
    public interface IStorageManager
    {
        void SaveUser(User user);
        void AddUsersToChat(Chat chat, List<User> users);
        public List<User> GetChatUsers(Chat chat);
    }
}