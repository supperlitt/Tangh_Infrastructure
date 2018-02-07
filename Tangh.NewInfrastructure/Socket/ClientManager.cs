using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections;

namespace Tangh.NewInfrastructure
{
    public class ClientManager
    {
        /// <summary>
        /// js解析类
        /// </summary>
        private static JavaScriptSerializer js = new JavaScriptSerializer();

        /// <summary>
        /// 数据存储
        /// </summary>
        private static List<ClientInfo> dataCache = new List<ClientInfo>();

        public static void AddClient(Socket client)
        {
            lock (dataCache)
            {
                string ip = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
                var item = dataCache.Find(p => p.Ip == ip);
                if (item == null)
                {
                    int index = dataCache.Count;
                    dataCache.Add(new ClientInfo()
                    {
                        Client = client,
                        Ip = ip
                    });
                }
                else
                {
                }
            }
        }

        public static void RemoveClient(string ip)
        {
            lock (dataCache)
            {
                var item = dataCache.Find(p => p.Ip == ip);
            }
        }

        private static byte[] GetHeaderByBody(int len)
        {
            byte[] header = new byte[4];
            header[0] = (byte)((len & 0xFF000000) >> 24);
            header[1] = (byte)((len & 0x00FF0000) >> 16);
            header[2] = (byte)((len & 0x0000FF00) >> 8);
            header[3] = (byte)(len & 0x000000FF);

            return header;
        }
    }

    public class ClientInfo
    {
        public string Ip { get; set; }

        public Socket Client { get; set; }

        public string Guid { get; set; }
    }

    public class CompareIP : IComparer<ClientInfo>
    {
        public int Compare(ClientInfo x, ClientInfo y)
        {
            string[] xips = x.Ip.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string[] yips = y.Ip.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            long xNum = long.Parse(xips[0]) * 1000000000 + long.Parse(xips[1]) * 1000000 + long.Parse(xips[2]) * 1000 + long.Parse(xips[3]);
            long yNum = long.Parse(yips[0]) * 1000000000 + long.Parse(yips[1]) * 1000000 + long.Parse(yips[2]) * 1000 + long.Parse(yips[3]);

            if (xNum > yNum)
            {
                return 1;
            }
            else if (xNum == yNum)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
