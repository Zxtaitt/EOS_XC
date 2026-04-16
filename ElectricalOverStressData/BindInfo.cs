using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ElectricalOverStressData
{
    [Serializable]
    public class BindInfo
    {
        [Category("绑定")]
        [DisplayName("01.层名称")]
        public DataEnum.Layer LayerName
        { get; set; }
        [Category("绑定")]
        [DisplayName("02.位置名称")]
        public DataEnum.Location LocationName
        { get; set; }
        [Category("绑定")]
        [DisplayName("03.开始通道")]
        [TypeConverter(typeof(ChannelGenderItem))]
        public string StartChannel
        { get; set; }
        [Category("绑定")]
        [DisplayName("04.结束通道")]
        [TypeConverter(typeof(ChannelGenderItem))]
        public string EndChannel
        { get; set; }
        private class ChannelGenderItem : StringConverter
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
                if (context != null && context.Instance is BindInfo)
                {
                    return new StandardValuesCollection(Global.Channel);
                }
                return base.GetStandardValues(context);
            }
        }
    }
}
