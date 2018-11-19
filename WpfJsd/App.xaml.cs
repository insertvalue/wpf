using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfJsd.Core;
using WpfJsd.Model;

namespace WpfJsd
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static List<Warehouse> WhList { get; set; }

        public static LoginUser User { get; set; }

        public static int CurrentWh { get; set; }
    }
}
