using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.ZMYY.Models
{
    internal class CustSubscribeDateDetailResponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 成都市成华区跳蹬河社区卫生服务中心杉板桥分中心
        /// </summary>
        public string customer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CustSubscribeDateDetailItem> list { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ver { get; set; }

    }
}
