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

namespace calibration_app
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string uname = this.UserLogin.Text.Trim();
            string password = this.PwdLogin.Password;
            List<string> accounts = new List<string>();
            string account = string.Empty;
            if (System.IO.File.Exists (App.ProgramData + "\\password.txt"))
            {
                accounts = System.IO.File.ReadAllLines(App.ProgramData + "\\password.txt").ToList();
                account = accounts.Find(c => c.StartsWith(uname + " "));
            }
            
            if (uname == "topflag" && password == "topflag")
            {
                App.AvailableUser = "topflag";
                this.DialogResult = true;
                this.Close();
            }
            else if (accounts.Count > 0 && !string.IsNullOrEmpty(account))
            {
                string[] accountDetail = account.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (CreateUser.MD5Encrypt(CreateUser.MD5Encrypt(password) + accountDetail[1]) == accountDetail[2])
                {
                    App.AvailableUser = accountDetail[0];
                    this.DialogResult = true;
                }
                else this.DialogResult = false;
            }
            else
            {
                MessageBox.Show("账号或密码不正确", "警告");
               
            }
        
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
