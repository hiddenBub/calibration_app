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
        public const string XmlPath = "./Setting.xml";
        public static bool IsFirst()
        {
            return !System.IO.File.Exists(XmlPath);
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
                    
                }
               
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}
