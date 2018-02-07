using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Tangh.NewInfrastructure
{
    public class SocketServer
    {
        public event Action<string> NotifyMsg = null;
        public event Action<string, string> ReceiveMsg = null;

        /// <summary>
        /// 只有在 isSingleIPMuliClient 设置True的时候使用
        /// </summary>
        public event Action<string, string, string> MuliReceiveMsg = null;

        private string ip = string.Empty;
        private int port = 0;
        private bool isSingleIPMuliClient = false;

        public SocketServer(string ip, int port, bool isSingleIPMuliClient = false)
        {
            this.ip = ip;
            this.port = port;
            this.isSingleIPMuliClient = isSingleIPMuliClient;
        }

        public void StartListen(int count = 1000)
        {
            try
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Parse(this.ip), this.port));
                server.Listen(count);
                this.NotifyMsgMethod("开始监听：" + this.ip + ":" + this.port);
                while (true)
                {
                    Socket client = server.Accept();
                    this.NotifyMsgMethod("收到一个连接用户:" + (client.RemoteEndPoint as IPEndPoint).Address.ToString());
                    ThreadPool.QueueUserWorkItem(new WaitCallback(HangUpMsg), client);
                }
            }
            catch (Exception ex)
            {
                this.NotifyMsgMethod("发生异常：" + ex.Message);
            }
        }

        public void SendMsgToClient(string msg, string ip, string guid = "")
        {
            if (this.isSingleIPMuliClient)
            {
            }
            else
            {
            }
        }

        private void HangUpMsg(object obj)
        {
            Socket client = obj as Socket;
            string ip = (client.RemoteEndPoint as IPEndPoint).Address.ToString();

            try
            {
                while (true)
                {
                    try
                    {
                        byte[] header = new byte[4];
                        int count = client.Receive(header, SocketFlags.None);
                        if (count > 0)
                        {
                            int bodyLen = (header[0] << 24);
                            bodyLen += (header[1] << 16);
                            bodyLen += (header[2] << 8);
                            bodyLen += header[3];

                            byte[] body = new byte[bodyLen];
                            count = client.Receive(body, SocketFlags.None);
                            while (count < bodyLen)
                            {
                                int temp = client.Receive(body, count, bodyLen - count, SocketFlags.None);
                                count = count + temp;
                            }

                            if (this.isSingleIPMuliClient)
                            {
                                string guid = Encoding.UTF8.GetString(body.Take(32).ToArray());
                                string msg = Encoding.UTF8.GetString(body.Skip(32).ToArray());
                                MuliReceiveMsgMethod(ip, guid, msg);
                            }
                            else
                            {
                                string msg = Encoding.UTF8.GetString(body);
                                ReceiveMsgMethod(ip, msg);
                            }
                        }
                    }
                    finally
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch { }
            }
        }

        #region 事件对于本地函数

        private void ReceiveMsgMethod(string ip, string msg)
        {
            if (this.ReceiveMsg != null)
            {
                this.ReceiveMsg(ip, msg);
            }
        }

        private void MuliReceiveMsgMethod(string guid, string ip, string msg)
        {
            if (this.MuliReceiveMsg != null)
            {
                this.MuliReceiveMsg(guid, ip, msg);
            }
        }

        private void NotifyMsgMethod(string msg)
        {
            if (this.NotifyMsg != null)
            {
                this.NotifyMsg(msg);
            }
        }
        #endregion
    }
}
