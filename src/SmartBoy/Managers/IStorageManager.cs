using System.Collections.Generic;
using Telegram.Bot.Types;

namespace SmartBoy.Managers
{
    public interface IStorageManager
    {
        void SaveUser(User user);
        User FindUser(string username);
        List<User> AddUsersToChat(long chatId, List<User> userList);
        List<User> GetChatUsers(long chatId);
    }
}