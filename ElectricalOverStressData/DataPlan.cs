using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalOverStressData
{
    public class DataPlan
    {
        public DataPlan()
        {           
            if (ChannelItem == null)
            {
                ChannelItem = new SerializableDictionary<int, List<ChannelItem>>();
            }
        }
        public string PlanName
        { get; set; }
        public int ChannelCount
        { get; set; }
        public BoardDriver.BoardDriverEnum.BoardType BoardType
        { get; set; }
        public SerializableDictionary<int, List<ChannelItem>> ChannelItem
        { get; set; }
    }
}
