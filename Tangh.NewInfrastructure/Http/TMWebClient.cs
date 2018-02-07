using System;
using System.Net;

namespace Tangh.NewInfrastructure.Http
{
    /// <summary>
    /// 简答HTTP请求，继承与WebClient，可以自定义超时时间
    /// 默认超时时间：30秒
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

        public TMWebClient()
        {
            this._timeout = 30000;
        }

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
