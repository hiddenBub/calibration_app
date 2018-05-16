using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Ink;
using System.Drawing;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Drawing.Drawing2D;

namespace calibration_app
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isGather = false;

        



        public bool IsGather
        {
            get => isGather;
            set {
                if ( value != false && value != true)
                {
                    isGather = false;
                } 
                else
                {
                    isGather = value;
                }
            } 
        }

        


        private List<SeriesCollection> seriesCollection = new List<SeriesCollection>(4);

        private List<string[]> labels = new List<string[]>(4);

        private List<Func<double, string>> yFormatter = new List<Func<double, string>>(4);
        public List<SeriesCollection> SeriesCollection { get => seriesCollection; set => seriesCollection = value; }
        public List<string[]> Labels { get => labels; set => labels = value; }
        public List<Func<double, string>> YFormatter { get => yFormatter; set => yFormatter = value; }

        public MainWindow()
        {
            
            
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {

                SeriesCollection temp= new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Series 1",
                        Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                    },
                    new ScatterSeries
                    {
                        Title = "Series 2",
                        Values = new ChartValues<ObservablePoint>
                        {
                            new ObservablePoint(0, 5),
                            new ObservablePoint(1, 7),
                            new ObservablePoint(2, 6),
                            new ObservablePoint(3, 4),
                            new ObservablePoint(4, 5)
                        },
                        
                        PointGeometry = DefaultGeometries.Triangle,
                    },
                   
                };
                SeriesCollection.Add(temp);

                string[] temp_L = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
                Labels.Add(temp_L);
                Func<double, string> temp_YFormatter = value => value.ToString("C");
                YFormatter.Add(temp_YFormatter);
                

            }

            

            
            DataContext = this;
        }
        

        private void ImportSource_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("导入数据源");
        }

        private void ImportCalibration_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("导入校准数据");
        }
        private void ImportDataMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("导入校准数据");
        }

        /// <summary>
        /// 菜单栏采集状态切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GatherMenu_Click(object sender, RoutedEventArgs e)
        {
            bool state = IsGather;
            // 没有开始采集时开始采集，并将说明文字更改为结束采集
            SwitchGather(state);
        }

        /// <summary>
        /// 工具栏采集状态切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GatherCB_Click(object sender, RoutedEventArgs e)
        {
            bool state = IsGather;
            // 没有开始采集时开始采集，并将说明文字更改为结束采集
            SwitchGather(state);
        }

        private void CloseWindowMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingMenu_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void USBBtn_Click(object sender,RoutedEventArgs e)
        {

        }



        /// <summary>
        /// 切换采集开关
        /// </summary>
        /// <param name="state">当前是否在采集中</param>
        public void SwitchGather(bool state)
        {
            if (!state)
            {
                GatherCB.Content = GatherMenu.Header = "结束采集";
            }
            else
            {
                GatherCB.Content = GatherMenu.Header = "开始采集";
            }
            IsGather = !state;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bit = new Bitmap(Convert.ToInt32(this.ChartZone.Width), Convert.ToInt32(this.ChartZone.Height));//实例化一个和窗体一样大的bitmap
            Graphics g = Graphics.FromImage(bit);
            g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
            g.CopyFromScreen(Convert.ToInt32(this.Left), Convert.ToInt32(this.Top), 0, 0, new System.Drawing.Size(Convert.ToInt32(this.ChartZone.Width), Convert.ToInt32(this.ChartZone.Height)));//保存整个窗体为图片
            //g.CopyFromScreen(panel游戏区 .PointToScreen(Point.Empty), Point.Empty, panel游戏区.Size);//只保存某个控件（这里是panel游戏区）
            bit.Save("weiboTemp.png");//默认保存格式为PNG，保存成jpg格式质量不是很好
        }


        
    }
}
