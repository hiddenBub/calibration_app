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

namespace calibration_app
{
    /// <summary>
    /// SetupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetupWindow : Window
    {
        public string EditingId { get; set; }
        public string FileName { get; set; }
        ObservableCollection<Column> ColumnList { get; set; }   //动态数组
        public SetupWindow()
        {
            InitializeComponent();
           
            ColumnList = new ObservableCollection<Column>();
            this.DataContext = this;
            
            GridGather.ItemsSource = ColumnList;
            GridGather.Focus();
        }
        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            
            Project project = new Project
            {
                Name = TbProName.Text,
                Lng = Convert.ToDouble(TbProLng.Text),
                Lat = Convert.ToDouble(TbProLat.Text),
            };
            
            //Column column= new Column
            //{
            //    Name = "GHI",
            //    Type = "double",
            //    Frequency = 60,
            //    Method = "avg",
            //};

           
            Gather gather = new Gather
            {
                DataPath = FileName,
                ColumnList = ColumnList,
            };
           
            Setting setting = new Setting
            {
                Gather = gather,
                Project = project
            };

            MessageBox.Show(setting.Project.Lng.ToString());
            //if (typeof(project.Parent).IsAssignableFrom(project.GetType()))
            //{
            //   
            //}
            //else
            //{
            //    MessageBox.Show("no");
            //}


            SerializeToXml<Setting>("setting.xml", setting);

            //this.DialogResult = true;
            //this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            
            if (ofd.ShowDialog() == true)
            {
                string FileName = ofd.FileName;
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
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath)) { System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T)); xs.Serialize(writer, obj); }
            }
            catch (Exception ex) { }
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

        private void GridGather_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            var uie = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter)
            {
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
                GridGather.Focus();
                
                //MessageBox.Show(ColumnList.Count().ToString());
            }
            


            //if (e.Key == Key.F5)
            //{
            //    foreach (Student tq in StuList)
            //    {
            //        FinishJincang fc = new FinishJincang();
            //        fc.RunNO = Convert.ToInt64(EditingId);//获得界面流程卡号 
            //        try
            //        {
            //            fc.InNumber = Convert.ToDouble(tq.InNumber);
            //            if (fc.InNumber == 0)
            //            {
            //                MessageBox.Show("第 " + tq.FId + " 行输入的的米数有误，请重新输入");
            //                return;
            //            }
            //        }
            //        catch
            //        {
            //            MessageBox.Show("第 " + tq.FId + " 行输入的的米数有误，请重新输入");
            //            return;
            //        }
            //        fc.Import = 1;
            //        fc.Indate = System.DateTime.Now;


            //        new FinishJincangDAL().Insert(fc);
            //    }
            //    System.Windows.Forms.MessageBox.Show("保存成功");



            //}




            if (e.Key == Key.Back)  //删除行       
            {
                Int32 Row = GridGather.Items.IndexOf(GridGather.CurrentItem);
                Int32 Col = GridGather.Columns.IndexOf(GridGather.CurrentColumn);
                for (int i = 0; i < ColumnList.Count; i++)//不用for会报错 
                {
                    foreach (Column tq in ColumnList)  //删除选中行
                    {
                        //MessageBox.Show(tq.Index.ToString());

                        ColumnList.RemoveAt(Row);
                        break;
                    }


                }


            }


        }

        private void GridGather_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = ColumnList.Count();
        }


        private void GridGather_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            // 序列号
            if (ColumnList == null || ColumnList.Count == 0)
            {
                ((Column)e.NewItem).Index = 1;
            }
            else
            {
                ((Column)e.NewItem).Index = ColumnList.Count;
            }
        }


        private void GridGather_Loaded(object sender, RoutedEventArgs e)
        {            //加载的时候就控制焦点 在第一行
            DataGridCell cell = GetCell(0, 1);
            if (cell != null)
            {
                cell.Focus();
                GridGather.BeginEdit();
            }
        }




        /// <summary>
        /// 根据行、列索引取的对应单元格对象
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);


            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);


                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    GridGather.ScrollIntoView(rowContainer, GridGather.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }


        /// <summary>
        /// 根据行索引取的行对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)GridGather.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                GridGather.ScrollIntoView(GridGather.Items[index]);
                row = (DataGridRow)GridGather.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }


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


    }
}
