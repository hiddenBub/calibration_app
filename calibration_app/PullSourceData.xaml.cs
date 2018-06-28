using System;
using System.Collections.Generic;
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

namespace calibration_app
{
    /// <summary>
    /// PullSourceData.xaml 的交互逻辑
    /// </summary>
    public partial class PullSourceData : Window
    {
        private DateTime start = new DateTime();
        private DateTime end = new DateTime();
        private string[] dataList;

        public DateTime Start { get => start; set => start = value; }
        public DateTime End { get => end; set => end = value; }
        public string[] DataList { get => dataList; set => dataList = value; }

        public PullSourceData()
        {
            InitializeComponent();
            try
            {
                string dataFile = System.IO.Path.GetDirectoryName(MainWindow.Setting.Gather.DataPath) + "\\CR1000XSeries_GHI_Min.dat";
                if (File.Exists(dataFile))
                {
                    //StartTime.TextInput += new TextCompositionEventHandler(StartChange);
                    //StartTime.PreviewTextInput += new TextCompositionEventHandler(StartChange);

                    DataList = File.ReadAllLines(dataFile, Encoding.UTF8);
                    string[] datBody = CalibrationWindow.GetDatBody(DataList);
                    string[] firstLine = datBody[0].Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    Start = Convert.ToDateTime(firstLine[0]);
                    string[] lastLine = datBody[datBody.Length - 1].Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    End = Convert.ToDateTime(lastLine[0]);
                    StartTime.Value = Start;
                    
                    EndTime.Value = End;
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(App.GatherPath))
                {
                    File.Delete(App.GatherPath);
                }
                StreamWriter sw = new StreamWriter(App.GatherPath, true);
                if (StartTime.Value != null)
                {
                    MainWindow.GatherTimer[0] = StartTime.Value;
                    sw.WriteLine(StartTime.Value.ToString());
                    if (EndTime.Value != null)
                    {
                        MainWindow.GatherTimer[1] = EndTime.Value;
                        sw.WriteLine(EndTime.Value.ToString());
                    }
                }
                sw.Close();
                string filename = MainWindow.GetFileName(App.DataStoragePath + "\\" + MainWindow.Setting.Project.Name, MainWindow.DataType.SourceData, (DateTime)StartTime.Value, EndTime.Value);
                string path = System.IO.Path.GetDirectoryName(filename);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (File.Exists(filename))
                {
                    File.SetAttributes(filename, FileAttributes.Normal);//file为要删除的文件
                    File.Delete(filename);
                }
                

                List<string> datHeader = CalibrationWindow.GetDatHeader(DataList).ToList();
                datHeader[0] = "\"" + MainWindow.Setting.Project.Name + "\",\"" + MainWindow.Setting.Project.Lng + "\",\"" + MainWindow.Setting.Project.Lat + "\"";
                datHeader[1] = "\"TIMESTAMP\",\"RECORD\",\"辐射①\",\"辐射②\",\"辐射③\"";
                MainWindow.AddDatHeader(datHeader, filename);
                string[] datBody = CalibrationWindow.GetDatBody(DataList);
                StreamWriter ss = new StreamWriter(filename, true, Encoding.UTF8);
                for (int i = 0; i < datBody.Length; i++)
                {
                    string[] datas = datBody[i].Split(new char[] { ',', '"' }, StringSplitOptions.RemoveEmptyEntries);
                    // 分割日期及时间
                    string[] split = datas[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    // 存储日期
                    string date = split[0];
                    // 设置最早的时间
                    DateTime earlist = Convert.ToDateTime(date + " 05:00:00");
                    // 设置最晚的时间
                    DateTime latest = Convert.ToDateTime(date + " 19:00:00");
                    // 获取记录时间
                    DateTime recordTime = Convert.ToDateTime(datas[0]);
                    // 比较时间
                    if (DateTime.Compare(recordTime, earlist) < 0 ||            // 数据时间早于最早时间
                        DateTime.Compare(recordTime, latest) > 0 ||             // 数据时间晚于最晚时间
                        Convert.ToDouble(datas[2]) > 1500 ||                    // 辐射1大于1500
                        Convert.ToDouble(datas[3]) > 1500 ||                    // 辐射2大于1500
                        Convert.ToDouble(datas[4]) > 1500)                      // 辐射3大于1500
                        continue;                                               // 跳过获取
                    // 获取数据写入至文件
                    if (recordTime.CompareTo(StartTime.Value) > 0 && recordTime.CompareTo(EndTime.Value) < 0)
                    {
                        ss.WriteLine(datBody[i]);
                    }
                }
                ss.Close();
                this.DialogResult = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
