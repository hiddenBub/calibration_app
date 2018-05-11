using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calibration_app.SetOption
{
    public class ProjectSetting
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        private string proName;

        /// <summary>
        /// 基站位置经度
        /// </summary>
        private double lng;

        /// <summary>
        /// 基站位置纬度
        /// </summary>
        private double lat;

        public string ProName { get => proName; set => proName = value; }
        public double Lng { get => lng; set => lng = value; }
        public double Lat { get => lat; set => lat = value; }


        public ProjectSetting()
        {
            ProName = "";
            Lng = 116.328354;
            lat = 39.984980;
        }
    }
}
