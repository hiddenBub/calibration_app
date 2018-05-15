using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace calibration_app.SetOption
{
    [XmlType(TypeName = "Setting")]
    public class Setting
    {
        [XmlIgnore]
        public object Parent;

        [XmlElement("Gather")]
        public Gather Gather { get; set; }

        [XmlElement("Project")]
        public Project Project { get; set; }
    }

    [XmlType(TypeName = "Project")]
    public class Project : Setting
    {
        //[XmlIgnore]
        //public Setting Parent = new Setting();

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Lng")]
        public double Lng { get; set; }

        [XmlElement("Lat")]
        public double Lat { get; set; }
    }

    [XmlType(TypeName = "Gather")]
    public class Gather :Setting
    {
        //[XmlIgnore]
        //public Setting Parent = new Setting();
        [XmlElement("DataPath")]
        public string DataPath { get; set; }

        [XmlArray("ColumnList")]
        public ObservableCollection<Column> ColumnList { get; set; }
    }

    [XmlType(TypeName = "Column")]
    public class Column : Gather, INotifyPropertyChanged
    {
        //[XmlIgnore]
        //public Gather Parent = new Gather();

        [XmlAttribute("Index")]
        public int Index { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Type")]
        public string Type { get; set; }
        [XmlAttribute("Frequency")]
        public int Frequency { get; set; }
        [XmlAttribute("Method")]
        public string Method { get; set; }



        #region 属性更改通知
        public event PropertyChangedEventHandler PropertyChanged;
        private void Changed(string PropertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion
    }


}
