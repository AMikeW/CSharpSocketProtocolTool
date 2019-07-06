using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameServer.Handle;
using GameServer.Protocol;
namespace GameServer.SocketFile
{
    public class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private List<Client> clientList = new List<Client>();
        public Server() { }
        public Server(string ipStr, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
            ProtocolMgr.Instance.Init();
        }
        ~Server()
        {
        }
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("正在捆绑IP和端口号... ...");
            //捆绑IP和端口
            serverSocket.Bind(ipEndPoint);
            Console.WriteLine("捆绑IP和端口号完毕");
            //监听无数个客户端连接
            serverSocket.Listen(0);
            Console.WriteLine("开启监听客户端连接");
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        /// <summary>
        /// 异步接收客户端里阿尼额
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallBack(IAsyncResult ar)
        {
            Console.WriteLine("正在接收客户端连接... ...");
            Socket clientSocket = serverSocket.EndAccept(ar);
            Console.WriteLine("接收到一条客户端连接" + ar.ToString());
            Client client = new Client(clientSocket, this);
            clientList.Add(client);
            //该客户端socket开启监听客户端消息
            client.Start();
            //继续接收下一个客户端连接
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        public void RemoveClient(Client client)
        {
            lock (clientList)
            {
                clientList.Remove(client);
            }
        }
    }
}
