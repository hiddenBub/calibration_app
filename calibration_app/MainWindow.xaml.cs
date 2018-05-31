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

        private static bool isGather = false;

        /// <summary>
        /// 表格容器根据所选选项卡布置表格
        /// </summary>
        private Grid ChartZone = new Grid
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,

        };

        private static List<DateTime?> gatherTimer = new List<DateTime?> {  new DateTime(), new DateTime() };

        bool first = true;


        public static bool IsGather
        {
            get => isGather;
            set
            {
                if (value != false && value != true)
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

        private List<List<decimal>> DataTemp = new List<List<decimal>>();

        private List<string[]> dataSource = new List<string[]>();

        public enum DataType
        {
            SourceData = 0,
            CalibrationData = 1,
        };

        private static string dataStorage = Environment.CurrentDirectory.ToString() + "\\DataStorage";

        private string dateTimeFormat = "yyyy-MM-dd hh：mm：ss";

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string strConn = "";

        /// <summary>
        /// 系统设置
        /// </summary>
        private static Setting setting;

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
        public static Setting Setting { get => setting; set => setting = value; }
        public static string DataStorage { get => dataStorage; set => dataStorage = value; }
        public string DateTimeFormat { get => dateTimeFormat; set => dateTimeFormat = value; }
        public static List<DateTime?> GatherTimer { get => gatherTimer; set => gatherTimer = value; }
        public List<string[]> DataSource { get => dataSource; set => dataSource = value; }

        public MainWindow()
        {


            InitializeComponent();

        }


        private void ImportSource_Click(object sender, RoutedEventArgs e)
        {
            bool IsGathering = isGather;
            if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据", "错误");
            }

        }

        private void ImportCalibration_Click(object sender, RoutedEventArgs e)
        {
            bool IsGathering = isGather;
            if (!File.Exists(GetFileName(DataType.SourceData, (DateTime)GatherTimer[0], GatherTimer[1])))
            {
                MessageBox.Show("请先采集源数据后再导入数据", "错误");
            }
            else if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据", "错误");
            }
            else if (!IsGathering && GatherTimer.Count == 2 && GatherTimer[1] != null)
            {
                //打开一个文件选择框
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Excel文件",
                    FileName = "",
                    Filter = "Excel文件(*.xls)|*"
                };
                ofd.ShowDialog();
                // 获取文件路径
                string filePath = ofd.FileName;
                // 如果选择文件不为空
                if (filePath != "" && filePath != null)
                {
                    // 配置连接字符串
                    string strConn = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + filePath + "; Extended Properties = 'Excel 12.0 Xml; HDR = No'";
                    // 创建新的数据集
                    DataSet ds = new DataSet();
                    // 获取数据
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [Sheet1$]", strConn);
                    // 将数据填充至数据集中
                    oada.Fill(ds);
                    // 存储列表
                    DataTable dt = ds.Tables[0];
                    // 存储表头长度+1即与dat文件存储格式相同
                    int length = dt.Columns.Count + 1;
                    //"\"" + string.Join("\",\"", new string[] { Setting.Project.Name, Setting.Project.Lng.ToString(), Setting.Project.Lat.ToString() }) + "\"",
                    //"\"" + string.Join("\",\"", new string[] { "TIMESTAMP", "RECORD", "GHI_80A", "GHI_80B", "GHI_80C" }) + "\"",
                    //"\"" + string.Join("\",\"", new string[] { "TS", "RN", "W/m2", "W/m2", "W/m2" }) + "\"",
                    //"\"" + string.Join("\",\"", new string[] { "", "", "Avg", "Avg", "Avg" }) + "\"",
                    // 初始化dat文件头
                    List<string[]> header = new List<string[]>
                    {
                        new string[] { Setting.Project.Name,Setting.Project.Lng.ToString(), Setting.Project.Lat.ToString()},
                        new string[length],
                        new string[length],
                        new string[length],
                    };
                    // 填充字段之前的数据
                    header[1][0] = "TIMESTAMP"; header[1][1] = "RECORD";
                    header[2][0] = "TS"; header[2][1] = "RN";
                    header[3][0] = ""; header[3][1] = "";
                    // 填充字段
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        header[1][j + 1] = dt.Columns[j].ToString();
                        header[2][j + 1] = "W/m2";
                        header[3][j + 1] = "Avg";
                    }
                    // 创建以行为区分的数据列表
                    List<string> headerNew = new List<string>();
                    // 循环遍历原列表并将字符串数据存至新列表
                    for (int k = 0; k < header.Count; k++)
                    {
                        headerNew.Add("\"" + string.Join("\",\"", header[k]) + "\"");
                    }
                    // 生成校准数据文件名
                    string fileName = GetFileName(DataType.CalibrationData, (DateTime)GatherTimer[0], GatherTimer[1]);
                    // 为该文件添加表头
                    AddDatHeader(headerNew, fileName);

                    List<String[]> list = new List<string[]>();
                    // 生成文件写入实例，准备写入文件
                    StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Append));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        Console.Write(dt.Rows[i][dt.Columns[0]] + " " + dt.Rows[i][dt.Columns[1]] + " " + dt.Rows[i][dt.Columns[2]] + "\n");
                        // 初始化数据(单行)字符串
                        string str = "\"" + DateFormat(Convert.ToDateTime(dt.Rows[i][dt.Columns[0]]), "yyyy-MM-dd HH:mm:ss") + "\"," + i;
                        // 填充数据
                        for (int ii = 1; ii < dt.Columns.Count; ii++)
                        {
                            str += "," + dt.Rows[i][dt.Columns[ii]];
                        }
                        // 添加行结束符
                        //str += Environment.NewLine;
                        sw.WriteLine(str);
                    }
                    sw.Close();
                    MessageBox.Show("导入完成");
                    CalibrationWindow window = new CalibrationWindow
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };
                    window.ShowDialog();
                }

            }
            
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

        private void USBBtn_Click(object sender, RoutedEventArgs e)
        {

        }





        /// <summary>
        /// 切换采集开关
        /// </summary>
        /// <param name="state">当前是否在采集中</param>
        public void SwitchGather(bool state)
        {
            DateTime now = DateTime.Now;
            string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            string timeStamp = DateFormat(now, dateTimeFormat);
            if (!state)
            {

                // 将开始采集时间点写入到文件做记录
                //DateTime now = new DateTime(2018, 05, 09, 21, 16, 39);
                //string timeStamp = DateFormat(now,"yyyy-MM-dd HH:mm:ss");


                
                // 计时器写入开始时间
                GatherTimer[0] = now;


                StreamWriter sw = new StreamWriter(@".\Gather.txt", false);
                //开始写入
                sw.WriteLine(timeStamp);
                //清空缓冲区、关闭流
                sw.Flush();
                sw.Close();


                // 变更采集按钮显示
                GatherCB.Content = GatherMenu.Header = "结束采集";


                // 判断数据文件存储文件夹是否存在不存在创建文件夹
                if (!Directory.Exists(DataStorage)) Directory.CreateDirectory(DataStorage);


                // 设置文件名、
                string fileName = GetFileName(DataType.SourceData, now, null);
                List<string> header = new List<string>
                {
                    "\"" + string.Join("\",\"", new string[] { Setting.Project.Name,Setting.Project.Lng.ToString(), Setting.Project.Lat.ToString()}) + "\"",
                    "\"" + string.Join("\",\"", new string[] { "TIMESTAMP","RECORD","GHI_80A","GHI_80B","GHI_80C" }) + "\"",
                    "\"" + string.Join("\",\"", new string[] {"TS","RN","W/m2","W/m2","W/m2" }) + "\"",
                    "\"" + string.Join("\",\"", new string[] {"","","Avg", "Avg", "Avg" }) + "\"",
                };
                // 创建数据文件并写入文件头
                AddDatHeader(header, fileName);

                // 重新获取图表
                InitChart(header);

                //定时查询-定时器
                //if (dispatcherTimer.Tick == null)
                //{
                    dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                //}
                
                dispatcherTimer.Start();
            }
            else
            {
                // 结束采集时将时间记录并写入文件
                //DateTime now = DateTime.Now;
                //string timeStamp = DateFormat(now, "yyyy-MM-dd HH:mm:ss");
                GatherTimer[1] = now;
                FileStream fs = new FileStream(@".\Gather.txt", FileMode.Append);
               
                //获得字节数组
                byte[] data = Encoding.Default.GetBytes(timeStamp);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                // 获取数据文件名
                string oldFileName = GetFileName(DataType.SourceData, (DateTime)GatherTimer[0], null);
                string newFileName = GetFileName(DataType.SourceData, (DateTime)GatherTimer[0], GatherTimer[1]);
                File.Move(oldFileName, newFileName);
                // 更新按钮显示
                GatherCB.Content = GatherMenu.Header = "开始采集";
                // 关闭计时器
                dispatcherTimer.Stop();
            }
            // 判断依据取反
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

            string filePath = "./Gather.txt";


            if (File.Exists(filePath))
            {
                // 读取gather.txt文件获取起始和结束采集时间
                StreamReader sr = new StreamReader(filePath, Encoding.UTF8);

                // 读取第一行
                string first = sr.ReadLine();
                // 将数据存至采集计时器中
                GatherTimer[0] = Convert.ToDateTime(first);
                string second = sr.ReadLine();
                // 结束时间
                if (second != null)
                {
                    GatherTimer[1] = Convert.ToDateTime(second);

                }
                else
                {
                    GatherTimer[1] = null;
                }
                // 关闭streamreader
                sr.Close();
                // 初始化chartZone的Grid控件
                AddCell(2, 2);

                GetChart();
            }
            else
            {
                for( int i = 0;i < GatherTimer.Count;i++)
                {
                    if (GatherTimer[i] == new DateTime()) GatherTimer[i] = null;
                }
            }
            

            
        }

        

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime beforDT = DateTime.Now;
            //string dataPath = setting.Gather.DataPath;
            //FileStream sr = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
            //StringBuilder s = new StringBuilder();
            //long pos = sr.Length;
            //pos = InverseReadRow(sr, pos,ref s);

            
            
            DateTime time = ChartStartFormat(DateTime.Now, 5);
            DateTime afterDT = DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);
            MessageBox.Show("DateTime总共花费"+ ts.TotalMilliseconds + "ms.");

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

            // 获取数据
            string dataPath = Setting.Gather.DataPath;
            FileStream fs = new FileStream(dataPath, FileMode.Open,FileAccess.Read);
           
            StringBuilder sb = new StringBuilder();
            long newPos = InverseReadRow(fs, fs.Length,ref sb);
            fs.Close();
            // 存储当前行
            string a = sb.ToString();
            // 设置分割字符
            char[] sp = { ',', '"' ,'\r','\n'};
            // 存储数据型
            string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            string[] split = datas[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string date = split[0];
            //DateTime earlist = Convert.ToDateTime(date + " 05:00:00");
            //DateTime latest = Convert.ToDateTime(date + " 19:00:00");
            //if (DateTime.Compare(Convert.ToDateTime(datas[0]), earlist) < 0 || DateTime.Compare(Convert.ToDateTime(datas[0]), latest) > 0) return;
            //if (GatherTimer.Count == 2 && (DateTime.Compare(Convert.ToDateTime(GatherTimer[1]), Convert.ToDateTime(datas[0])) < 0)) return;

            //// 获取当前选择选项卡的数据模型
            //Column column = Setting.Gather.ColumnList[optionIndex];
            // 取得X轴中最后一个时间戳
            string lastTimeStamp = labels[0][(labels[0].Count) - 1];
            // 将数据中的时间取出
            DateTime dateTime = Convert.ToDateTime(datas[0]);
            // 当前数据中的时间与X轴的计量点中坐标轴时间较大时
            if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStamp)) >= 0)
            {
                for (int di = 2; di < datas.Length; di++)
                {
                    if (!decimal.TryParse(datas[di], out decimal x))
                    {
                        return;
                    }
                    int scIndex = di - 2;

                    // 将取得的数据存入dataTemp
                    DataTemp = AddSonItem<Decimal>(DataTemp, x, scIndex);

                }
                // 获取临时数据存储数组项目数量                                
                int count = DataTemp.Count;
                double[] avg = new double[datas.Length - 2];
                for (int dli = 0; dli < count; dli++)
                {
                    // 使用均值方法获取均值
                    avg[dli] = GetAvg(DataTemp[dli]);
                    // 数据列中当前列的数量大于0 并且dataTemp中对应数据多于1的时候将数据列中的该点移除
                    if (SeriesCollection[0][dli].Values.Count > 0 && DataTemp[dli].Count > 1)
                    {
                        SeriesCollection[0][dli].Values.RemoveAt(SeriesCollection[0][dli].Values.Count - 1);
                    }
                    // 添加当前的点进入数组
                    SeriesCollection[0][dli].Values.Add(avg[dli]);
                }
                
                // 当两个相等时获取
                if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStamp)) == 0)
                {
                    // 将新的坐标轴时间加入Labels数组

                    Labels[0].Add(Convert.ToDateTime(lastTimeStamp).AddSeconds(60).ToString());
                    
                    // 行数据数组
                    string[] column = new string[datas.Length];
                    // 写数据
                    column[0] = "\"" + datas[0] + "\"";
                    // 数据编号
                    column[1] = spendTime.ToString();
                    // 遍历数据列
                    for (int key = 0; key < avg.Length; key++)
                    {
                        column[key + 2] = avg[key].ToString();
                    }
                    // 将数据数组接合为字符串
                    string line = string.Join(",", column) + Environment.NewLine;
                    // 将字符串转换为byte型数据
                    byte[] by = Encoding.Default.GetBytes(line);
                    // 获取当前需要操作的文件名
                    string fn = GetFileName(DataType.SourceData, (DateTime)GatherTimer[0], null);
                    // 以追加写方式打开文件流
                    FileStream fileStream = new FileStream(fn, FileMode.Append, FileAccess.Write);
                    // 写数据
                    fileStream.Write(by, 0, by.Length);
                    // 关闭文件流
                    fileStream.Close();
                    spendTime++;
                    // 清理数据缓存，准备下次数据接入
                    DataTemp.Clear();
                }

            }
            fs.Close();
        }
        #region 自定义函数开始

        /// <summary>
        /// 格式化时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DateFormat (DateTime time,string format = "yyyy-MM-dd HH：mm：ss")
        {
            return time.ToString(format);
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static string GetFileName(DataType type, DateTime startTime, DateTime? endTime)
        {
            // 生成字符串
            string result = Setting.Project.Name;
            // 依据数据类型生成文件名
            switch (type)
            {
                case DataType.SourceData:
                    result += "_" + DataType.SourceData.ToString();
                    break;
                case DataType.CalibrationData:
                    result += "_" + DataType.CalibrationData.ToString();
                    break;
            }
            result += "_" + DateFormat(startTime);
            // 判断是否传入了结束时间
            if (endTime != null) result += "_" + DateFormat((DateTime)endTime);
            result += ".dat";
            result = DataStorage + "\\" + result;
            // 返回文件名
            return result;
        }

        /// <summary>
        /// 为dat文件添加文件头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="fileName"></param>
        public static void AddDatHeader(List<string> header, string fileName)
        {
            
            // 要生成的文件不存在
            if (!File.Exists(fileName))
            {
                // 创建写入对象
                StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Append));
                // 将文件头中所有数据写入文件
                foreach (string str in header)
                {
                    // 写入一整行
                    sw.WriteLine(str);
                }
                sw.Flush();
                // 关闭文件
                sw.Close();
            }

            // filename = "北控项目_SourceData_2018-5-23 10:39:21_2018-5-24 17:00:00.dat"
        }

        public bool IsDatHeader(string[] line)
        {

            bool result = false;
            return result;
        }

        //public List<string[]> GetDatHeader ()
        //{

        //}

        public List<string> ReadAllDatas(string filePath)
        {
            List<string> datas = new List<string>();
            // 判断文件是否存在
            if (File.Exists(filePath))
            {
                // 创建数据对象
                StreamReader sr = new StreamReader(filePath, false);
                // 如果有数据则读取一行加入数组
                while (sr.ReadLine() != null)
                {
                    datas.Add(sr.ReadLine());
                }
                sr.Close();
            }

            return datas;
        }

        /// <summary>  
        /// 从后向前按行读取文本文件，最多读取 10kb  
        /// </summary>  
        /// <param name="fs"></param>  
        /// <param name="position"></param>  
        /// <param name="s"></param>  
        /// <param name="maxRead">默认每次最多读取10kb数据</param>  
        /// <returns>返回读取位置</returns>  
        long InverseReadRow(FileStream fs
            , long position
            , ref StringBuilder s
            , int maxRead = 10240)
        {
            byte n = 0xD;//回车符  
            byte r = 0xA;//换行符  
            if (fs.Length == 0)
                return 0;
            var newPos = position - 1;
            int curVal = 0;
            var readLength = 0;
            List<byte> arr = new List<byte>();
            var str = "";
            //bool brk = false;  
            do
            {
                readLength++;
                if (newPos <= 0)
                    newPos = 0;

                fs.Position = newPos;
                curVal = fs.ReadByte();
                if (curVal == -1)
                    break;

                arr.Insert(0, (byte)curVal);

                if (newPos <= 0)
                    break;
                //  
                if (readLength == maxRead)
                    break;


                if (curVal != n)
                    newPos--;

            } while (curVal != n);
            str = Encoding.UTF8.GetString(arr.ToArray());
            s.Insert(0, str);
            string st = s.ToString();
            if (st == "\r\n")
            {
               newPos = InverseReadRow(fs, newPos, ref s, maxRead);
            }
            return newPos;
        }

        /// <summary>
        /// 求指定数组中的所有值的平均值
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public double GetAvg(List<decimal> arr)
        {
            int count = arr.Count;
            decimal sum = arr.Sum();
            return Convert.ToDouble(sum / count);
        }
        //public string[] SaveData ()

        /// <summary>
        /// 将数据存储到字段中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="delimiter"></param>
        public void SaveAllData(string filePath, char[] delimiter)
        {
            List<string> data = ReadAllDatas(filePath);
            foreach (string line in data)
            {
                string[] column = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                DataSource.Add(column);
            }
        }

        /// <summary>
        /// 将指定类型的数据添加至数组中，
        /// </summary>
        /// <typeparam name="T">数组中指定的类型</typeparam>
        /// <param name="variable">数组变量</param>
        /// <param name="item">数组中需要更改的数据</param>
        /// /// <param name="index">数组中需要更改的索引</param>
        /// <returns></returns>
        public List<T> AddItem<T>(List<T> variable, T item, int index)
        {
            /*判断数组的长度*/
            // 如果数组长度大于需要更改的索引值，则说明该索引存在
            if (variable.Count > index)
            {
                // 更改该项目
                variable[index] = item;
            }
            // 小于则说明该索引不存在
            else
            {
                // 添加该索引至数组
                variable.Add(item);
            }
            // 将数组返回
            return variable;
        }

        public List<T> AddItem<T>(List<T> variable, T item)
        {
            /*判断数组的长度*/
            // 如果数组长度大于需要更改的索引值，则说明该索引存在
            if (variable.Count > 0)
            {
                // 更改该项目
                variable[0] = item;
            }
            // 小于则说明该索引不存在
            else
            {
                // 添加该索引至数组
                variable.Add(item);
            }
            // 将数组返回
            return variable;
        }

        public List<List<T>> AddSonItem<T> (List<List<T>> variable,T sonItem ,int index)
        {
            /*判断数组的长度*/
            // 如果数组长度大于需要更改的索引值，则说明该索引存在
            if (variable.Count > index)
            {
                // 更改该项目
                variable[index].Add(sonItem);
            }
            // 小于则说明该索引不存在
            else
            {
                // 添加该索引至数组
                variable.Add(new List<T> { sonItem });
            }
            // 将数组返回
            return variable;
        }
       


        /// <summary>
        /// 为表格ChartZone填充行列
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void AddCell(int row, int col)
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
        #endregion 自定义函数结束

        private void ClearChartElement()
        {
            // 将chartzone内所有子元素清空
            ChartZone.Children.Clear();
            // 重置曲线
            SeriesCollection.Clear();
            // 充值图表
            CartesianChart.Clear();
            // 重置Y轴文本格式
            YFormatter.Clear();
            // 重置X轴标签
            Labels.Clear();
        }

        private void InitChart(List<string> datasList,bool isAddLabel = true)
        {
            ClearChartElement();

            // 固定取第二行的数据（此行为列名数据）
            string title = datasList[1];
            string[] data = title.Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries);

            int count = data.Length;

            for (int startIndex = 2; startIndex < count; startIndex++)
            {
                LineSeries ls = new LineSeries
                {
                    Title = data[startIndex],// 设置集合标题
                    Values = new ChartValues<double> { },                // 初始化数据集
                    PointGeometry = DefaultGeometries.None,             // 取消点的图形标注
                };

                if (SeriesCollection.Count > 0)
                {

                    if (SeriesCollection[0].Count > startIndex - 2)
                    {
                        seriesCollection[0][startIndex - 2] = ls;
                    }
                    else
                    {
                        SeriesCollection[0].Add(ls);
                    }

                }
                else
                {
                    seriesCollection.Add(new SeriesCollection { });
                    if (SeriesCollection[0].Count > startIndex - 2)
                    {
                        seriesCollection[0][startIndex - 2] = ls;
                    }
                    else
                    {
                        SeriesCollection[0].Add(ls);
                    }
                }
            }
            Labels = AddItem<List<string>>(Labels, new List<string> {  });
            // 判断是否需要初始化标签
            if (isAddLabel)
            {
                Labels = AddSonItem<string>(Labels, ChartStartFormat((DateTime)GatherTimer[0], 60).ToString(), 0);
            }
            // Y轴的轴标签显示结构
            YFormatter = AddItem<Func<double, string>>(YFormatter, value => value.ToString("N"));
            CartesianChart cartesian = new CartesianChart
            {
                Series = SeriesCollection[0],
                LegendLocation = LegendLocation.Right,
                AxisY = new AxesCollection
                {
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
            CartesianChart = AddItem<CartesianChart>(CartesianChart, cartesian);

            

            // 将图表实例添加至ChartZone这个grid中去
            ChartZone.Children.Add(cartesianChart[0]);
            // 设定图表合并行参数
            CartesianChart[0].SetValue(Grid.RowSpanProperty, 2);
            // 设定图表合并列参数
            CartesianChart[0].SetValue(Grid.ColumnSpanProperty, 2);

        }

        private DateTime ChartStartFormat(DateTime startTime, int frequency)
        {
            // 起始时间
            DateTime result = new DateTime();
            if (frequency <= 60 && 60 % frequency == 0)
            {
                // 获取当前周期内的起始时间
                string time_temp = string.Format("{0:g}", startTime);
                time_temp += ":00";
                // 设置初始时间
                DateTime timestamp = Convert.ToDateTime(time_temp);
                // 数据记录开始时间有可能大于当前时间段，将时间对扩大查找至当前分钟内后几段
                int times = 1;
                // 如果当前时间段处于数据时间左侧则将time+1
                while (DateTime.Compare(timestamp.AddSeconds(frequency * times), Convert.ToDateTime(startTime)) < 0) times++;
                // 结果
                result = timestamp.AddSeconds(frequency * times);
            }
            // 采集频率 大于1分钟 但 小于1小时 同时能 整除60 并 被3600整除
            else if (frequency > 60 && frequency <= 60 * 60
                && 3600 % frequency == 0 && frequency % 60 == 0)
            {
                // 获取当前周期内的起始时间
                string time_temp = string.Format("{0:yyyy-MM-dd HH}", startTime);
                time_temp += ":00:00";
                // 设置初始时间
                DateTime timestamp = Convert.ToDateTime(time_temp);
                // 数据记录开始时间有可能大于当前时间段，将时间对扩大查找至当前分钟内后几段
                int times = 1;
                // 如果当前时间段处于数据时间左侧则将time+1
                while (DateTime.Compare(timestamp.AddSeconds(frequency * times), Convert.ToDateTime(startTime)) < 0) times++;
                // 结果
                result = timestamp.AddSeconds(frequency * times);
            }
            // 采集频率 大于1小时 但 小于24小时 同时能 整除60、60*60 并 被24*60*60整除
            else if ( frequency > 60 * 60 && frequency <= 24 * 60 * 60  
                && (24 * 60 * 60) % frequency == 0 && frequency % 60 * 60 == 0 && frequency % 60 == 0)
            {
                // 获取当前周期内的起始时间
                string time_temp = string.Format("{0:yyyy-MM-dd HH}", startTime);
                time_temp += ":00:00";
                // 设置初始时间
                DateTime timestamp = Convert.ToDateTime(time_temp);
                // 数据记录开始时间有可能大于当前时间段，将时间对扩大查找至当前分钟内后几段
                int times = 1;
                // 如果当前时间段处于数据时间左侧则将time+1
                while (DateTime.Compare(timestamp.AddSeconds(frequency * times), Convert.ToDateTime(startTime)) < 0) times++;
                // 结果
                result = timestamp.AddSeconds(frequency * times);
            }
           
            return result;
        }

        private void GetChart()
        {
            
            string filepath = GetFileName(DataType.CalibrationData,(DateTime)GatherTimer[0],GatherTimer[1]);

            // 起始时间
            DateTime startTime = (DateTime)GatherTimer[0];
            // 结束时间
            DateTime? endTime = GatherTimer[1];

            // 获取当前周期内的起始时间
            string date_temp = string.Format("{0:g}", startTime);
            date_temp += ":00";
            // 获取当前周期内的频率
            double fre = Setting.Gather.ColumnList[optionIndex].Frequency;
            
            // 获取数据
            string dataPath = GetFileName(DataType.SourceData, startTime, endTime);
            if (!File.Exists(dataPath))  return; 
            string[] dataList = File.ReadAllLines(dataPath, Encoding.Default);
            // 设置曲线对象
            string[] dataHeader = dataList.Take(5).ToArray();
            string[] dataBody = dataList.Skip(4).ToArray();
            List<string> header = new List<string>(dataHeader);
            InitChart(header,false);
            // 临时数据存储数组
            List<List<decimal>> dataTemp = new List<List<decimal>>();
            // 遍历源数据，并按照需校准数据进行调整
            foreach (string line in dataBody)
            {
                // 存储当前行
                string a = line;
                // 设置分割字符
                char[] sp = { ',', '"' };
                // 存储数据型
                string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                // 数据正确判断依据
                bool correct = true;
                string date = datas[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                DateTime earlist = Convert.ToDateTime(date + " 05:00:00");
                DateTime latest = Convert.ToDateTime(date + " 19:00:00");
                if (DateTime.Compare(Convert.ToDateTime(datas[0]), earlist) < 0 || DateTime.Compare(Convert.ToDateTime(datas[0]), latest) > 0) continue;
                // 如果时间数据越界中断循环
                if (endTime != null && (DateTime.Compare(Convert.ToDateTime(endTime), Convert.ToDateTime(datas[0])) < 0)) break;
                //// 获取当前选择选项卡的数据模型
                //Column column = Setting.Gather.ColumnList[optionIndex];

                //int length = labels[0].Count;
                //string lastTimeStamp = labels[0][(length) - 1];
                //// 将数据中的时间取出
                //DateTime dateTime = Convert.ToDateTime(datas[0]);

                // 当前数据中的时间与X轴的计量点中坐标轴时间较大时

                // 判断当前点是否是个正确的数据
                
                // 获取临时数据存储数组项目数量                                
                int count = datas.Length;
                for (int dli = 2; dli < count; dli++)
                {
                    if (!decimal.TryParse(datas[dli], out decimal x))
                    {
                        correct = false;
                        break;
                    }
                    
                    SeriesCollection[0][dli -2].Values.Add(Convert.ToDouble(datas[dli]));

                }

                if (!correct) continue;
                // 将新的坐标轴时间加入Labels数组

                Labels[0].Add(datas[0]);
                
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsGather)
            {
                MessageBoxResult a = MessageBox.Show("程序正在采集中，是否结束采集", "警告", MessageBoxButton.OKCancel);
                if (a == MessageBoxResult.OK)
                {
                    SwitchGather(isGather);
                }
               
            }
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
