using System;
using System.Collections.Generic;
using System.Collections;
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
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using LibUsbDotNet.Descriptors;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.WinUsb;
using calibration_app.SetOption;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace calibration_app
{
    
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public string FileName { get; set; }
        ObservableCollection<Column> ColumnList { get; set; }   //动态数组
        ObservableCollection<Pj> ProjectList { get; set; }
        /// <summary>
        /// 系统设置
        /// </summary>
        private Setting setting;
        public SettingWindow()
        {
            InitializeComponent();
            Setting = lib.XmlHelper.DeserializeFromXml<Setting>(App.SettingPath);
            TbProName.Text = Setting.Project.Name;
            TbProLng.Text = Setting.Project.Lng.ToString();
            TbProLat.Text = Setting.Project.Lat.ToString();
            FileName = Setting.Gather.DataPath;
            ColumnList = new ObservableCollection<Column>(Setting.Gather.ColumnList);
            this.DataContext = this;
            // 获取设置实例
            Setting setting = lib.XmlHelper.DeserializeFromXml<Setting>(App.SettingPath);
            if (setting != null) ProjectList = setting.Project.PjList;

            if (ProjectList.Count > 0)
            {
                ProjectCB.ItemsSource = setting.Project.PjList;
                ProjectCB.SelectedValuePath = "Pid";
                ProjectCB.DisplayMemberPath = "Name";
                ProjectCB.SelectedIndex = 0;
            }
            else
            {
                Existed.Children.Clear();
                TextBlock tb = new TextBlock
                {
                    Text = "当前还没有项目，请您创建新的项目以启动",
                    Foreground = new SolidColorBrush(Colors.Gray),

                };
                Existed.Children.Add(tb);
            }

            this.DataContext = this;
            // 为单选框添加选中事件
            NewProject.Checked += new RoutedEventHandler(Radio_Checked);
            ExistedProject.Checked += new RoutedEventHandler(Radio_Checked);
            //GridGather.ItemsSource = ColumnList;
        }

        void Radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn == null)
                return;
            if (btn.Name == "NewProject")
            {
                New.Visibility = Visibility.Visible;
                Existed.Visibility = Visibility.Collapsed;
            }
            else if (btn.Name == "ExistedProject")
            {
                Existed.Visibility = Visibility.Visible;
                New.Visibility = Visibility.Collapsed;
            }
        }

        private void FileTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // 获取文件夹位置
            string directory = FileTextBox.Text;
            if (!System.IO.Directory.Exists(directory))
            {
                MessageBox.Show("输入的路径不是一个文件夹", "错误");
            }

        }

        public Setting Setting { get => setting; set => setting = value; }


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
        /// 选择数据文件按钮程序代码段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string dataPath = Setting.Gather.DataPath;
            // 设置截取长度
            int length = dataPath.LastIndexOf('\\');
            // 从文件路径字符串中截取路径字符串
            string folder = dataPath.Substring(0, length);
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "数据文件|*.dat",
                InitialDirectory = folder,
            };
            if (ofd.ShowDialog() == true)
            {
                FileName = ofd.FileName;
            }
        }

        public void Apply()
        {
            if (TbProName.Text == "")
            {
                MessageBox.Show("项目名称不能为空");
                return;
            }

            if (TbProLng.Text == "" || TbProLat.Text == "")
            {
                MessageBox.Show("项目地理位置信息不能为空");
                return;
            }

            if (FileName == null)
            {
                MessageBox.Show("必须设置采集源文件路径");
                return;
            }

            //if (ColumnList[0].Name == null || ColumnList[0].Sensitivity == 0 || ColumnList[0].Frequency == 0)
            //{
            //    MessageBox.Show("请设置需校准的数据");
            //    return;
            //}
            // 项目设置
            Project project = new Project
            {
                Name = TbProName.Text,
                Lng = Convert.ToDouble(TbProLng.Text),
                Lat = Convert.ToDouble(TbProLat.Text),
            };

            // 采集设置
            Gather gather = new Gather
            {
                DataPath = FileName,
                //ColumnList = ColumnList,
            };

            // 设置总结点
            Setting setting = new Setting
            {
                Gather = gather,
                Project = project
            };


            // 将设置保存到setting.xml文件
            SerializeToXml<Setting>("./Setting.xml", setting);
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
            Apply();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        /// <summary>     
        /// 从某一XML文件反序列化到某一类型   
        /// </summary>    
        /// <param name="filePath">待反序列化的XML文件名称</param>  
        /// <param name="type">反序列化出的</param>  
        /// <returns></returns>    
        //public string[] DataType { get => dataType; }

        //public string[] DataName { get => dataName; set => dataName = value; }
    }


}
