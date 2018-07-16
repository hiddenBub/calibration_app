using Microsoft.Win32;
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
using calibration_app.SetOption;
using System.Windows.Controls.Primitives;
using calibration_app.lib;

namespace calibration_app
{
    /// <summary>
    /// SetupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetupWindow : Window
    {
        public string EditingId { get; set; }
        public string FileName { get; set; }

        private ObservableCollection<Pj> projectList = new ObservableCollection<Pj>();
        public ObservableCollection<Pj> ProjectList { get => projectList; set => projectList = value; }

       
        

        /// <summary>
        ///  程序主代码区
        /// </summary>
        public SetupWindow()
        {
            InitializeComponent();
            // 获取设置实例
            Setting setting = XmlHelper.DeserializeFromXml<Setting>(App.SettingPath);
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

        /// <summary>
        /// 确认按钮执行方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TbProName.Text == "")
            {
                MessageBox.Show("项目名称不能为空");
                return;
            }
            string proName = TbProName.Text;

            
            foreach (char rInvalidChar in System.IO.Path.GetInvalidFileNameChars())
            {
                if (proName.Contains(rInvalidChar.ToString())) {
                    MessageBox.Show("项目名称包含特殊字符：" + rInvalidChar.ToString());
                    return;
                }
            }


            if (TbProLng.Text == "" || TbProLat.Text == "")
            {
                MessageBox.Show("项目地理位置信息不能为空");
                return;
            }

            if (!double.TryParse(TbProLat.Text, out double lat) || !double.TryParse(TbProLng.Text, out double lng))
            {
                MessageBox.Show("地理位置数据类型不正确");
                return;
            }

            if (FileTextBox.Text == "")
            {
                MessageBox.Show("必须设置采集源文件路径");
                return;
            }

            string strFileName = FileTextBox.Text;

            
            
            // 判断文件夹存在与否
            if (!Directory.Exists(strFileName))
            {
                MessageBox.Show("输入的路径不是一个文件夹", "错误");
                return;
            }
            // 文件夹存在则在文件夹下查找SEC级别的数据文件
            else
            {
                
                    string[] files = Directory.GetFiles(strFileName, "*.dat");
                    foreach (string dat in files)
                    {
                        string file = System.IO.Path.GetFileName(dat);
                        if (file == "CR1000XSeries_GHI_SEC.dat")
                        {
                            FileName = dat;
                            break;
                        }

                    }
                    if (FileName == null)
                    {
                        MessageBox.Show("该文件夹下无可用的dat数据文件", "错误");
                        return;
                    }
                
                
                
            }
            // 初始化文件夹
            if (!System.IO.Directory.Exists(App.ProgramData)) System.IO.Directory.CreateDirectory(App.ProgramData);
            if (!System.IO.Directory.Exists(App.DocumentPath)) System.IO.Directory.CreateDirectory(App.DocumentPath);
            if (!System.IO.Directory.Exists(App.DataStoragePath)) System.IO.Directory.CreateDirectory(App.DocumentPath);
            if (!System.IO.Directory.Exists(App.ReportPath)) System.IO.Directory.CreateDirectory(App.DocumentPath);
            // 如果数据文件夹下不存在项目文件夹则新建
            if (!Directory.Exists(App.DataStoragePath + "\\" + proName))
                Directory.CreateDirectory(App.DataStoragePath + "\\" + proName);
            // 如果报告文件夹下不存在项目文件夹则新建
            if (!Directory.Exists(App.ReportPath + "\\" + proName))
                Directory.CreateDirectory(App.ReportPath + "\\" + proName);
            Setting setting = new Setting
            {
                Gather = new Gather
                {
                    DataPath = FileName,
                },
                Project = new Project
                {
                    Name = proName,
                    Lng = lng,
                    Lat = lat,
                    SelectedIndex = 0,
                    PjList = new ObservableCollection<Pj>
                    {
                        new Pj
                        {
                            Name = proName,
                            Lat = lat,
                            Lng = lng,
                            Pid  = 0,
                        }
                    }
                }
            };

            if (!Directory.Exists(App.ProgramData)) Directory.CreateDirectory(App.ProgramData);

            // 将设置保存到setting.xml文件
            
            XmlHelper.SerializeToXml<Setting>(App.SettingPath, setting);

            // 操作完成后设置返回数据并关闭窗口
            this.DialogResult = true;
            
        }

        /// <summary>
        /// 取消按钮程序代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            
        }

