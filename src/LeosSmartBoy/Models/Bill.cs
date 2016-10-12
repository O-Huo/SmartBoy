using System.Collections.Generic;

namespace LeosSmartBoy.Models
{
    public class Bill
    {
        public enum Status
        {
            SetAmountIntegeral,
            SetAmountFractional,
            SetSharedWith,
            Sealed
        }
        public int Id;
        public long ChatId;
        public int CreatedBy;
        public ISet<int> SharedWith;
        public float Amount;
        public Status CurrentStatus;
    }
}