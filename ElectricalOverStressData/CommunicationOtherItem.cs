using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ElectricalOverStressData
{
    [Serializable]
    public class CommunicationOtherItem
    {
        [Category("配置")]
        [DisplayName("01.驱动板串口号")]
       // [TypeConverter(typeof(SerialPortGenderItem))]
        public string BoardSerialPort
        { get; set; }

        [Category("配置")]
        [DisplayName("02.驱动板数量")]
        public int BoardNumber
        { get; set; } = 1;
        [Category("配置")]
        [DisplayName("03.示波器地址")]
        public string OscilloscopeAaddress
        { get; set; }
        [Category("配置")]
        [DisplayName("04.测试计划")]
        [TypeConverter(typeof(PlanNameGenderItem))]
        public string PlanName
        { get; set; }
        [Category("配置")]
        [DisplayName("05.图片保存地址")]
        public string IamgePath
        { get; set; } = "D:\\ElectricalOverStressPicture";
        private class PlanNameGenderItem : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (context != null && context.Instance is CommunicationOtherItem)
                {
                    return new StandardValuesCollection(Global.PlanName);
                }
                return base.GetStandardValues(context);
            }
        }
        private class SerialPortGenderItem : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (context != null && context.Instance is CommunicationOtherItem)
                {
                    return new StandardValuesCollection(Global.SerialPortName);
                }
                return base.GetStandardValues(context);
            }
        }
    }
}
