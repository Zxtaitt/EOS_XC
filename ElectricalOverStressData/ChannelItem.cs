using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BoardDriver;

namespace ElectricalOverStressData
{
    [Serializable]
    public class ChannelItem
    {
        [Category("通道配置")]
        [DisplayName("01.上电类型")]
        public BoardDriverEnum.SourceType SourceType
        { get; set; }
        [Category("通道配置")]
        [DisplayName("02.设置值(mA/V)")]
        public double SourceValue
        { get; set; }
        [Category("通道配置")]
        [DisplayName("03.上电方法")]
        public BoardDriverEnum.PowerMethod SetMethod
        { get; set; }
        [Category("通道配置")]
        [DisplayName("04.步进数")]
        public int StepCount
        { get; set; } = 5;
        [Category("通道配置")]
        [DisplayName("05.示波器通道")]
        public DataEnum.TriggerSource OscilloscopeTriggerSource
        { get; set; } = DataEnum.TriggerSource.CHANnel1;
        [Category("通道配置")]
        [DisplayName("06.示波器通道模式")]
        public DataEnum.ChannelSource OscilloscopeChannelSource
        { get; set; } = DataEnum.ChannelSource.VOLT;
        [Category("通道配置")]
        [DisplayName("07.示波器触发斜率")]
        public DataEnum.TriggerSlope OscilloscopeTriggerSlope
        { get; set; } = DataEnum.TriggerSlope.EITHer;
        [Category("通道配置")]
        [DisplayName("08.示波器触发电平(V/A)")]
        public double OscilloscopeTriggerLevel
        { get; set; } = 0.5;
        [Category("通道配置")]
        [DisplayName("09.示波器通道量程(V/A)")]
        public double OscilloscopeChannelRange
        { get; set; } = 0.5;
        [Category("通道配置")]
        [DisplayName("10.示波器基础延时(S)")]
        public double OscilloscopeBaseDelay
        { get; set; } = 0.6;
        [Category("通道配置")]
        [DisplayName("11.示波器基础时间刻度(S)")]
        public double OscilloscopeTimeScale
        { get; set; } = 0.3;
    }
}
