using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalOverStressData
{
    public class Global
    {
        public static List<string> SerialPortName
        { get; set; }
        public static List<string> PlanName
        { get; set; }
        public static string CommunicationPath
        { get; set; }
        public static string CommunicationPathName
        { get; set; }
        public static string PlanPath
        { get; set; }
        public static List<string> Channel
        { get; set; }
    }
}
