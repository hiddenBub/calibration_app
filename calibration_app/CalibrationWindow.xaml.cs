using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace calibration_app
{
    /// <summary>
    /// CalibrationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationWindow : Window
    {


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

        public List<CartesianChart> CartesianChart { get => cartesianChart; set => cartesianChart = value; }
        public List<SeriesCollection> SeriesCollection { get => seriesCollection; set => seriesCollection = value; }
        public List<List<string>> Labels { get => labels; set => labels = value; }
        public List<Func<double, string>> YFormatter { get => yFormatter; set => yFormatter = value; }

        public CalibrationWindow()
        {
            InitializeComponent();
            
        }

        private void ImportCalibration_Click(object sender, RoutedEventArgs e)
        {


            bool IsGathering = MainWindow.IsGather;
            
            if (!File.Exists(MainWindow.GetFileName(MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1])))
            {
                MessageBox.Show("请先采集源数据后再导入数据", "错误");
            }
            else if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据", "错误");
            }
            else if (!IsGathering && MainWindow.GatherTimer.Count == 2 && MainWindow.GatherTimer[1] != null)
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
                        new string[] { MainWindow.Setting.Project.Name, MainWindow.Setting.Project.Lng.ToString(), MainWindow.Setting.Project.Lat.ToString()},
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
                    string fileName = MainWindow.GetFileName(MainWindow.DataType.CalibrationData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
                    // 为该文件添加表头
                    MainWindow.AddDatHeader(headerNew, fileName);

                    List<String[]> list = new List<string[]>();
                    // 生成文件写入实例，准备写入文件
                    StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Append));
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        //Console.Write(dt.Rows[i][dt.Columns[0]] + " " + dt.Rows[i][dt.Columns[1]] + " " + dt.Rows[i][dt.Columns[2]] + "\n");
                        // 初始化数据(单行)字符串
                        string str = "\"" + MainWindow.DateFormat(Convert.ToDateTime(dt.Rows[i][dt.Columns[0]]), "yyyy-MM-dd HH:mm:ss") + "\"," + i;
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

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CalibrationBtn_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSettingWindow csw = new CalibrationSettingWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CalibrationSettingWindow csw = new CalibrationSettingWindow();
            csw.ShowDialog();

            string sourceFile = MainWindow.GetFileName(MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
            string calibrationFile = MainWindow.GetFileName(MainWindow.DataType.CalibrationData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
            string[] sourceDataList = File.ReadAllLines(sourceFile, Encoding.UTF8);
            string[] calibrationDataList = File.ReadAllLines(calibrationFile, Encoding.UTF8);
            string[] sourceDataBody = GetDatBody(sourceDataList);
            string[] calibrationDataBody = GetDatBody(calibrationDataList);
            List<List<double>> CombineDataList = new List<List<double>>();
            List<List<double>> DataTemp = new List<List<double>>();
            ////////////////////////////////////////////////////////////////////////////////////////////

            // 固定取第二行的数据（此行为列名数据）
            string sStr = sourceDataList[1];
            string cStr = calibrationDataList[1];
            string[] stitle = sStr.Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] ctitle = cStr.Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] data = stitle.Skip(2).Concat(ctitle.Skip(2)).ToArray();

            int count = data.Length;

            for (int startIndex = 0; startIndex < count; startIndex++)
            {
                LineSeries ls = new LineSeries
                {
                    Title = data[startIndex],// 设置集合标题
                    Values = new ChartValues<double> { },                // 初始化数据集
                    PointGeometry = DefaultGeometries.None,             // 取消点的图形标注
                };

                if (SeriesCollection.Count > 0)
                {

                    if (SeriesCollection[0].Count > startIndex)
                    {
                        seriesCollection[0][startIndex] = ls;
                    }
                    else
                    {
                        SeriesCollection[0].Add(ls);
                    }

                }
                else
                {
                    seriesCollection.Add(new SeriesCollection { });
                    if (SeriesCollection[0].Count > startIndex)
                    {
                        seriesCollection[0][startIndex] = ls;
                    }
                    else
                    {
                        SeriesCollection[0].Add(ls);
                    }
                }
            }
            Labels = MainWindow.AddItem<List<string>>(Labels, new List<string> { });
            // Y轴的轴标签显示结构
            YFormatter = MainWindow.AddItem<Func<double, string>>(YFormatter, value => value.ToString("N"));
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
            CartesianChart = MainWindow.AddItem<CartesianChart>(CartesianChart, cartesian);



            // 将图表实例添加至ChartZone这个grid中去
            ChartZone.Children.Add(cartesianChart[0]);
            // 设定图表合并行参数
            CartesianChart[0].SetValue(Grid.RowSpanProperty, 2);


            //////////////////////////////////////////////////////////////////////////////////////////
            int i = 0;
            int cdlIndex = 0;
            // 遍历校准数据
            for (int ii = 0; ii < sourceDataBody.Length; ii++)
            {
                bool correct = true;
                char[] separator = new char[] { ',', '"' };
                string[] datas = sourceDataBody[ii].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                string[] cDatas = calibrationDataBody[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                DateTime sTime = Convert.ToDateTime(datas[0]);
                DateTime cTime = Convert.ToDateTime(cDatas[0]);
                // 当源数据时间戳小于等于校准数据时
                if (DateTime.Compare(sTime, cTime )<=0){

                    for (int di = 2; di < datas.Length; di++)
                    {
                        int scIndex = di - 2;
                        // 尝试转化字符串数据为decimal数据
                        if (!double.TryParse(datas[di], out double x))
                        {
                            // 如果同行数据插入成功但后续数据无效则删除该行数据，保证数据列数一致性
                            if (DataTemp.Count > scIndex) DataTemp.RemoveAt(scIndex);
                            // 准备跳出该次大循环
                            correct = false;
                            // 跳出循环
                            break;
                        }
                        
                        // 将取得的数据存入dataTemp
                        DataTemp = MainWindow.AddSonItem<double>(DataTemp, x, scIndex);

                    }
                    // 数据有误则跳过该次循环
                    if (!correct) continue;
                    

                    // 当两个相等时获取
                    if (DateTime.Compare(sTime, cTime) == 0)
                    {
                        // 将新的坐标轴时间加入Labels数组

                        Labels[0].Add(cDatas[0]);
                        
                        for (int index = cDatas.Length - 1; index >= 2; index--)
                        {
                            if (!double.TryParse(cDatas[index], out double x))
                            {
                                Labels.RemoveAt(Labels.Count - 1);
                                i++;
                                DataTemp.Clear();
                                correct = false;
                                break;
                            }
                            
                            // 将取得的数据存入dataTemp
                            CombineDataList = MainWindow.AddSonItem<double>(CombineDataList, x, cdlIndex);
                        }
                        if (!correct) continue;
                        for (int dli = DataTemp.Count - 1; dli >= 0; dli--)
                        {
                            // 使用均值方法获取均值
                            double avg = (double)MainWindow.GetAvg(DataTemp[dli]);

                            // 添加当前的点进入数组
                            CombineDataList = MainWindow.AddSonItem<double>(CombineDataList, avg, cdlIndex);
                        }
                        CombineDataList[CombineDataList.Count - 1].Reverse();
                        cdlIndex++;
                        i++;
                        // 清理数据缓存，准备下次数据接入
                        DataTemp.Clear();
                    }
                    
                }
                // 当源数据时间戳大于校准数据时
                else
                {
                    for (int cIndex = i;i<cDatas.Length;cIndex++)
                    {
                        string[] line = calibrationDataBody[cIndex].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        if (DateTime.Compare(sTime, Convert.ToDateTime(line[0])) <= 0)
                        {
                            i = cIndex;
                            break;
                        }
                        
                        
                    }
                }
                
            }

            for (int si = 0; si < CombineDataList.Count; si++)
            {
                for (int sik = 0; sik < CombineDataList[si].Count; sik++)
                {
                    SeriesCollection[0][sik].Values.Add( CombineDataList[si][sik]);
                }
                
            }

        }

        public string[] GetDatHeader(string[] dataList)
        {
            return dataList.Take(4).ToArray();
        }

        public string[] GetDatBody(string[] dataList)
        {
            return dataList.Skip(4).ToArray();
        }
    }
}
