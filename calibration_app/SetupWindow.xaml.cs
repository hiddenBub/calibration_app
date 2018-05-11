using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using calibration_app.SetOption;

namespace calibration_app
{
    /// <summary>
    /// SetupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetupWindow : Window
    {
        public string EditingId { get; set; }
        ObservableCollection<ColumnSetting> ColumnList { get; set; }   //动态数组
        public SetupWindow()
        {
            InitializeComponent();
           
            ColumnList = new ObservableCollection<ColumnSetting>();
            this.DataContext = this;
            GridGather.Focus();
        }
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void ControlDat_Click(object sender, RoutedEventArgs e)
        //{
        //    FileStream fs = new FileStream("D:\\Program Files\\loggerNet\\datasource\\CR1000XSeries_GHI_SEC.dat", FileMode.Open, FileAccess.Read, FileShare.None, 1, true);
        //    StreamReader sr = new StreamReader(fs);
        //    int i = 1;
        //    while (sr.ReadLine() != "")
        //    {
        //        int range = 5;
        //        string[] temp = new string[range];
        //        string line = sr.ReadLine();
        //        // 第五行开始是数据行
        //        if (i >= 5)
        //        {
                    
        //            int index = i % range;
        //            // 定义数据容量即dat文件中每行作为索引的数据长度
        //            int indexSize = 2;
        //            char[] delimiterChars = { ',' };
        //            string[] tempArr = line.Split(delimiterChars,);

        //        }
        //    }
            
        //}

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            
            if (ofd.ShowDialog() == true)
            {
                string sourceDataPath = ofd.FileName;
                
            }
        }
    }
}
