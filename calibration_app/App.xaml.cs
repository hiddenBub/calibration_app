using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using calibration_app.SetOption;
using IWshRuntimeLibrary;

namespace calibration_app
{
    /*
        需更新的项目：
            1.  拉取场站数据后有可能出现多出“的问题导致无法正常拉取数据
            2.  不需严格定义文件名(标准站分钟数据等)
            3.  无法校准时无法继续执行后续代码，所有数据列均无法校准时，直接导出报告文件，有任何数据列可以正常校准时继续执行，并在后续报告中标注数据无法校准
                同时做出数据对比表格
            4.  第一次打开软件后将软件设置快捷方式 （完成）（待测试）
            5.  添加帮助说明 (完成）（待测试）
            6.  加入gear以使数据过多时不会卡死界面
         */
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        #region 系统变量
        // 项目名称
        /// <summary>
        /// 项目名称
        /// </summary>
        public static string ProjectName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;

        /// <summary>
        /// 程序根路径
        /// </summary>
        public static string RootPath = Environment.CurrentDirectory;

        /// <summary>
        /// 程序数据存储路径
        /// </summary>
        public static string ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TopFlagTec\\" + ProjectName;

        /// <summary>
        /// 我的文档存储路径
        /// </summary>
        public static string DocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TopFlagTec\\" + ProjectName;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string SettingPath = ProgramData + "\\Setting.xml";
        /// <summary>
        /// 采集标识文件路径
        /// </summary>
        public static string GatherPath = ProgramData + "\\Gather.txt";

        /// <summary>
        /// 数据存储路径
        /// </summary>
        public static string DataStoragePath = DocumentPath + "\\DataStorage";

        /// <summary>
        /// 报告存储路径
        /// </summary>
        public static string ReportPath = DocumentPath + "\\Report";
        
        public static string AvailableUser = string.Empty;
        #endregion

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
                    /* 设置快捷方式 */
                    var desktop = lib.Shortcut.GetDeskDir();


                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    WshShell shell = new WshShell();

                    string shotcutName = "旗云测光站校准系统.lnk";
                    string shortcutAddress = Path.Combine(desktopPath, shotcutName);
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                    shortcut.Description = "旗云测光站校准系统";
                    //shortcut.Hotkey = "Ctrl+Shift+N";
                    shortcut.TargetPath = RootPath + "\\" + ProjectName + ".exe";

                    /* 初始判断逻辑 */
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
                // 初始化文件夹
                if (!Directory.Exists(ProgramData)) Directory.CreateDirectory(ProgramData);
                if (!Directory.Exists(DocumentPath)) Directory.CreateDirectory(DocumentPath);
                if (!Directory.Exists(DataStoragePath)) Directory.CreateDirectory(DataStoragePath);
                if (!Directory.Exists(ReportPath)) Directory.CreateDirectory(ReportPath);
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}
