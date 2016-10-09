using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace LeosSmartBoy.Managers
{
    public class MemoryStorageManager : IStorageManager
    {
        private readonly ISet<User> users = new HashSet<User>();
        private readonly IDictionary<long, ISet<int>> chatUserListDictionary = new Dictionary<long, ISet<int>>();

        public void SaveUser(User user)
        {
            users.Add(user);
        }

        public void AddUsersToChat(Chat chat, List<User> users)
        {
            if (!chatUserListDictionary.ContainsKey(chat.Id))
            {
                chatUserListDictionary.Add(chat.Id, new HashSet<int>());
            }
            foreach (var user in users)
            {
                SaveUser(user);
                chatUserListDictionary[chat.Id].Add(user.Id);
            }
        }

        public List<User> GetChatUsers(Chat chat)
        {
            if (!chatUserListDictionary.ContainsKey(chat.Id)) return null;
            var userIdSet = chatUserListDictionary[chat.Id];
            return users.Where(user => userIdSet.Contains(user.Id)).ToList();
        }

    }
}