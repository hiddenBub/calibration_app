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
using LiveCharts.Defaults;
using calibration_app.SetOption;
using System.Collections.ObjectModel;
using Microsoft.Office.Interop.Word;
using WordMLHelperUtil.Entity;
using WordMLHelperUtil;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace calibration_app
{
    /// <summary>
    /// CalibrationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationWindow : System.Windows.Window
    {
        public List<Geometry> Point = new List<Geometry>
        {
            DefaultGeometries.None,
            DefaultGeometries.Circle,
            DefaultGeometries.Square,
            DefaultGeometries.Diamond,
            DefaultGeometries.Triangle,
            DefaultGeometries.Cross,
        };
        /// <summary>
        /// 模板名称
        /// </summary>
        private string mubanFile = "muban.docx";
        
        /// <summary>
        /// 图表LIST
        /// </summary>
        private List<CartesianChart> cartesianChart = new List<CartesianChart>();

        /// <summary>
        /// 图表内数据集LIST
        /// </summary>
        private List<LiveCharts.SeriesCollection> seriesCollection = new List<LiveCharts.SeriesCollection>();

        /// <summary>
        /// 图标内标签LIST
        /// </summary>
        private List<List<string>> labels = new List<List<string>>();


        /// <summary>
        /// 图标内Y轴数据格式化LIST
        /// </summary>
        private List<Func<double, string>> yFormatter = new List<Func<double, string>>();

        private List<ColumnSetting> columnList = new List<ColumnSetting>();


        public List<CartesianChart> CartesianChart { get => cartesianChart; set => cartesianChart = value; }
        public List<LiveCharts.SeriesCollection> SeriesCollection { get => seriesCollection; set => seriesCollection = value; }
        public List<List<string>> Labels { get => labels; set => labels = value; }
        public List<Func<double, string>> YFormatter { get => yFormatter; set => yFormatter = value; }
        public List<ColumnSetting> ColumnList { get => columnList; set => columnList = value; }
        public string MubanFile { get => mubanFile; set => mubanFile = value; }

        public CalibrationWindow()
        {
            InitializeComponent();
            
        }

        private void ImportCalibration_Click(object sender, RoutedEventArgs e)
        {

            CartesianChart.Clear();
            YFormatter.Clear();
            SeriesCollection.Clear();
            Labels.Clear();
            ColumnList.Clear();
            ChartZone.Children.Clear();
            bool IsGathering = MainWindow.IsGather;
            
            if (!File.Exists(MainWindow.GetFileName(MainWindow.DataStorage,MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1])))
            {
                MessageBox.Show("请先采集源数据后再导入数据", "错误");
            }
            else if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据", "错误");
            }
            else if (!IsGathering && MainWindow.GatherTimer.Count == 2 && MainWindow.GatherTimer[1] != null)
            {
                try
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
                        System.Data.DataTable dt = ds.Tables[0];
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
                        string fileName = MainWindow.GetFileName(MainWindow.DataStorage, MainWindow.DataType.CalibrationData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
                        // 为该文件添加表头
                        MainWindow.AddDatHeader(headerNew, fileName);

                        List<String[]> list = new List<string[]>();
                        // 生成文件写入实例，准备写入文件
                        StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Append));
                        for (int dri = 0; dri < dt.Rows.Count; dri++)
                        {

                            //Console.Write(dt.Rows[i][dt.Columns[0]] + " " + dt.Rows[i][dt.Columns[1]] + " " + dt.Rows[i][dt.Columns[2]] + "\n");
                            // 初始化数据(单行)字符串
                            string str = "\"" + MainWindow.DateFormat(Convert.ToDateTime(dt.Rows[dri][dt.Columns[0]]), "yyyy-MM-dd HH:mm:ss") + "\"," + dri;
                            // 填充数据
                            for (int ii = 1; ii < dt.Columns.Count; ii++)
                            {
                                str += "," + dt.Rows[dri][dt.Columns[ii]];
                            }
                            // 添加行结束符
                            //str += Environment.NewLine;
                            sw.WriteLine(str);
                        }
                        sw.Close();

                        string sourceFile = MainWindow.GetFileName(MainWindow.DataStorage, MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
                        string calibrationFile = MainWindow.GetFileName(MainWindow.DataStorage, MainWindow.DataType.CalibrationData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
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
                                seriesCollection.Add(new LiveCharts.SeriesCollection { });
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
                    new LiveCharts.Wpf.Axis{
                        Title = "辐射强度,单位W/m²",
                        LabelFormatter = YFormatter[0],
                    }
                },
                            AxisX = new AxesCollection
                {
                    new LiveCharts.Wpf.Axis
                    {
                        Title = "时间",
                        Labels = Labels[0]
                    }
                },
                            DisableAnimations = true,
                            DataTooltip = null
                        };
                        CartesianChart = MainWindow.AddItem<CartesianChart>(CartesianChart, cartesian);



                        // 将图表实例添加至ChartZone这个grid中去
                        ChartZone.Children.Add(cartesianChart[0]);
                        // 设定图表合并行参数
                        CartesianChart[0].SetValue(Grid.RowSpanProperty, 2);


                        //////////////////////////////////////////////////////////////////////////////////////////
                        int i = 0;
                        int cdlIndex = 0;
                        // 数据收缩级别，生产环境需要置1
                        int shrink = 3;
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
                            if (DateTime.Compare(sTime, cTime) <= 0)
                            {

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
                                    DataTemp = MainWindow.AddSonItem<double>(DataTemp, Math.Round(x / shrink, 3), scIndex);

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
                                        CombineDataList = MainWindow.AddSonItem<double>(CombineDataList, Math.Round(x / shrink, 3), cdlIndex);
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
                                for (int cIndex = i; i < cDatas.Length; cIndex++)
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
                        // 将数据写入数组中
                        for (int si = 0; si < CombineDataList.Count; si++)
                        {
                            for (int sik = 0; sik < CombineDataList[si].Count; sik++)
                            {
                                SeriesCollection[0][sik].Values.Add(CombineDataList[si][sik]);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示");
                }
                

            }
        }

       
        private void CalibrationBtn_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSettingWindow csw = new CalibrationSettingWindow();
            csw.ShowDialog();
        }

       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CalibrationSettingWindow csw = new CalibrationSettingWindow();
            csw.ShowDialog();

            string sourceFile = MainWindow.GetFileName(MainWindow.DataStorage, MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
            string calibrationFile = MainWindow.GetFileName(MainWindow.DataStorage, MainWindow.DataType.CalibrationData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
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
                    seriesCollection.Add(new LiveCharts.SeriesCollection { });
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
                    new LiveCharts.Wpf.Axis{
                        Title = "辐射强度,单位W/m²",
                        LabelFormatter = YFormatter[0],
                    }
                },
                AxisX = new AxesCollection
                {
                    new LiveCharts.Wpf.Axis
                    {
                        Title = "时间",
                        Labels = Labels[0]
                    }
                },
                DisableAnimations = true,
                Hoverable = false,
                //DataTooltip = null,
            };
            CartesianChart = MainWindow.AddItem<CartesianChart>(CartesianChart, cartesian);



            // 将图表实例添加至ChartZone这个grid中去
            ChartZone.Children.Add(cartesianChart[0]);
            // 设定图表合并行参数
            CartesianChart[0].SetValue(Grid.RowSpanProperty, 2);


            //////////////////////////////////////////////////////////////////////////////////////////
            int i = 0;
            int cdlIndex = 0;
            // 数据收缩级别，生产环境需要置1
            int shrink = 1;
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
                        DataTemp = MainWindow.AddSonItem<double>(DataTemp, Math.Round(x/shrink,3), scIndex);

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
                            CombineDataList = MainWindow.AddSonItem<double>(CombineDataList, Math.Round(x/shrink,3), cdlIndex);
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
            // 将数据写入数组中
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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (CartesianChart.Count> 1)
            {
                return;
            }
            
            // 取得校准数据列的数量
            int seriesCount = MainWindow.Setting.Gather.ColumnList.Count;
            // 首先获取作为校准的数据列
            List<LiveCharts.Definitions.Series.ISeriesView> reverseSeries = SeriesCollection[0].ToList();
            // 元数据列
            List<LiveCharts.Definitions.Series.ISeriesView> SourceColletction = reverseSeries.Take(SeriesCollection[0].Count - seriesCount).ToList();
            reverseSeries.Reverse();
            // 校准数据列
            List<LiveCharts.Definitions.Series.ISeriesView> CalibrationCollection = reverseSeries.Take(seriesCount).ToList();
           
            // 将校准数据列反转为正常顺序
            CalibrationCollection.Reverse();
            double minX = 9999;double minY = 9999; double maxX = 0; double maxY = 0;

            //LineSeries ls = new LineSeries
            //{
            //    Title = "基准线",
            //    Values = new ChartValues<double>(),
            //    PointGeometry = DefaultGeometries.Triangle,             // 取消点的图形标注
               
            //};
            SeriesCollection.Add(new LiveCharts.SeriesCollection { });
            List<string> errorMessage = new List<string>();
            // 遍历数据
            for (int i = 0;i < CalibrationCollection.Count;i++ )
            {
                // 获取shadow字段
                string shadow = MainWindow.Setting.Gather.ColumnList[i].Shadow;
                LiveCharts.Definitions.Series.ISeriesView lineSeries = new LineSeries();
                
                for (int j = 0;j < SourceColletction.Count; j++)
                {
                    // 判断映射的线列并跳出循环
                    if (SourceColletction[j].Title == shadow)
                    {
                        lineSeries = SourceColletction[j];
                        break;
                    }
                }
                
                ScatterSeries ss = new ScatterSeries
                {
                    Title = CalibrationCollection[i].Title + "-" + lineSeries.Title,    // 设置集合标题
                    Values = new ChartValues<ObservablePoint> { },                      // 初始化数据集
                    PointGeometry = Point[i+1],             // 取消点的图形标注
                };
                int N1 = 0;
                // 清理点数据
                for (int index = 0; index < CalibrationCollection[i].Values.Count; index++)
                {
                    double dataX = Convert.ToDouble(lineSeries.Values[index]);
                    double dataY = Convert.ToDouble(CalibrationCollection[i].Values[index]);
                    double deviation = Math.Abs(dataX - dataY) / dataX;
                    if (dataX > 200 && dataX < 1500)
                    {
                        N1++;
                        if (deviation < 0.2)
                        {
                            // 将符合条件的点添加至
                            ss.Values.Add(new ObservablePoint(dataX, dataY));

                            // 判断是否包含该点不包含
                            //if (ls.Values.Contains(new ObservablePoint(dataX, dataY))) ls.Values.Add(new ObservablePoint(dataX, dataY));
                            minX = Math.Min(minX, dataX);
                            minY = Math.Min(minY, dataY);
                            maxX = Math.Max(maxX, dataX);
                            maxY = Math.Max(maxY, dataY);
                        }
                    }
                    else if (deviation > 0.2 || dataX > 1500 || dataX < 200) continue;
                    
                }
                if (N1 == 0 || ss.Values.Count / N1 < 0.5)
                {
                    errorMessage.Add(MainWindow.Setting.Gather.ColumnList[i].Name);
                    continue;
                }
                if (SeriesCollection[1].Count > i)
                {
                    seriesCollection[1][i] = ss;
                }
                else
                {
                    SeriesCollection[1].Add(ss);
                }

                
            }
            if (errorMessage.Count > 0)
            {
                MessageBox.Show("场站数据" + string.Join("、",errorMessage.ToArray()) + "无法校准，请重新采集", "警告");
                return;
            }

            //ls.Values.Add(minY);
            //ls.Values.Add(maxY);

            //SeriesCollection[1].Add(ls);
            CartesianChart cartesian = new CartesianChart
            {
                Series = SeriesCollection[1],
                LegendLocation = LegendLocation.Bottom,
                AxisY = new AxesCollection
                {
                    new LiveCharts.Wpf.Axis{
                        Title = "场站数据，W/m²",
                        MinValue = minY - 100,
                        MaxValue = maxY + 100,
                    }
                },
                AxisX = new AxesCollection
                {
                    new LiveCharts.Wpf.Axis
                    {
                        Title = "标准数据，W/m²",
                        MinValue = minX - 100,
                        MaxValue = maxX + 100,

                    }
                },
                Hoverable = false,
                DisableAnimations = true,
                
            };
            this.CartesianChart = MainWindow.AddItem<CartesianChart>(CartesianChart, cartesian,1);



            // 将图表实例添加至ChartZone这个grid中去
            this.ChartZone.Children.Add(cartesianChart[1]);
            // 设定图表合并行参数
            this.CartesianChart[1].SetValue(Grid.RowProperty, 0);
            this.CartesianChart[1].SetValue(Grid.ColumnProperty, 1);return;
        }

        private void CalibrateButton_Click(object sender, RoutedEventArgs e)
        {
            if (SeriesCollection.Count > 1)
            {
                if (CartesianChart.Count > 2)
                {
                    return;
                }
                ObservableCollection<SetOption.Column> columnList = MainWindow.Setting.Gather.ColumnList;
                LiveCharts.SeriesCollection calibrated = SeriesCollection[1];
                int index = 0;
                SeriesCollection.Add(new LiveCharts.SeriesCollection());
                foreach (ScatterSeries scatter in calibrated)
                {
                    // 偏差和
                    double sum = 0;
                    // 绝对偏差和
                    double absSum = 0;
                    // 灵敏度和
                    double senSum = 0;
                    ScatterSeries ss = new ScatterSeries
                    {
                        Title = scatter.Title,    // 设置集合标题
                        Values = new ChartValues<ObservablePoint> { },                      // 初始化数据集
                        PointGeometry = Point[index + 1],             // 取消点的图形标注
                    };
                    // 遍历数组求得数据和
                    for (int i = 0; i < scatter.Values.Count; i++)
                    {
                        ObservablePoint op = (ObservablePoint)scatter.Values[i];
                        // 标准数据
                        double STDv = op.X;
                        // 场站数据
                        double STTv = op.Y;

                        // 原灵敏度
                        double oldSen = columnList[index].Sensitivity;

                        sum += (STTv - STDv) / STDv;
                        absSum += Math.Abs(STTv - STDv) / STDv;
                        senSum += oldSen * STTv / STDv;
                    }
                    ColumnList.Add(new ColumnSetting {
                        Name = columnList[index].Name,
                        Frequency = columnList[index].Frequency,
                        OldSensitivity = columnList[index].Sensitivity,
                        NewSensitivity = Math.Round(senSum / scatter.Values.Count, 6),
                        OldAverageDeviation = Math.Round(sum / scatter.Values.Count, 6),
                        OldAverageAbsoluteDeviation = Math.Round(absSum / scatter.Values.Count, 6),
                    });
                    SeriesCollection[2].Add(ss);
                    index++;
                }
                for (int si = 0; si < SeriesCollection[1].Count; si++)
                {
                    // 偏差和
                    double sum = 0;
                    // 绝对偏差和
                    double absSum = 0;
                    for (int sik = 0; sik < SeriesCollection[1][si].Values.Count; sik++)
                    {
                        ObservablePoint op = (ObservablePoint)SeriesCollection[1][si].Values[sik];
                        double fix = 1 - ColumnList[si].OldAverageDeviation;
                        // 标准数据
                        double STDv = op.X;
                        // 场站数据
                        double STTv = op.Y * fix;

                        
                        
                        SeriesCollection[2][si].Values.Add(new ObservablePoint(op.X, op.Y * fix) );
                        sum += (STTv - STDv) / STDv;
                        absSum += Math.Abs(STTv - STDv) / STDv;
                    }
                    ColumnList[si].NewAverageDeviation = Math.Round(sum / SeriesCollection[1][si].Values.Count, 6);
                    ColumnList[si].NewAverageAbsoluteDeviation = Math.Round(absSum / SeriesCollection[1][si].Values.Count, 6);
                   
                }
                double minX = CartesianChart[1].AxisX[0].MinValue;
                double minY = CartesianChart[1].AxisY[0].MinValue;
                double maxX = CartesianChart[1].AxisX[0].MaxValue;
                double maxY = CartesianChart[1].AxisY[0].MaxValue;
                CartesianChart cartesian = new CartesianChart
                {
                    Series = SeriesCollection[2],
                    LegendLocation = LegendLocation.Bottom,
                    AxisY = new AxesCollection
                {
                    new LiveCharts.Wpf.Axis{
                        Title = "场站数据，W/m²",
                        MinValue = minY,
                        MaxValue = maxY,
                    }
                },
                    AxisX = new AxesCollection
                {
                    new LiveCharts.Wpf.Axis{
                        Title = "标准数据，W/m²",
                        MinValue = minX,
                        MaxValue = maxX,
                    }
                },
                    DisableAnimations = true,
                    DataTooltip = null
                };
                this.CartesianChart = MainWindow.AddItem<CartesianChart>(CartesianChart, cartesian, 2);



                // 将图表实例添加至ChartZone这个grid中去
                this.ChartZone.Children.Add(cartesianChart[2]);
                // 设定图表合并行参数
                this.CartesianChart[2].SetValue(Grid.RowProperty, 1);
                this.CartesianChart[2].SetValue(Grid.ColumnProperty, 1);
            }
            else
            {
                MessageBox.Show("数据未清理无法校准", "警告");
            }
            
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {

            
            string message = "";
            if (SeriesCollection.Count > 2)
            {
                try
                {
                    string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MubanFile);
                    WordMLHelper wordMLHelper = new WordMLHelper();
                    List<TagInfo> tagInfos = wordMLHelper.GetAllTagInfo(File.OpenRead(templatePath));
                    for (int i = 0; i < tagInfos.Count; i++)
                    {
                        int table = 1;
                        //填充域有两种类型,1:段落或图片,2:表格
                        //对填充域填充时需先判断填充域类型
                        if (tagInfos[i].Tbl == null)
                        {
                            if (string.Equals(tagInfos[i].TagTips.Trim(), "{Description}"))
                            {
                                TxtInfo txtInfo = new TxtInfo();
                                txtInfo.Content = "数据采集起始时间：" + Labels[0][0] + ",数据采集结束时间：" + Labels[0][Labels[0].Count - 1] + Environment.NewLine;
                                // 计算起止时间
                                DateTime startTime = Convert.ToDateTime(Labels[0][0]);
                                DateTime endTime = Convert.ToDateTime(Labels[0][Labels[0].Count - 1]);
                                TimeSpan span = endTime.Subtract(startTime);
                                TimeSpan pro = span;
                                string spendTime = string.Empty;
                                if (span.Days > 0)
                                {
                                    spendTime = span.Days.ToString() + "天";
                                    span -= new TimeSpan(span.Days, 0, 0, 0);
                                }
                                if (span.Hours > 0 || spendTime.Length > 0)
                                {
                                    spendTime += span.Hours.ToString() + "小时";
                                    span -= new TimeSpan(span.Hours, 0, 0);
                                }
                                if (span.Minutes > 0 || spendTime.Length > 0)
                                {
                                    spendTime += span.Minutes.ToString() + "分钟";
                                    span -= new TimeSpan(0, span.Minutes, 0);
                                }
                                if (span.Seconds > 0 || spendTime.Length > 0)
                                {
                                    spendTime += span.Seconds.ToString() + "秒";
                                }
                                txtInfo.Content += "采集共计：" + spendTime + "，约采集" + pro.TotalMinutes.ToString() + "条数据，共清洗掉"
                                    + (SeriesCollection[0][0].Values.Count - SeriesCollection[1][0].Values.Count).ToString() + "条数据";
                                txtInfo.ForeColor = "0055A3";
                                //txtInfo.HightLight = HighlightColor.Blue;
                                tagInfos[i].AddContent(txtInfo);

                            }
                            else if (string.Equals(tagInfos[i].TagTips.Trim(), "{projectName}"))
                            {
                                TxtInfo txtInfo = new TxtInfo();
                                txtInfo.Content = MainWindow.Setting.Project.Name;
                                txtInfo.ForeColor = "0055A3";
                                //txtInfo.HightLight = HighlightColor.Blue;
                                tagInfos[i].AddContent(txtInfo);
                            }

                            else if (string.Equals(tagInfos[i].TagTips.Trim(), "{LngLat}"))
                            {
                                TxtInfo txtInfo = new TxtInfo();
                                txtInfo.Content = MainWindow.Setting.Project.Lng.ToString() + "," + MainWindow.Setting.Project.Lat.ToString();
                                txtInfo.ForeColor = "0055A3";
                                //txtInfo.HightLight = HighlightColor.Blue;
                                tagInfos[i].AddContent(txtInfo);
                            }
                            else if (string.Equals(tagInfos[i].TagTips.Trim(), "{snapshoot}"))
                            {
                                //Bitmap bit = new Bitmap(this.Width, this.Height);//实例化一个和窗体一样大的bitmap
                                //Graphics g = Graphics.FromImage(bit);
                                //g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
                                //g.CopyFromScreen(this.Left, this.Top, 0, 0, new Size(this.Width, this.Height));//保存整个窗体为图片
                                //g.CopyFromScreen(panel游戏区.PointToScreen(Point.Empty), Point.Empty, panel游戏区.Size);//只保存某个控件（这里是panel游戏区）
                                //bit.Save("weiboTemp.png");//默认保存格式为PNG，保存成jpg格式质量不是很好
                                ImgInfo imgInfo = new ImgInfo();
                                //imgInfo.ImgPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory
                                //    , "./image/a1.jpg");
                                //imgInfo.Width = 200;
                                //imgInfo.Height = 200;
                                //tagInfos[i].AddContent(imgInfo);
                            }
                        }
                        else
                        {
                            TableStructureInfo tblInfo = tagInfos[i].Tbl;
                            
                            if (tagInfos[i].Seq >= 2)
                            {
                                for (int j = 0; j < ColumnList.Count; j++)
                                {
                                    RowStructureInfo row = new RowStructureInfo();

                                    for (int k = 0; k < 5; k++)
                                    {
                                       
                                        List<TxtInfo> txtInfo = new List<TxtInfo>();

                                        string content = string.Empty;
                                        switch (k)
                                        {
                                            case 0:
                                                txtInfo.Add(new TxtInfo
                                                {
                                                    Content = (j + 1).ToString(),
                                                    Size = 25,
                                                    ForeColor = "0070C0",
                                                });
                                                
                                                break;
                                            case 1:
                                                txtInfo.Add(new TxtInfo
                                                {
                                                    Content = ColumnList[j].Name,
                                                    Size = 25,
                                                    ForeColor = "0070C0",
                                                });
                                                
                                                break;
                                            case 2:
                                                if (table == 1)
                                                {
                                                    txtInfo.Add(new TxtInfo
                                                    {
                                                        Content = ColumnList[j].OldSensitivity.ToString(),
                                                        Size = 25,
                                                        ForeColor = "0070C0",
                                                    });
                                                }
                                                else
                                                {
                                                    txtInfo.Add(new TxtInfo
                                                    {
                                                        Content = ColumnList[j].NewSensitivity.ToString(),
                                                        Size = 25,
                                                        ForeColor = "0070C0",
                                                    });
                                                }
                                                
                                                
                                                break;
                                            case 3:
                                                if (table == 1)
                                                {
                                                    txtInfo.Add(new TxtInfo
                                                    {
                                                        Content = (ColumnList[j].OldAverageDeviation * 100).ToString(),
                                                        Size = 25,
                                                        ForeColor = "0070C0",
                                                    });
                                                }
                                                else
                                                {
                                                    txtInfo.Add(new TxtInfo
                                                    {
                                                        Content = (ColumnList[j].NewAverageDeviation * 100).ToString(),
                                                        Size = 25,
                                                        ForeColor = "0070C0",
                                                    });
                                                }
                                                
                                                
                                                break;
                                            case 4:
                                                if (table == 1)
                                                {
                                                    txtInfo.Add(new TxtInfo
                                                    {
                                                        Content = (ColumnList[j].OldAverageAbsoluteDeviation * 100).ToString(),
                                                        Size = 25,
                                                        ForeColor = "0070C0",
                                                    });
                                                }
                                                else
                                                {
                                                    txtInfo.Add(new TxtInfo
                                                    {
                                                        Content = (ColumnList[j].NewAverageAbsoluteDeviation * 100).ToString(),
                                                        Size = 25,
                                                        ForeColor = "0070C0",
                                                    });
                                                }
                                                break;
                                        }
                                        if (txtInfo.Count == 1)
                                        {
                                            CellStructureInfo cell = new CellStructureInfo();
                                            cell.AddContentLine(txtInfo[0]);
                                            row.AddCell(cell);
                                        }
                                        else if (txtInfo.Count > 1)
                                        {
                                            for (int ti = 0; ti < txtInfo.Count; ti++)
                                            {
                                                CellStructureInfo cell = new CellStructureInfo();
                                                cell.AddContent(txtInfo[ti]);
                                                row.AddCell(cell);
                                            }
                                            
                                            
                                        }
                                        table++;
                                        
                                    }
                                    tblInfo.AddRow(row);
                                }
                            }

                        }
                    }
                    string dir = Environment.CurrentDirectory + "\\Report\\" + MainWindow.Setting.Project.Name;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string outPutPath = dir + "\\" + MainWindow.DateFormat((DateTime)MainWindow.GatherTimer[0]) + "_" + MainWindow.DateFormat((DateTime)MainWindow.GatherTimer[1]) + ".docx";
                    if (!string.IsNullOrEmpty(outPutPath))
                    {
                        templatePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory
                           , mubanFile);
                        wordMLHelper.GenerateWordDocument(File.OpenRead(templatePath)
                            , outPutPath
                            , tagInfos);

                        Assistance.RemoveAllTmpFile();// 删除所有临时文件
                                                      //Response.Redirect(Request.Url.AbsoluteUri);
                    }
                    MessageBox.Show("报告导出成功", "完成");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                
               
            }
            else
            {
                MessageBox.Show("请先校准数据再导出报告", "错误");
            }

            }


           
        }
}
