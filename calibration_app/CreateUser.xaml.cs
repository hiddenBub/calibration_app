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
    /// CreateUser.xaml 的交互逻辑
    /// </summary>
    public partial class CreateUser : Window
    {
        private static char[] constant =
      {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        '!','@','#','$','%','~','^','&','*','(',')','-','_','=','+','<','>','?','`'
      };

        public CreateUser()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(App.ProgramData))
                Directory.CreateDirectory(App.ProgramData);

            if (String.IsNullOrEmpty(UserNameTB.Text.Trim()))
            {
                MessageBox.Show("用户名不能为空", "提示");
                return;
            }
            if (System.IO.File.Exists(App.ProgramData + "\\password.txt"))
            {
               string[] accounts =  File.ReadAllLines(App.ProgramData + "\\password.txt", Encoding.Default);
                string account = string.Empty;
                account = accounts.ToList().Find(c => c.StartsWith(UserNameTB.Text.Trim() + " "));
                if (!string.IsNullOrEmpty(account))
                {
                    MessageBox.Show("用户已存在，请更换用户名", "提示");
                    return;
                }
            }
            

            if (String.IsNullOrEmpty(Password.Password))
            {
                MessageBox.Show("密码不能为空", "提示");
                return;
            }
            if (String.IsNullOrEmpty(Password.Password))
            {
                MessageBox.Show("密码不一致", "提示");
                return;
            }
            
              
            StreamWriter sw = new StreamWriter(App.ProgramData + "\\password.txt", true, Encoding.Default);
            string encrypt = GetRandomStr();
            string Md5 = MD5Encrypt(MD5Encrypt(Password.Password) + encrypt);
            string line = UserNameTB.Text + " " + encrypt + " " + Md5;
            sw.WriteLine(line);
            sw.Close();
            this.DialogResult = true;
        }

        

        /// <summary>
        /// 用MD5加密字符串，可选择生成16位或者32位的加密字符串
        /// </summary>
        /// <param name="password">待加密的字符串</param>
        /// <param name="bit">位数，一般取值16 或 32</param>
        /// <returns>返回的加密后的字符串</returns>
        public static string MD5Encrypt(string password, int bit = 32)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            if (bit == 16)
                return tmp.ToString().Substring(8, 16);
            else
            if (bit == 32) return tmp.ToString();//默认情况
            else return string.Empty;
        }

        public string GetRandomStr(int Length = 6)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(constant.Length);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(constant.Length)]);
            }
            return newRandom.ToString();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
