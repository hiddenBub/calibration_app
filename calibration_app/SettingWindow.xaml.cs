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
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.WinUsb;
using calibration_app.SetOption;

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
            ColumnSetting GHI = new ColumnSetting("GHI", "byte", 1, "sum");
            ColumnSetting GTI = new ColumnSetting("GTI");
            dataList.Add(GHI);
            dataList.Add(GTI);
            GridGather.ItemsSource = dataList;
        }

        

        

        //public string[] DataType { get => dataType; }

        //public string[] DataName { get => dataName; set => dataName = value; }
    }

   
}
