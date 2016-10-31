using System;
using System.Collections.Generic;
using System.Text;

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
        public int MessageId;
        public long ChatId;
        public int CreatedBy;
        public ISet<int> SharedWith;
        public StringBuilder AmountString = new StringBuilder();
        public double Amount => Convert.ToDouble(AmountString.ToString());
        public Status CurrentStatus;
    }
}