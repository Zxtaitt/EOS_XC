using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardDriver
{
    public class BoardDriverEnum
    {
        public enum BoardType
        {
            FW0144E00 = 1,
            FW0325C00 = 2,
            FW0325D00 = 3,
            FW0680A00 = 4,
            FW0061A00 = 5,
            FW0089B00 = 6,
            FW0228B00 = 7,
            FW0207A00 = 8,
            FW0325E00 = 9,
            FW0422400 = 10,
            HW=11,
            FW0325E00_NTE = 12,
            FW0089B00_Net = 13,
            FW0089D00_Net = 14,
            FW0422400_Net = 15,
            FW03744A00 = 16
        }
        public enum BoardChannel
        {
            FW0144E00 = 64,
            FW0325C00 = 48,
            FW0325D00 = 48,
            FW0680A00 = 64,
            FW0061A00 = 48,
            FW0089B00 = 32,
            FW0228B00 = 48,
            FW0207A00 = 64,
            FW0325E00 = 48,
            FW0422400 = 96,
            HW=8,
            FW0325E00_NTE = 48,
            FW0089B00_Net = 32,
            FW0089D00_Net=48,
            FW0422400_Net=96,
            FW03744A00 = 48
        }
        public enum SourceType
        {
            CurrentSource = 0,
            VoltageSource = 1
        }
        public enum PowerMethod
        {
            Single = 0,
            Step = 1
        }
        public enum Direction
        {
            Positive = 0,
            Negative = 1
        }
    }
}
