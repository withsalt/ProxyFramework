using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.ZMYY.Models
{
    internal class ListItemModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 九价人乳头瘤病毒疫苗
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 九价简介：接种九价宫颈癌疫苗可以预防人乳头瘤病毒6、11、16、18、31、33、45、52、58型感染引起的尖锐湿疣、癌前病变、原位腺癌和宫颈癌，是目前覆盖HPV型别最多的HPV疫苗。全程三针， 适用于16-26岁女性。
        /// </summary>
        public string descript { get; set; }
        /// <summary>
        /// 适用于16-26岁女性。
        /// </summary>
        public string warn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> tags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int questionnaireId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NumbersVaccineItemModel> NumbersVaccine { get; set; }
        /// <summary>
        /// 暂无
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 暂未开始
        /// </summary>
        public string BtnLable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool enable { get; set; }
    }
}
