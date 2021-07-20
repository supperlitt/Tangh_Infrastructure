using System;
using System.Net;

namespace SupperlittTool
{
    /// <summary>
    /// Http网络请求(支持自定义超时时间)
    /// </summary>
    public class TMWebClient : WebClient
    {
        private int _timeout;

        /// <summary>
        /// 超时时间(毫秒)
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        /// <summary>
        /// 默认超时30秒
        /// </summary>
        public TMWebClient()
        {
            this._timeout = 30000;
        }

        /// <summary>
        /// 超时单位：秒
        /// </summary>
        /// <param name="timeout">秒</param>
        public TMWebClient(int timeout)
        {
            this._timeout = timeout * 1000;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = base.GetWebRequest(address);
            result.Timeout = this._timeout;

            return result;
        }
    }
}