        /// <summary>
        /// 窗口关闭代码段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            this.DialogResult = false;
            
        }



        //private void ControlDat_Click(object sender, RoutedEventArgs e)
        //{
        //    FileStream fs = new FileStream("D:\\Program Files\\loggerNet\\datasource\\CR1000XSeries_GHI_SEC.dat", FileMode.Open, FileAccess.Read, FileShare.None, 1, true);
        //    StreamReader sr = new StreamReader(fs);
        //    int i = 1;
        //    while (sr.ReadLine() != "")
        //    {
        //        int range = 5;
        //        string[] temp = new string[range];
        //        string line = sr.ReadLine();
        //        // 第五行开始是数据行
        //        if (i >= 5)
        //        {

        //            int index = i % range;
        //            // 定义数据容量即dat文件中每行作为索引的数据长度
        //            int indexSize = 2;
        //            char[] delimiterChars = { ',' };
        //            string[] tempArr = line.Split(delimiterChars,);

        //        }
        //    }

        //}

            /// <summary>
            /// 选择数据文件按钮程序代码段
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            
            dialog.Description = "请选择Dat数据文件所在的文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                string[] files = Directory.GetFiles(dialog.SelectedPath, "*.dat");
                foreach (string dat in files)
                {
                    string file = System.IO.Path.GetFileName(dat);
                    if ( file == "CR1000XSeries_GHI_SEC.dat")
                    {
                        FileTextBox.Text = dialog.SelectedPath;
                        return;
                    }
                }
                if (FileName == null)
                {
                    MessageBox.Show("该文件夹下无可用的dat数据文件","提示");
                    return;
                }
                FileTextBox.Text = dialog.SelectedPath;
                //this.LoadingText = "处理中...";
                //this.LoadingDisplay = true;
                //Action<string> a = DaoRuData;
                //a.BeginInvoke(dialog.SelectedPath, asyncCallback, a);
            }
            
        }



        

        //private void GridGather_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
            
        //    var uie = e.OriginalSource as UIElement;
        //    if (e.Key == Key.Enter)
        //    {
        //        uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        //        e.Handled = true;
        //        //GridGather.Focus();
                
        //        //MessageBox.Show(ColumnList.Count().ToString());
        //    }
            


        //    //if (e.Key == Key.F5)
        //    //{
        //    //    foreach (Student tq in StuList)
        //    //    {
        //    //        FinishJincang fc = new FinishJincang();
        //    //        fc.RunNO = Convert.ToInt64(EditingId);//获得界面流程卡号 
        //    //        try
        //    //        {
        //    //            fc.InNumber = Convert.ToDouble(tq.InNumber);
        //    //            if (fc.InNumber == 0)
        //    //            {
        //    //                MessageBox.Show("第 " + tq.FId + " 行输入的的米数有误，请重新输入");
        //    //                return;
        //    //            }
        //    //        }
        //    //        catch
        //    //        {
        //    //            MessageBox.Show("第 " + tq.FId + " 行输入的的米数有误，请重新输入");
        //    //            return;
        //    //        }
        //    //        fc.Import = 1;
        //    //        fc.Indate = System.DateTime.Now;


        //    //        new FinishJincangDAL().Insert(fc);
        //    //    }
        //    //    System.Windows.Forms.MessageBox.Show("保存成功");



        //    //}




        //    if (e.Key == Key.Back)  //删除行       
        //    {
        //        //Int32 Row = GridGather.Items.IndexOf(GridGather.CurrentItem);
        //        //Int32 Col = GridGather.Columns.IndexOf(GridGather.CurrentColumn);
        //        int i = 0;
        //        int j = 0;
        //        foreach (Column tq in ColumnList)  //删除选中行
        //        {
        //                //MessageBox.Show(tq.Index.ToString());
        //            if (i == Row)
        //            {
        //                j = 0;
        //                foreach (System.Reflection.PropertyInfo p in tq.GetType().GetProperties())
        //                {
        //                    if (j == Col)
        //                    {
                                
        //                        if (p.Name != "Index")
        //                        {
        //                            p.SetValue(tq, null);
        //                            uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        //                        }
                                
        //                    }
        //                    j++;
        //                }
        //            }
                        
        //            i++;
        //                //ColumnList.RemoveAt(Row);
        //                //break;
        //        }


                


        //    }


        //}

        //private void GridGather_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    e.Row.Header = ColumnList.Count();
        //}


        //private void GridGather_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        //{
        //    // 序列号
        //    if (ColumnList == null || ColumnList.Count == 0)
        //    {
        //        ((Column)e.NewItem).Index = 1;
        //    }
        //    else
        //    {
        //        ((Column)e.NewItem).Index = ColumnList.Count;
        //    }
        //}


        //private void GridGather_Loaded(object sender, RoutedEventArgs e)
        //{            //加载的时候就控制焦点 在第一行
        //    DataGridCell cell = GetCell(0, 1);
        //    if (cell != null)
        //    {
        //        cell.Focus();
        //        //GridGather.BeginEdit();
        //    }
        //}




        /// <summary>
        /// 根据行、列索引取的对应单元格对象
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        //public DataGridCell GetCell(int row, int column)
        //{
        //    DataGridRow rowContainer = GetRow(row);


        //    if (rowContainer != null)
        //    {
        //        DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);


        //        // try to get the cell but it may possibly be virtualized
        //        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        //        if (cell == null)
        //        {
        //            // now try to bring into view and retreive the cell
        //            //GridGather.ScrollIntoView(rowContainer, GridGather.Columns[column]);
        //            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        //        }
        //        return cell;
        //    }
        //    return null;
        //}


        /// <summary>
        /// 根据行索引取的行对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        //public DataGridRow GetRow(int index)
        //{
        //    DataGridRow row = (DataGridRow)GridGather.ItemContainerGenerator.ContainerFromIndex(index);
        //    if (row == null)
        //    {
        //        // may be virtualized, bring into view and try again
        //        GridGather.ScrollIntoView(GridGather.Items[index]);
        //        row = (DataGridRow)GridGather.ItemContainerGenerator.ContainerFromIndex(index);
        //    }
        //    return row;
        //}


        /// <summary>
        /// 获取指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void FileTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // 获取文件夹位置
            string directory = FileTextBox.Text;
            if (!Directory.Exists(directory))
            {
                MessageBox.Show("输入的路径不是一个文件夹", "错误");
            }
            
        }
    }
}
