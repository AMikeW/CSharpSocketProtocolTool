using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.SocketFile
{
    public class Client
    {
        private Server server;
        private Socket clientSocket;
        private Message msg;
        public Client()
        {
        }
        public Client(Socket clientSocket, Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            msg = new Message(this);
        }

        /// <summary>
        /// 监听客户端消息
        /// </summary>
        public void Start()
        {
            if (clientSocket != null && clientSocket.Connected == true)
            {
                //开启一个线程去接收
                clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
            }
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null || clientSocket.Connected == false) return;
                //接收数据 count为整个包的长度
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                }
                //处理数据
                msg.ReadMessage(count);
                //继续监听客户端消息
                Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Close();
            }
        }

        /// <summary>
        /// 发送响应给客户端
        /// </summary>
        public void SendResponse(byte[] datas)
        {
            if (clientSocket != null && clientSocket.Connected == true)
            {
                //需要拼接上数据长度作为包头
                byte[] dataCountBytes = BitConverter.GetBytes(datas.Length);
                clientSocket.Send(dataCountBytes.Concat(datas).ToArray());
            }
            else
            {
                Console.WriteLine("服务器已中断");
            }
        }

        private void Close()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
            if (server != null)
            {
                server.RemoveClient(this);
            }
        }
    }
}
