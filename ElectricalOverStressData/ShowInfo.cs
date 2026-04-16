using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ElectricalOverStressData
{
    public class ShowInfo
    {
        [Category("显示")]
        [DisplayName("01.抽屉编号")]
        [ReadOnly(true)]
        public string DrawerNumber
        { get; set; }
        [Category("显示")]
        [DisplayName("02.计划名称")]
        [ReadOnly(true)]
        public string PlanName
        { get; set; }
        [Category("显示")]
        [DisplayName("03.驱动板类型")]
        [ReadOnly(true)]
        public string BoardType
        { get; set; }

    }
}
