namespace BoardDriver
{
    /// <summary>
    /// 夹具设置
    /// </summary>
    public class ClampSetting
    {
        public double ClampVoltagePostive { get; set; }

        public double ClampVoltageNegtive { get; set; }

        public double ClampCurrentPostive { get; set; }

        public double ClampCurrentNegtive { get; set; }

        public double[] CSPVoltage { get; set; }

        public double[] CSPVoltageMax { get; set; }

        public double[] CSPVoltageMin { get; set; }

        public double[] CSNVoltage { get; set; }

        public double[] CSNVoltageMax { get; set; }

        public double[] CSNVoltageMin { get; set; }

    }
}
