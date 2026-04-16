using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalOverStressData
{
    public class DataEnum
    {
        public enum Layer
        {
            L1 = 1,
            L2 = 2,
            L3 = 3,
            L4 = 4,
            L5 = 5,
            L6 = 6,
            L7 = 7,
            L8 = 8,
            L9 = 9,
            L10 = 10,
            L11 = 11
        }
        public enum Location
        {
            _1 = 1,
            _2 = 2,
            _3 = 3,
            _4 = 4
        }
        public enum Result
        {
            Pass = 1,
            Fail = 2,
            Skip = 3
        }
        public enum TriggerSource
        {
            CHANnel1 = 1,
            CHANnel2 = 2,
            CHANnel3 = 3,
            CHANnel4 = 4
        }
        public enum TriggerSlope
        {
            POSitive = 1,
            NEGative = 2,
            ALTernate = 3,
            EITHer = 4
        }
        public enum ChannelSource
        {
            VOLT = 1,
            AMPere = 2
        }
    }
}
