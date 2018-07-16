using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using calibration_app.SetOption;

namespace calibration_app
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        // 程序根路径
        public static string RootPath = Environment.CurrentDirectory;
        // 程序数据路径
        public static string ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TopFlagTec\\SolarCalibration";
        // 文档路径
        public static string DocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Solar Calibration";

        // 配置文件路径
        public static string SettingPath = ProgramData + "\\Setting.xml";
        // 采集指针路径
        public static string GatherPath = ProgramData + "\\Gather.txt";
        // 数据存储路径
        public static string DataStoragePath = DocumentPath + "\\DataStorage";
        // 报告路径
        public static string ReportPath = DocumentPath + "\\Report";
        
        public static string AvailableUser = string.Empty;
        public static bool IsFirst()
        {
            return !System.IO.File.Exists(SettingPath);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            LoginWindow window = new LoginWindow();
            bool? dialogResult = window.ShowDialog();
            if ((dialogResult.HasValue == true) &&
                (dialogResult.Value == true))
            {
                // 判断是否是第一次进入程序
                bool isfirst =  IsFirst();
                // 第一次启动则调起启动配置窗口
                if (isfirst)
                {
                    
                    SetupWindow setup = new SetupWindow();
                    setup.ShowDialog();
                    bool? res = setup.DialogResult;
                    if (res != true)
                    {
                        this.Shutdown();
                    }
                    else
                    {
                        base.OnStartup(e);
                        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    }
                } 
                else
                {
                    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                }
               
            }
            else
            {
                this.Shutdown();
            }
            // 初始化文件夹
            if (!System.IO.Directory.Exists(ProgramData)) System.IO.Directory.CreateDirectory(ProgramData);
            if (!System.IO.Directory.Exists(DocumentPath)) System.IO.Directory.CreateDirectory(DocumentPath);
            if (!System.IO.Directory.Exists(DataStoragePath)) System.IO.Directory.CreateDirectory(DocumentPath);
            if (!System.IO.Directory.Exists(ReportPath)) System.IO.Directory.CreateDirectory(DocumentPath);
        }
    }
}
