using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
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
using calibration_app.SetOption;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Data;

namespace calibration_app
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isGather = false;

        /// <summary>
        /// 表格容器根据所选选项卡布置表格
        /// </summary>
        private Grid ChartZone = new Grid
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
           
        };


        bool first = true;


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

        private ulong spendTime = 0;

        private List<decimal> dataTemp = new List<decimal>();

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string strConn = "";

        /// <summary>
        /// 系统设置
        /// </summary>
        private Setting setting;

        /// <summary>
        /// 计时器
        /// </summary>
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        /// <summary>
        /// 操作的选项卡索引
        /// </summary>
        private int optionIndex = 0;

        /// <summary>
        /// 图表LIST
        /// </summary>
        private List<CartesianChart> cartesianChart = new List<CartesianChart>();

        /// <summary>
        /// 图表内数据集LIST
        /// </summary>
        private List<SeriesCollection> seriesCollection = new List<SeriesCollection>();

        /// <summary>
        /// 图标内标签LIST
        /// </summary>
        private List<List<string>> labels = new List<List<string>>();

        /// <summary>
        /// 图标内Y轴数据格式化LIST
        /// </summary>
        private List<Func<double, string>> yFormatter = new List<Func<double, string>>();
        public List<SeriesCollection> SeriesCollection { get => seriesCollection; set => seriesCollection = value; }
        public List<List<string>> Labels { get => labels; set => labels = value; }
        public List<Func<double, string>> YFormatter { get => yFormatter; set => yFormatter = value; }
        public List<CartesianChart> CartesianChart { get => cartesianChart; set => cartesianChart = value; }
        public Setting Setting { get => setting; set => setting = value; }

        public MainWindow()
        {
            
            
            InitializeComponent();
            
        }
        

        private void ImportSource_Click(object sender, RoutedEventArgs e)
        {
            bool IsGathering = isGather;
            if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据","错误");
            }
            
        }

        private void ImportCalibration_Click(object sender, RoutedEventArgs e)
        {
            bool IsGathering = isGather;
            if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据", "错误");
            }
            //else
            //{
            //    //打开一个文件选择框
            //    OpenFileDialog ofd = new OpenFileDialog();
            //    ofd.Title = "Excel文件";
            //    ofd.FileName = "";
            //    ofd.Filter = "Excel文件(*.xls)|*";
            //    DataTable dt = new DataTable();
            //    string filePath = Application.StartupPath + @"\Skills_TreeView\Skills.xls";

            //    string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
            //    using (OleDbConnection conn = new OleDbConnection(strConn))
            //    {
            //        conn.Open();
            //        string strExcel = "select * from [sheet1$]";
            //        OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
            //        myCommand.Fill(dt);
            //    }
            //    return dt;
            //}
        }
        //private void ImportDataMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("导入校准数据");
        //}

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
                // 将开始采集时间点写入到文件做记录
                FileStream fs = new FileStream(@".\Gather.txt", FileMode.Create);
                string start = "2018-05-09 21:16:39\r\n";
                //获得字节数组
                byte[] data = Encoding.Default.GetBytes(start);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                GatherCB.Content = GatherMenu.Header = "结束采集";
                // 重新获取图表
                GetChart();
                //定时查询-定时器
                dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
            }
            else
            {
                // 结束采集时将时间记录并写入文件
                FileStream fs = new FileStream(@".\Gather.txt", FileMode.Append);
                string start = DateTime.Now.ToString() + "\r\n";
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(start);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                GatherCB.Content = GatherMenu.Header = "开始采集";
                dispatcherTimer.Stop();
            }
            IsGather = !state;
        }

        


        /// <summary>
        /// 页面载入成功时的逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            first = false;
            // 读取本地设置文件中的设置
            Setting = DeserializeFromXml<Setting>(@".\Setting.xml");
            // 遍历用户所做配置的字段
            foreach (Column temp in setting.Gather.ColumnList)
            {
                // 根据字段名称布置选项卡并添加至ColTab内
                TabItem myDnymicTab = new TabItem() { Header = temp.Name, MaxHeight = 20, MaxWidth = 78 };
                ColTab.Items.Add(myDnymicTab);

            }
            // 页面载入成功后直接选择第一个选项卡
            ColTab.SelectedItem = ColTab.Items[0];


            // 初始化chartZone的Grid控件
            AddCell(2, 2);

            GetChart();
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            //Bitmap bit = new Bitmap(Convert.ToInt32(this.ChartZone.Width), Convert.ToInt32(this.ChartZone.Height));//实例化一个和窗体一样大的bitmap
            //Graphics g = Graphics.FromImage(bit);
            //g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
            //g.CopyFromScreen(Convert.ToInt32(this.Left), Convert.ToInt32(this.Top), 0, 0, new System.Drawing.Size(Convert.ToInt32(this.ChartZone.Width), Convert.ToInt32(this.ChartZone.Height)));//保存整个窗体为图片
            ////g.CopyFromScreen(panel游戏区 .PointToScreen(Point.Empty), Point.Empty, panel游戏区.Size);//只保存某个控件（这里是panel游戏区）
            //bit.Save("weiboTemp.png");//默认保存格式为PNG，保存成jpg格式质量不是很好
        }

        /// <summary>     
        /// 从某一XML文件反序列化到某一类型   
        /// </summary>    
        /// <param name="filePath">待反序列化的XML文件名称</param>  
        /// <param name="type">反序列化出的</param>  
        /// <returns></returns>    
        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    throw new ArgumentNullException(filePath + " not Exists");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 切换选项卡后的逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColTab_Change(object sender, SelectionChangedEventArgs e)
        {
            // 判断当前是否是首次进入页面，主要用于屏蔽一些bug
            if (!first)
            {
                // 设置索引初始值
                int i = 0;
                // 遍历选项卡数据(初始化原始选项卡标签)
                foreach (TabItem it in ColTab.Items)
                {
                    // 将当前选项卡的对象存至item变量中
                    var item = ColTab.ItemContainerGenerator.ContainerFromItem(ColTab.Items[i]) as TabItem;
                    // 取得当前选项卡的显示标题
                    string header = item.Header.ToString();
                    // 将header分割成数组
                    string[] headers = header.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    // 取得只需显示的部分
                    item.Header = headers[0];
                    // 将选项卡里的内容移除，用作数据表grid单例构造
                    item.Content = null;
                    // 更新索引
                    i++;
                }
                // 取得当前操作的选项卡并将该对象存至temp变量中
                var temp = ColTab.ItemContainerGenerator.ContainerFromItem(ColTab.SelectedItem) as TabItem;
                optionIndex = ColTab.SelectedIndex;
                // 以可显示的方式展示被选中选项卡
                temp.Header += "-selected";
                // 将图表grid存放至选项卡中
                temp.Content = ChartZone;
            }
            
        }

        /// <summary>
        /// 定时器回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            string filePath = "./Gather.txt";
            DateTime startTime = DateTime.Now;
            DateTime? endTime = null;
            if (File.Exists(filePath))
            {
                // 读取gather.txt文件获取起始和结束采集时间
                StreamReader sr = new StreamReader(@"./Gather.txt", Encoding.Default);
                // 定义起始时间
                startTime = Convert.ToDateTime(sr.ReadLine());
                // 结束时间
                endTime = startTime.AddSeconds(spendTime);
                sr.Close();
            }
            // 获取当前周期内的起始时间
            string date_temp = string.Format("{0:g}", startTime);
            date_temp += ":00";
            // 获取当前周期内的频率
            double fre = Setting.Gather.ColumnList[optionIndex].Frequency;
            int i = 1;
            // 获取数据
            string dataPath = Setting.Gather.DataPath;
            string[] data = File.ReadAllLines(dataPath, Encoding.Default);
            foreach (string line in data)
            {
                if (i >= 5)
                {
                    // 存储当前行
                    string a = line;
                    // 设置分割字符
                    char[] sp = { ',', '"' };
                    // 存储数据型
                    string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (endTime != null && (DateTime.Compare(Convert.ToDateTime(endTime), Convert.ToDateTime(datas[0])) < 0)) break;
                    
                    // 获取当前选择选项卡的数据模型
                    Column column = Setting.Gather.ColumnList[optionIndex];

                    int length = labels[0].Count;
                    string lastTimeStemp = labels[0][(length) - 1];
                    // 将数据中的时间取出
                    DateTime dateTime = Convert.ToDateTime(datas[0]);
                    // 当前数据中的时间与X轴的计量点中坐标轴时间较大时
                    if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStemp)) <= 0)
                    {
                        if (!decimal.TryParse(datas[optionIndex + 2], out decimal x))
                        {
                            continue;
                        }
                        // 将取得的数据存入
                        dataTemp.Add(x);
                        // 获取临时数据存储数组项目数量                                
                        int count = dataTemp.Count;
                        // 对数据进行求和
                        decimal sum = dataTemp.Sum();
                        // 根据当前选项卡的设置获取正确的数据
                        if (column.Method.ToLower() == "avg")
                        {
                            if (SeriesCollection[0][0].Values.Count > 0 && dataTemp.Count > 1)
                            {
                                SeriesCollection[0][0].Values.RemoveAt(SeriesCollection[0][0].Values.Count - 1);
                            }
                            SeriesCollection[0][0].Values.Add(Convert.ToDouble(sum / count));
                        }
                        else if (column.Method.ToLower() == "sum")
                        {
                            if(SeriesCollection[0][0].Values.Count > 0 && dataTemp.Count > 1)
                            {
                                SeriesCollection[0][0].Values.RemoveAt(SeriesCollection[0][0].Values.Count - 1);
                            }
                            SeriesCollection[0][0].Values.Add(Convert.ToDouble(sum));
                        }
                        
                        // 当两个相等时获取
                        if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStemp)) == 0)
                        {
                            // 将新的坐标轴时间加入Labels数组

                            Labels[0].Add(Convert.ToDateTime(lastTimeStemp).AddSeconds(column.Frequency).ToString());

                            dataTemp.Clear();
                        }
                    }
                    
                }
                i++;
            }
            spendTime++;
        }

        /// <summary>
        /// 为表格ChartZone填充行列
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void AddCell(int row,int col)
        {
            // 布置行分割
            for (int i = 0; i < row; i++)
            {
                ChartZone.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < col; j++)
            {
                ChartZone.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }


        private void GetChart()
        {
            string filepath = @"./calibrationData.dat";
            // 将chartzone内所有子元素清空
            ChartZone.Children.Clear();
            // 已经上传需校准数据时
            if (File.Exists(filepath))
            {

            }
            // 没有上传校准数据时(包括采集中/采集结束但并未上传数据集)
            else
            {
               
                string filePath = "./Gather.txt";
                // 起始时间
                DateTime startTime = DateTime.Now;
                // 结束时间
                DateTime? endTime = null;
                if (File.Exists(filePath))
                {
                    // 读取gather.txt文件获取起始和结束采集时间
                    StreamReader sr = new StreamReader(@"./Gather.txt", Encoding.Default);
                    // 设置当前行数
                    int index = 1;
                    
                    
                    // 定义当前行内容
                    String lineSr;
                    // 
                    while ((lineSr = sr.ReadLine()) != null)
                    {
                        if (index == 1)
                        {
                            startTime = Convert.ToDateTime(lineSr);
                        }
                        else if (index == 2)
                        {
                            endTime = Convert.ToDateTime(lineSr);
                        }
                        index++;
                    }
                    if (endTime == null) endTime = startTime;
                    sr.Close();
                }
                // 获取当前周期内的起始时间
                string date_temp = string.Format("{0:g}", startTime);
                date_temp += ":00";
                // 获取当前周期内的频率
                double fre = Setting.Gather.ColumnList[optionIndex].Frequency;
               
                
                
                int i = 1;
                // 获取数据
                string dataPath = Setting.Gather.DataPath;
                string[] data = File.ReadAllLines(dataPath, Encoding.Default);
                // 设置曲线对象
                LineSeries ls = new LineSeries
                {
                    Title = Setting.Gather.ColumnList[optionIndex].Name,
                    Values = new ChartValues<double> {}
                };
                // 临时数据存储数组
                List<decimal> dataList = new List<decimal>();
            // 遍历源数据，并按照需校准数据进行调整
                foreach (string line in data)
                {
                    if (i >= 5)
                    {
                        // 存储当前行
                        string a = line;
                        // 设置分割字符
                        char[] sp = { ',', '"' };
                        // 存储数据型
                        string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                        if (endTime != null && (DateTime.Compare(Convert.ToDateTime(endTime), Convert.ToDateTime(datas[0])) < 0)) break;
                        if (i == 5)
                        {
                            // 数据记录开始时间有可能大于当前时间段，将时间对扩大查找至当前分钟内后几段
                            int time = 1;

                            // 如果当前时间段处于数据时间左侧则将time+1
                            while (DateTime.Compare(Convert.ToDateTime(date_temp).AddSeconds(fre * time), Convert.ToDateTime(datas[0])) < 0) time++;

                            // 进入到此处时说明时间段处于数据时间右侧，可以正确的进入循环
                            if (labels.Count > 0)
                            {
                                Labels[0] = new List<string> { Convert.ToDateTime(date_temp).AddSeconds(fre * time).ToString() };
                            }
                            else
                            {
                                Labels.Add(new List<string> { Convert.ToDateTime(date_temp).AddSeconds(fre * time).ToString() });
                            }

                        }
                        // 获取当前选择选项卡的数据模型
                        Column column = Setting.Gather.ColumnList[optionIndex];

                        int length = labels[0].Count;
                        string lastTimeStemp = labels[0][(length) - 1];
                        // 将数据中的时间取出
                        DateTime dateTime = Convert.ToDateTime(datas[0]);
                        // 当前数据中的时间与X轴的计量点中坐标轴时间较大时
                        if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStemp)) <= 0)
                        {
                            if (!decimal.TryParse(datas[optionIndex + 2], out decimal x))
                            {
                                continue;
                            }
                            // 将取得的数据存入
                            dataList.Add(x);
                            // 当两个相等时获取
                            if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStemp)) == 0)
                            {
                                // 将新的坐标轴时间加入Labels数组
                             
                                Labels[0].Add(Convert.ToDateTime(lastTimeStemp).AddSeconds(column.Frequency).ToString());
                              
                                // 获取临时数据存储数组项目数量                                
                                int count = dataList.Count;
                                // 对数据进行求和
                                decimal sum = dataList.Sum();
                                // 根据当前选项卡的设置获取正确的数据
                                if (column.Method.ToLower() == "avg")
                                {
                                    ls.Values.Add(Convert.ToDouble(sum / count));
                                }
                                else if (column.Method.ToLower() == "sum")
                                {
                                    ls.Values.Add(Convert.ToDouble(sum));
                                }
                                dataList.Clear();
                            }
                        }


                    }
                    i++;
                }
                // Y轴的轴标签显示结构
                if (YFormatter.Count > 0)
                {
                    YFormatter[0] = value => value.ToString("N");
                }
                else
                {
                    YFormatter.Add(value => value.ToString("N"));
                }
                if (SeriesCollection.Count > 0)
                {
                    SeriesCollection[0] = new SeriesCollection
                    {
                        ls
                    };
                }
                else
                {
                    seriesCollection.Add(new SeriesCollection
                    {
                        ls
                    });
                }
                if (CartesianChart.Count > 0) {
                    cartesianChart[0] = new CartesianChart
                    {
                        Series = SeriesCollection[0],
                        LegendLocation = LegendLocation.Right,
                        AxisY = new AxesCollection {
                    new Axis{
                        Title = "辐射强度,单位W/m²",
                        LabelFormatter = YFormatter[0],
                    }
                },
                        AxisX = new AxesCollection
                    {
                        new Axis
                        {
                            Title = "时间",
                            Labels = Labels[0]
                        }
                    }
                    };
                }
                else
                {
                    cartesianChart.Add(new CartesianChart
                    {
                        Series = SeriesCollection[0],
                        LegendLocation = LegendLocation.Right,
                        AxisY = new AxesCollection {
                    new Axis{
                        Title = "辐射强度,单位W/m²",
                        LabelFormatter = YFormatter[0],
                    }
                },
                        AxisX = new AxesCollection
                    {
                        new Axis
                        {
                            Title = "时间",
                            Labels = Labels[0]
                        }
                    }
                    });
                }
                
                // 将图表实例添加至ChartZone这个grid中去
                ChartZone.Children.Add(cartesianChart[0]);
                // 设定图表合并行参数
                cartesianChart[0].SetValue(Grid.RowSpanProperty, 2);
                // 设定图表合并列参数
                cartesianChart[0].SetValue(Grid.ColumnSpanProperty, 2);
            }
        }
    }
}
