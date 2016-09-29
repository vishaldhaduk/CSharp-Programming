using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMaster
{
    public class MemberAccountMaster
    {
        public int ID { get; set; }
        //[ForeignKey("MemberMaster")]
        public int MemberID { get; set; }
        public bool Paid { get; set; }
        public string Amount { get; set; }
        public string DepositDate { get; set; }
        public string PaymentType { get; set; }
        public string Comment { get; set; }
    }
}
