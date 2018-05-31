using System.Collections.Generic;
using SmartBoy.Models;
using Telegram.Bot.Types;

namespace SmartBoy.Managers
{
    public interface IStorageManager
    {
        void SaveUser(User user);
        List<User> AddUsersToChat(long chatId, List<User> userList);
        List<User> GetChatUsers(long chatId);

        void SaveBill(Bill bill);
        Bill GetBillWIthMessageId(int id);
    }
}