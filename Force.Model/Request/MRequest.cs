using Force.Model.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Force.Model.Request
{
    public class MRequest
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long TimeStamp { set; get; }
        /// <summary>
        /// 数据token，解析出基础数据用
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 数据token，解析出基础数据用
        /// </summary>
        public TokenInfo TokenInfo { set; get; }
        /// <summary>
        /// 需提交的参数
        /// </summary>
        public Dictionary<string, object> Params { set; get; }
    }
}
