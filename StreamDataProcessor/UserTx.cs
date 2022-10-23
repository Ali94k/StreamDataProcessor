using System;

namespace StreamDataProcessor
{
    public class UserTx
    {
        public string UserId { get; set; }
        public DateTime TxDate { get; set; }
        public int TotalAmount { get; set; }
    }
}
