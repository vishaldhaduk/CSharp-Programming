using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMaster
{
    public class MemberMaster
    {
        public int ID { get; set; }
        public string BarcodeId { get; set; }
        public bool IsPrimary { get; set; }
        public int FamilyId { get; set; }
        public string Title { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
    }
}
