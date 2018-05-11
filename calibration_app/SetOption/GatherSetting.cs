using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace calibration_app.SetOption
{
    public class GatherSetting : Setting
    {
        // 类内字段
        private string dataPath;

        private List<ColumnSetting> ColumnList = new List<ColumnSetting>();

        public string DataPath { get => dataPath; set => dataPath = value; }





        /// <summary>
        /// 确定项目的构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="frequency"></param>
        /// <param name="method"></param>
        //public GatherSetting(string name,string type,int frequency,string method)
        //{
        //    Name = name;
        //    Type = type;
        //    Frequency = frequency;
        //    Method = method;
        //}
        //public GatherSetting(string name, string type, int frequency)
        //{
        //    Name = name;
        //    Type = type;
        //    Frequency = frequency;
        //    Method = "sum";
        //}
        //public GatherSetting(string name, string type)
        //{
        //    Name = name;
        //    Type = type;
        //    Frequency = 60;
        //    Method = "avg";
        //}
        //public GatherSetting(string name)
        //{
        //    Name = name;
        //    Type = "double";
        //    Frequency = 60;
        //    Method = "avg";
        //}

        public GatherSetting()
        {
            DataPath = "";
            ColumnList.Add(new ColumnSetting());
        }


        #region 属性更改通知
        public event PropertyChangedEventHandler PropertyChanged;
        private void Changed(string PropertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion

    }
}
