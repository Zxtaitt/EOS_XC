using System.Collections.Generic;

namespace BoardDriver
{
    /// <summary>
    /// 驱动板配置基类
    /// </summary>
    public class BaseBoardSetting
    {
        public BaseBoardSetting()
        {
            if (ChannelSourceIsCurrentDictionary == null)
            {
                ChannelSourceIsCurrentDictionary = new Dictionary<int, bool>();
            }
            if (ChannelMuzzleDictionary == null)
            {
                ChannelMuzzleDictionary = new Dictionary<int, double>();
            }
            if (ChannelDirectionIsPositiveDictionary == null)
            {
                ChannelDirectionIsPositiveDictionary = new Dictionary<int, bool>();
            }
            if (ProductPDChannelList == null)
            {
                ProductPDChannelList = new List<int>();
            }
            if (ProductMPDChannelList == null)
            {
                ProductMPDChannelList = new List<int>();
            }
            if (Clamp_Setting == null)
            {
                Clamp_Setting = new ClampSetting();
            }
        }

        public Dictionary<int, bool> ChannelSourceIsCurrentDictionary { get; set; }

        public Dictionary<int, double> ChannelMuzzleDictionary { get; set; }

        public Dictionary<int, bool> ChannelDirectionIsPositiveDictionary { get; set; }

        public List<int> ProductPDChannelList { get; set; }

        public List<int> ProductMPDChannelList { get; set; }

        public ClampSetting Clamp_Setting { get; set; }
    }
}
