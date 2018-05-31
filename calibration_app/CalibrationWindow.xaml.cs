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
            if (IsGathering)
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

                        Console.Write(dt.Rows[i][dt.Columns[0]] + " " + dt.Rows[i][dt.Columns[1]] + " " + dt.Rows[i][dt.Columns[2]] + "\n");
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
                    
                }

            }
            else
            {
                MessageBox.Show("请先采集源数据后再导入数据", "错误");
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
    }
}
