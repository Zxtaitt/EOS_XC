using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ElectricalOverStressData
{
    [Serializable]
    public class CommunicationBoardItem
    {
        [DisplayName("驱动板位置")]
        
        [ReadOnly(true)]
        public DataEnum.Location BoardLocation
        { get; set; } = DataEnum.Location._1;
        [DisplayName("驱动板编号")]
        [ReadOnly(true)]
        public int BoardIndex
        { get; set; } = 1;
        [DisplayName("驱动板地址")]
        public int BoardAddress
        { get; set; } = 1;
    }
}
