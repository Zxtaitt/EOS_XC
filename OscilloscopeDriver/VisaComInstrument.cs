using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivi.Visa.Interop;
using System.Runtime.InteropServices;

namespace OscilloscopeDriver
{
    public class VisaComInstrument
    {
        private ResourceManagerClass m_ResourceManager;
        private FormattedIO488Class m_IoObject;
        private string m_strVisaAddress;
        public VisaComInstrument(string strVisaAddress)
        {
            m_strVisaAddress = strVisaAddress;
            OpenIo();
            m_IoObject.IO.Clear();
        }
        public void DoCommand(string strCommand)
        {
            m_IoObject.WriteString(strCommand, true);
        }
        public void DoCommandIEEEBlock(string strCommand, byte[] DataArray)
        {
            // Send the command to the device. 
            m_IoObject.WriteIEEEBlock(strCommand, DataArray, true);
            // Check for inst errors. 
            CheckInstrumentErrors(strCommand);
        }
        public string DoQueryString(string strQuery)
        {
            // Send the query.
            m_IoObject.WriteString(strQuery, true);
            // Get the result string.
            string strResults;
            strResults = m_IoObject.ReadString();
            // Check for inst errors.
            CheckInstrumentErrors(strQuery);
            // Return results string.
            return strResults;
        }
        public double DoQueryNumber(string strQuery)
        {
            // Send the query.
            m_IoObject.WriteString(strQuery, true);
            // Get the result number.
            double fResult;
            fResult = (double)m_IoObject.ReadNumber(IEEEASCIIType.ASCIIType_R8, true);
            // Check for inst errors. 
            CheckInstrumentErrors(strQuery);
            // Return result number. 
            return fResult;
        }
        public double[] DoQueryNumbers(string strQuery)
        {
            // Send the query. 
            m_IoObject.WriteString(strQuery, true);
            // Get the result numbers.
            double[] fResultsArray;
            fResultsArray = (double[])m_IoObject.ReadList(IEEEASCIIType.ASCIIType_R8, ",;");
            // Check for inst errors.
            CheckInstrumentErrors(strQuery);
            // Return result numbers.
            return fResultsArray;
        }
        public byte[] DoQueryIEEEBlock(string strQuery)
        {
            // Send the query. 
            m_IoObject.WriteString(strQuery, true);
            // Get the results array. 
            System.Threading.Thread.Sleep(2000);
            // Delay before reading. 
            byte[] ResultsArray;
            ResultsArray = (byte[])m_IoObject.ReadIEEEBlock(IEEEBinaryType.BinaryType_UI1, false, true);
            // Check for inst errors.
            CheckInstrumentErrors(strQuery);
            // Return results array.
            return ResultsArray;
        }
        private void CheckInstrumentErrors(string strCommand)
        {
            // Check for instrument errors.
            string strInstrumentError;
            bool bFirstError = true;
            do // While not "0,No error". 
            {
                m_IoObject.WriteString(":SYSTem:ERRor?", true);
                strInstrumentError = m_IoObject.ReadString();
                if (!strInstrumentError.ToString().StartsWith("+0,"))
                {
                    if (bFirstError)
                    {
                        //Console.WriteLine("ERROR(s) for command '{0}': ", strCommand);
                        bFirstError = false;
                    }
                    //Console.Write(strInstrumentError);
                }
            } while (!strInstrumentError.ToString().StartsWith("+0,"));
        }
        private void OpenIo()
        {
            m_ResourceManager = new ResourceManagerClass();
            m_IoObject = new FormattedIO488Class();
            // Open the default VISA COM IO object.
            try
            {
                m_IoObject.IO = (IMessage)m_ResourceManager.Open(m_strVisaAddress, AccessMode.NO_LOCK, 0, "");
            }
            catch (Exception e)
            {
                //Console.WriteLine("An error occurred: {0}", e.Message);
            }
        }
        public void SetTimeoutSeconds(int nSeconds)
        {
            m_IoObject.IO.Timeout = nSeconds * 1000;
        }
        public void Close()
        {
            try
            {
                m_IoObject.IO.Close();
            }
            catch { }
            try
            {
                Marshal.ReleaseComObject(m_IoObject);
            }
            catch { }
            try
            {
                Marshal.ReleaseComObject(m_ResourceManager);
            }
            catch { }
        }
    }
}
