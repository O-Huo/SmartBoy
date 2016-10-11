using System.Collections.Generic;
using LeosSmartBoy.Models;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Managers
{
    public interface IStorageManager
    {
        void SaveUser(User user);
        void AddUsersToChat(Chat chat, List<User> userList);
        List<User> GetChatUsers(Chat chat);

        void SaveBill(Bill bill);
        Bill GetBill(int id);
    }
}