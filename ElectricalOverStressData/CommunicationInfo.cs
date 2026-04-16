using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalOverStressData
{
    public class CommunicationInfo
    {
        public CommunicationInfo()
        {
            if (BoardItem == null)
            {
                BoardItem = new List<CommunicationBoardItem>();
            }
            if (OtherItem == null)
            {
                OtherItem = new CommunicationOtherItem();
            }
        }
        public CommunicationOtherItem OtherItem
        { get; set; }
        public List<CommunicationBoardItem> BoardItem
        { get; set; }               
    }
}
