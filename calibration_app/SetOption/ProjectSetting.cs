using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace calibration_app.SetOption
{
    public class ProjectSetting
    {
        private int pid;
        private string name;
        private double lng;
        private double lat;

        public int Pid { get => pid; set => pid = value; }
        public string Name { get => name; set => name = value; }
        public double Lng { get => lng; set => lng = value; }
        public double Lat { get => lat; set => lat = value; }
    }
}
