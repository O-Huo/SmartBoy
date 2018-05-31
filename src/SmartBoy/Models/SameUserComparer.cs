using System.Collections.Generic;
using Telegram.Bot.Types;

namespace SmartBoy.Models
{
    class SameUserComparer : EqualityComparer<User>
    {
        public override bool Equals(User u1, User u2)
        {
            return u1.Id == u2.Id;
        }

        public override int GetHashCode(User u)
        {
            return u.Id;
        }
    }
}