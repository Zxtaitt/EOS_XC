using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardDriver
{
   public class BoardDriver_FW0228A00:BoardDriver_FW0089A00
    {
        protected override void SetBoardOnSingleChannel(int Channel)
        {
            
            int espectLength = 8;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, (byte)(Channel), 0x01 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set DVB Board On Single Exception");
        }
        protected override void SetBoardOffSingleChannel(int Channel)
        {
            
            int espectLength = 8;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x10, 0x85, 0x00, (byte)(Channel), 0x00 };
                byte[] response = this.QueryProduct(cmd, espectLength);
                if ((response.Length == espectLength) && Tool.CheckFeedback(response[5]))
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set DVB Board Off Single Exception");
        }
    }
}
