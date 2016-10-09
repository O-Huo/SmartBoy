using System.Collections.Generic;

namespace LeosSmartBoy.Models
{
    public class Bill
    {
        public int Id;
        public long ChatId;
        public int CreatedBy;
        public List<int> SharedWith;
        public float Amount;
    }
}