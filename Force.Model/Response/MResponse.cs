using System;
using System.Collections.Generic;
using System.Text;

namespace Force.Model.Response
{
    public class MResponse<T>
    {
        /// <summary>
        /// 返回的msg
        /// </summary>
        public string Msg { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { set; get; }
        /// <summary>
        /// 返回的内容
        /// </summary>
        public T Data { set; get; }
        /// <summary>
        /// 服务端版本
        /// </summary>
        public string Version { set; get; }
    }
}
