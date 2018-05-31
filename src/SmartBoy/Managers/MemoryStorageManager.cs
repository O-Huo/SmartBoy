using System.Collections.Generic;
using System.Linq;
using SmartBoy.Models;
using Telegram.Bot.Types;

namespace SmartBoy.Managers
{
    public class MemoryStorageManager : IStorageManager
    {
        private readonly ISet<User> users = new HashSet<User>(new SameUserComparer());
        private readonly IDictionary<long, ISet<int>> chatUserListDictionary = new Dictionary<long, ISet<int>>();
        public void SaveUser(User user)
        {
            users.Remove(user);
            users.Add(user);
        }

        public User FindUser(string username)
        {
            var u = users.FirstOrDefault(x => x.Username?.Equals(username.Substring(1)) ?? false);
            return u;
        }

        public List<User> AddUsersToChat(long chatId, List<User> userList)
        {
            var newUsers = new List<User>();
            if (!chatUserListDictionary.ContainsKey(chatId))
            {
                chatUserListDictionary.Add(chatId, new HashSet<int>());
            }
            foreach (var user in userList)
            {
                SaveUser(user);
                if (!chatUserListDictionary[chatId].Contains(user.Id))
                {
                    chatUserListDictionary[chatId].Add(user.Id);
                    newUsers.Add(user);
                }
            }
            return newUsers;
        }

        public List<User> GetChatUsers(long chatId)
        {
            if (!chatUserListDictionary.ContainsKey(chatId)) return new List<User>();
            var userIdSet = chatUserListDictionary[chatId];
            return users.Where(user => userIdSet.Contains(user.Id)).ToList();
        }
    }
}