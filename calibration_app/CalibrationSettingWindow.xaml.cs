using calibration_app.SetOption;
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

namespace calibration_app
{
    /// <summary>
    /// CalibrationSetting.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrationSettingWindow : Window
    {
        private Setting setting;
        private static List<string> sourceCol;
        public ObservableCollection<Column> ColumnList { get; set; }   //动态数组

        public Setting Setting { get => setting; set => setting = value; }
        public static List<string> SourceCol { get => sourceCol; set => sourceCol = value; }

        public CalibrationSettingWindow()
        {
            InitializeComponent();
            Setting = DeserializeFromXml<Setting>(@".\Setting.xml");
            ColumnList = new ObservableCollection<Column>(Setting.Gather.ColumnList);
            string fileName = MainWindow.GetFileName(MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
            StreamReader sr = new StreamReader(fileName,false);
            string[] lines = new string[4];
            for (int i = 0; i < 4; i++)
            {
                lines[i] = sr.ReadLine();
            }
            sourceCol = new List<string>(lines[1].Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries));
            GridGather.ItemsSource = ColumnList;
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

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
