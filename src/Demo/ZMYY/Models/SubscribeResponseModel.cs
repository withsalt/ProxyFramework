using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.ZMYY.Models
{
    internal class SubscribeResponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 成华区杉板桥路266号5栋1层40号
        /// </summary>
        public string addr { get; set; }
        /// <summary>
        /// 成都市成华区跳蹬河社区卫生服务中心杉板桥分中心
        /// </summary>
        public string cname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double distance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PaymentModel payment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BigPic { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IdcardLimit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string notice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ListItemModel> list { get; set; }
    }
}
