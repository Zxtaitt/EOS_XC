namespace BoardDriver
{
    /// <summary>
    /// 夹具配置
    /// </summary>
    public class ClampConfig
    {
        /// <summary>
        /// 无参的构造函数
        /// </summary>
        public ClampConfig() { }
        /// <summary>
        /// 带全参的构造函数
        /// </summary>
        public ClampConfig(string clampName, bool browsable, string displayName, string description, object value)
        {
            ClampName = clampName;
            Browsable = browsable;
            DisplayName = displayName;
            Description = description;
            Value = value;
        }
        public string ClampName { get; set; }
        public bool Browsable { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public object Value { get; set; }
    }
}
