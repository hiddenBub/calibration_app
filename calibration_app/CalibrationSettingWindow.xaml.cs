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
        private static ObservableCollection<string> sourceCol = new ObservableCollection<string>();
        private ObservableCollection<Column> columnList = new ObservableCollection<Column>();


        public Setting Setting { get => setting; set => setting = value; }
        public static ObservableCollection<string> SourceCol { get => sourceCol; set => sourceCol = value; }
        public ObservableCollection<Column> ColumnList { get => columnList; set => columnList = value; }

        public CalibrationSettingWindow()
        {
            InitializeComponent();
            Setting = DeserializeFromXml<Setting>(@".\Setting.xml");
            
            string fileSource = MainWindow.GetFileName(MainWindow.DataType.SourceData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
            string fileCalibration = MainWindow.GetFileName(MainWindow.DataType.CalibrationData, (DateTime)MainWindow.GatherTimer[0], MainWindow.GatherTimer[1]);
            StreamReader srSource = new StreamReader(fileSource, false);
            StreamReader srCalibration = new StreamReader(fileCalibration, false);
            int length = 2;
            string[] linesS = new string[length];
            string[] linesC = new string[length];
            for (int i = 0; i < length; i++)
            {
                linesS[i] = srSource.ReadLine();
                linesC[i] = srCalibration.ReadLine();
            }
            srCalibration.Close();
            srSource.Close();
            List<string> ColS = new List<string>(linesS[1].Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> ColC = new List<string>(linesC[1].Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries));
            if (Setting.Gather.ColumnList.Count == ColC.Count - 2)
            {
                ColumnList = new ObservableCollection<Column>(Setting.Gather.ColumnList);
            }
            else
            {
                for (int i = 2; i < ColC.Count; i++)
                {
                    ColumnList.Add(new Column( i-2,ColC[i]));
                }
            }
            List<string> labs = new List<string>
            {
                "辐射①",
                "辐射②",
                "辐射③",
            };
            if (SourceCol.Count == 0)
            {
                for (int i = 2; i < ColS.Count; i++)
                {
                    sourceCol.Add(labs[i - 2]);
                }
            }
            
            
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

        /// <summary>     
        /// XML序列化某一类型到指定的文件   
        /// /// </summary>   
        /// /// <param name="filePath"></param>   
        /// /// <param name="obj"></param>  
        /// /// <param name="type"></param>   
        public static void SerializeToXml<T>(string filePath, T obj)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                    //Add an empty namespace and empty value
                    ns.Add("", ""); System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T)); xs.Serialize(writer, obj, ns);
                }
            }
            catch (Exception ex) { }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Setting.Gather.ColumnList = ColumnList;
            MainWindow.Setting.Gather.ColumnList = ColumnList;
            string settingPath = @"./Setting.xml";
            SerializeToXml(settingPath, Setting);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
