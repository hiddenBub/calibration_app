﻿using System;
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

        private List<DateTime> gatherTimer = new List<DateTime>();

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

        private List<List<decimal>> dataTemp = new List<List<decimal>>();

        private List<string[]> dataSource = new List<string[]>();
        
        public enum DataType
        {
            SourceData = 0,
            CalibrationData = 1,
        };

        private string dataStorage = "./DataStorage";

        private string dateTimeFormat = "yyyy-MM-dd hh:mm:ss";

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
        public string DataStorage { get => dataStorage; set => dataStorage = value; }
        public string DateTimeFormat { get => dateTimeFormat; set => dateTimeFormat = value; }
        public List<DateTime> GatherTimer { get => gatherTimer; set => gatherTimer = value; }
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
            //DateTime now = DateTime.Now;
            //string dateTimeFormat = "yyyy-MM-dd hh:mm:ss";
            //string timeStamp = now.ToString(dateTimeFormat) + "\r\n";
            if (!state)
            {
               
                // 将开始采集时间点写入到文件做记录
                DateTime now = new DateTime(2018,05,09,21,16,39);
                string timeStamp = now.ToString(DateTimeFormat);


                // 清空计时器
                GatherTimer.Clear();
                // 计时器写入开始时间
                GatherTimer.Add(now);


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
                    "\"" + string.Join("\",\"", new string[] { "TIMESTAMP","RECORD","GHI_80A","GHI_80B","GHI_80C" }) + "\"",
                    "\"" + string.Join("\",\"", new string[] {"TS","RN","W/m2","W/m2","W/m2" }) + "\"",
                    "\"" + string.Join("\",\"", new string[] {"","","Avg", "Avg", "Avg" }) + "\"",
                };
                // 创建数据文件并写入文件头
                AddDatHeader(header, fileName);

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
                DateTime now = DateTime.Now;
                string timeStamp = now.ToString(DateTimeFormat);
                GatherTimer.Add(now);
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(timeStamp);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                spendTime = 0;
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
            
            string filePath = "./Gather.txt";
            

            if (File.Exists(filePath))
            {
                // 读取gather.txt文件获取起始和结束采集时间
                StreamReader sr = new StreamReader(filePath, Encoding.Default);

                // 读取第一行
                string first = sr.ReadLine();
                // 将数据存至采集计时器中
                GatherTimer.Add(Convert.ToDateTime(first));
                string second = sr.ReadLine();
                // 结束时间
                if (second != null) GatherTimer.Add(Convert.ToDateTime(second));
                // 关闭streamreader
                sr.Close();
            }

            // 初始化chartZone的Grid控件
            AddCell(2, 2);

            GetChart();
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            string dataPath = Setting.Gather.DataPath;
            //FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
            //fs.Seek(0, SeekOrigin.End);
            //fs.Seek(-1, SeekOrigin.Current);
            //int fsLen = (int)fs.Length;
            //byte[] heByte = new byte[fsLen];
            //int r = fs.Read(heByte, 0, heByte.Length);
            //string str = Encoding.UTF8.GetString(heByte);

            StreamReader sr = new StreamReader(dataPath, false);
            StringBuilder str = new StringBuilder();
            long start_time = GetTickCount();
            while (sr.ReadLine() != null)
            {
                str.Append(sr.ReadLine());
            }
            
            //sr.BaseStream.Seek(0, SeekOrigin.End);
            //ulong length = (ulong)sr.BaseStream.Length;
            //string str = string.Empty;
            //for (ulong i = 0;i< length;i++)
            //{
            //    sr.BaseStream.Seek(-1, SeekOrigin.Current);
            //    str += sr.Read();
            //    char[] array = str.ToCharArray();
            //    IEnumerable<char> cs = array.Reverse<char>();
            //    char[] array1 = cs.ToArray<char>();
            //    str = new string(array1);
            //    if (str.IndexOf("\t") == 0)
            //    {
            //        MessageBox.Show(str);
            //        break;
            //    }
            //}
            
           
            MessageBox.Show(str);
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
           
            // 获取当前周期内的起始时间
            string date_temp = string.Format("{0:g}", GatherTimer[0]);
            date_temp += ":00";
            // 获取当前周期内的频率
            double fre = Setting.Gather.ColumnList[optionIndex].Frequency;
            int i = 1;
            // 获取数据
            string dataPath = Setting.Gather.DataPath;
            //StreamReader sr = new StreamReader(dataPath, false);
            //sr.BaseStream.Seek(0, SeekOrigin.End);
            //string str = streamReader.ReadToEnd();
            //FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
            //fs.Seek(0, SeekOrigin.End);
            //fs.Seek(-1, SeekOrigin.Current);
            //int fsLen = (int)fs.Length;
            //byte[] heByte = new byte[fsLen];
            //int r = fs.Read(heByte, 0, heByte.Length);
            //string myStr = Encoding.UTF8.GetString(heByte);

            string data = ReadLastData(dataPath);
            //foreach (string line in data)
            //{
            //    bool correct = true;
            //    if (i >= 5)
            //    {
            //        // 存储当前行
            //        string a = line;
            //        // 设置分割字符
            //        char[] sp = { ',', '"' };
            //        // 存储数据型
            //        string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            //        if (GatherTimer[1] != null && (DateTime.Compare(Convert.ToDateTime(GatherTimer[1]), Convert.ToDateTime(datas[0])) < 0)) break;
                    
            //        // 获取当前选择选项卡的数据模型
            //        Column column = Setting.Gather.ColumnList[optionIndex];

            //        int length = labels[0].Count;
            //        string lastTimeStamp = labels[0][(length) - 1];
            //        // 将数据中的时间取出
            //        DateTime dateTime = Convert.ToDateTime(datas[0]);
            //        // 当前数据中的时间与X轴的计量点中坐标轴时间较大时
            //        if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStamp)) <= 0)
            //        {
            //            // 当两个相等时获取
            //            if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStamp)) == 0)
            //            {
            //                // 将新的坐标轴时间加入Labels数组

            //                Labels[0].Add(Convert.ToDateTime(lastTimeStamp).AddSeconds(column.Frequency).ToString());

            //                dataTemp.Clear();
            //            }
            //            for (int di = 2; di < datas.Length; di++)
            //            {
            //                if (!decimal.TryParse(datas[di], out decimal x))
            //                {
            //                    correct = false;
            //                    continue;
            //                }
            //                int scIndex = di - 2;

            //                // 将取得的数据存入
            //                if (dataTemp.Count > scIndex)
            //                {
            //                    dataTemp[scIndex].Add(x);
            //                }
            //                else
            //                {
            //                    dataTemp.Add(new List<decimal> { x });
            //                }


            //            }
            //            // 继续跳出循环
            //            if (!correct) continue;
            //            // 获取临时数据存储数组项目数量                                
            //            int count = dataTemp.Count;
            //            for (int dli = 0; dli < count; dli++)
            //            {
            //                int dcount = dataTemp[dli].Count;
            //                // 对数据进行求和
            //                decimal sum = dataTemp[dli].Sum();
            //                if (SeriesCollection[0][dli].Values.Count > 0 && dataTemp[dli].Count > 1)
            //                {
            //                    SeriesCollection[0][dli].Values.RemoveAt(SeriesCollection[0][dli].Values.Count - 1);
            //                }
            //                SeriesCollection[0][dli].Values.Add(Convert.ToDouble(sum / dcount));


            //            }
                        
            //            //// 根据当前选项卡的设置获取正确的数据
            //            //if (column.Method.ToLower() == "avg")
            //            //{
            //            //    if (SeriesCollection[0][0].Values.Count > 0 && dataTemp.Count > 1)
            //            //    {
            //            //        SeriesCollection[0][0].Values.RemoveAt(SeriesCollection[0][0].Values.Count - 1);
            //            //    }
            //            //    SeriesCollection[0][0].Values.Add(Convert.ToDouble(sum / count));
            //            //}
            //            //else if (column.Method.ToLower() == "sum")
            //            //{
            //            //    if(SeriesCollection[0][0].Values.Count > 0 && dataTemp.Count > 1)
            //            //    {
            //            //        SeriesCollection[0][0].Values.RemoveAt(SeriesCollection[0][0].Values.Count - 1);
            //            //    }
            //            //    SeriesCollection[0][0].Values.Add(Convert.ToDouble(sum));
            //            //}
                        
                        
            //        }
                    
            //    }
            //    i++;
            //}
            spendTime++;
        }
        #region 自定义函数开始

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public string GetFileName (DataType type,DateTime startTime ,DateTime? endTime)
        {
            // 生成一个空字符串
            string result = string.Empty;
            // 依据数据类型生成文件名
            switch (type)
            {
                case DataType.SourceData:
                    result = Setting.Project.Name + "_" + DataType.SourceData.ToString() + "_" + startTime.ToString();
                    break;
                case DataType.CalibrationData:
                    result = Setting.Project.Name + "_" + DataType.CalibrationData.ToString() + "_" + startTime.ToString();
                    break;
            }
            // 判断是否传入了结束时间
            if (endTime != null) result += "_" + endTime.ToString();
            //result += extension;
            // 返回文件名
            return result;
        }

        /// <summary>
        /// 为dat文件添加文件头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="fileName"></param>
        public void AddDatHeader (List<string> header,string fileName)
        {
            // 布置文件以及文件所在目录
            fileName = dataStorage + @"/" + fileName + ".dat";
            // 要生成的文件不存在
            if (!File.Exists(fileName))
            {
                // 创建写入对象
                StreamWriter sw = new StreamWriter(fileName, false);
                // 将文件头中所有数据写入文件
                foreach (string str in header)
                {
                    // 写入一整行
                    sw.WriteLine(str);
                }
                // 关闭文件
                sw.Close();
            }
            
            // filename = "北控项目_SourceData_2018-5-23 10:39:21_2018-5-24 17:00:00.dat"
        }

        public bool IsDatHeader (string[] line)
        {
            
            bool result = false;
            return result;
        }

        //public List<string[]> GetDatHeader ()
        //{

        //}

        public string[] ReadAllDatas (string filePath)
        {
            // 读取所有行数据
            return File.ReadAllLines(filePath, Encoding.Default);
        }

        /// <summary>
        /// 返回最后一行数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string ReadLastData (string filePath)
        {
            string[] lines = ReadAllDatas(filePath);
            return lines[lines.Length -1];
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
        public void SaveAllData(string filePath ,char[] delimiter)
        {
            string[] data = ReadAllDatas(filePath);
            foreach (string line in data)
            {
                string[] column =  line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
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

        public List<T> AddItem<T> (List<T> variable, T item)
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
        #endregion 自定义函数结束

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
                
                // 临时数据存储数组
                List<List<decimal>> dataList = new List<List<decimal>>();
            // 遍历源数据，并按照需校准数据进行调整
                foreach (string line in data)
                {
                    // 存储当前行
                    string a = line;
                    // 设置分割字符
                    char[] sp = { ',', '"' };
                    // 存储数据型
                    string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    // 数据正确判断依据
                    bool correct = true;
                    if (i == 2)
                    {
                        for (int di = 2; di < datas.Length; di++)
                        {
                            int count = di - 1;
                            int index = di - 2;
                            LineSeries ls = new LineSeries
                            {
                                Title = datas[di],// 设置集合标题
                                Values = new ChartValues<double> { },                // 初始化数据集
                                PointGeometry = DefaultGeometries.None,             // 取消点的图形标注
                            };
                            if (SeriesCollection.Count > 0)
                            {
                                
                                if (SeriesCollection[0].Count >= count)
                                {
                                    seriesCollection[0][index] = ls;
                                }
                                else
                                {
                                    SeriesCollection[0].Add(ls);
                                }
                                    
                            }
                            else
                            {
                                seriesCollection.Add(new SeriesCollection { });
                                if (SeriesCollection[0].Count >= di)
                                {
                                    seriesCollection[0][index] = ls;
                                }
                                else
                                {
                                    SeriesCollection[0].Add(ls);
                                }
                            }
                        }
                        
                        
                        
                    }
                    else if (i == 5)
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
                    else if (i > 5)
                    {
                        
                        
                        // 如果时间数据越界中断循环
                        if (endTime != null && (DateTime.Compare(Convert.ToDateTime(endTime), Convert.ToDateTime(datas[0])) < 0)) break;
                        // 获取当前选择选项卡的数据模型
                        Column column = Setting.Gather.ColumnList[optionIndex];

                        int length = labels[0].Count;
                        string lastTimeStamp = labels[0][(length) - 1];
                        // 将数据中的时间取出
                        DateTime dateTime = Convert.ToDateTime(datas[0]);
                        
                        // 当前数据中的时间与X轴的计量点中坐标轴时间较大时
                        if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStamp)) <= 0)
                        {
                            // 判断当前点是否是个正确的数据
                            for (int di = 2;di < datas.Length; di++)
                            {
                                if (!decimal.TryParse(datas[di], out decimal x))
                                {
                                    correct = false;
                                    continue;
                                }
                                int scIndex = di - 2;
                                
                                // 将取得的数据存入
                                if (dataList.Count > scIndex)
                                {
                                    dataList[scIndex].Add(x);
                                }
                                else
                                {
                                    dataList.Add(new List<decimal> { x });
                                }
                                
                                
                            }
                            // 当两个相等时获取
                            if (DateTime.Compare(dateTime, Convert.ToDateTime(lastTimeStamp)) == 0)
                            {
                                // 将新的坐标轴时间加入Labels数组

                                Labels[0].Add(Convert.ToDateTime(lastTimeStamp).AddSeconds(column.Frequency).ToString());

                                // 获取临时数据存储数组项目数量                                
                                int count = dataList.Count;
                                for (int dli = 0; dli < count; dli++)
                                {
                                    int dcount = dataList[dli].Count;
                                    // 对数据进行求和
                                    decimal sum = dataList[dli].Sum();

                                    SeriesCollection[0][dli].Values.Add(Convert.ToDouble(sum / dcount));
                                    
                                }
                                
                                
                                dataList.Clear();
                            }
                            if (!correct) continue;
                            
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
