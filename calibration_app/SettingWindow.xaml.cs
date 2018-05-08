using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace calibration_app
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            ArrayList dataList = new ArrayList();
            GatherSetting GHI = new GatherSetting("GHI", "byte", 1, "sum");
            GatherSetting GTI = new GatherSetting("GTI");
            dataList.Add(GHI);
            dataList.Add(GTI);
            GridGather.ItemsSource = dataList;
        }

        

        

        //public string[] DataType { get => dataType; }

        //public string[] DataName { get => dataName; set => dataName = value; }
    }

   
}
