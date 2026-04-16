using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalOverStressData
{
    public class DetectionResult
    {
        public int Channel
        { get; set; }
        public DataEnum.Result Result
        { get; set; }
    }
}
