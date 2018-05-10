using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace calibration_app
{
    public class GatherSetting : Setting
    {
        private string name;
        public string Name { get => name; set => name = value; }


        private static readonly List<string> dataType = new List<string> { "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal" };
        public static List<string> DataType
        {
            get => dataType;
        }

        private string type;
        public string Type
        {
            get => type;
            set
            {
                if (DataType.Contains(value))
                {
                    type = value.ToString();
                }
            } 
        }


        private int frequency;
        public int Frequency
        { get => frequency;
            set
            {
                if (value > 0)
                {
                    frequency = value;
                }
            }
        }

        private static readonly List<string> calculateMethod = new List<string> { "sum", "avg", "max", "min" };
        public static List<string> CalculateMethod => calculateMethod;


        private string method;
        public string Method
        {
            get => method;
            set
            {
                if (CalculateMethod.Contains(value))
                {
                    method = value.ToString();
                }
            }
        }

        

        public GatherSetting(string name,string type,int frequency,string method)
        {
            Name = name;
            Type = type;
            Frequency = frequency;
            Method = method;
        }
        public GatherSetting(string name, string type, int frequency)
        {
            Name = name;
            Type = type;
            Frequency = frequency;
            Method = "sum";
        }
        public GatherSetting(string name, string type)
        {
            Name = name;
            Type = type;
            Frequency = 60;
            Method = "avg";
        }
        public GatherSetting(string name)
        {
            Name = name;
            Type = "double";
            Frequency = 60;
            Method = "avg";
        }

        static GatherSetting()
        {
            
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
