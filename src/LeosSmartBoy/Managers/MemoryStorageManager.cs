using System.Collections.Generic;
using System.Linq;
using LeosSmartBoy.Models;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Managers
{
    public class MemoryStorageManager : IStorageManager
    {
        private readonly ISet<User> users = new HashSet<User>();
        private readonly IDictionary<long, ISet<int>> chatUserListDictionary = new Dictionary<long, ISet<int>>();
        private readonly ISet<Bill> bills = new HashSet<Bill>();

        public void SaveUser(User user)
        {
            users.Add(user);
        }

        public void AddUsersToChat(long chatId, List<User> userList)
        {
            if (!chatUserListDictionary.ContainsKey(chatId))
            {
                chatUserListDictionary.Add(chatId, new HashSet<int>());
            }
            foreach (var user in userList)
            {
                SaveUser(user);
                chatUserListDictionary[chatId].Add(user.Id);
            }
        }

        public List<User> GetChatUsers(long chatId)
        {
            if (!chatUserListDictionary.ContainsKey(chatId)) return new List<User>();
            var userIdSet = chatUserListDictionary[chatId];
            return users.Where(user => userIdSet.Contains(user.Id)).ToList();
        }

        public void SaveBill(Bill bill)
        {
            bills.Add(bill);
        }

        public Bill GetBillWIthMessageId(int messageId)
        {
            return bills.FirstOrDefault(bill => bill.MessageId == messageId);
        }
    }
}