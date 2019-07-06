using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;
using System.Linq;
public class ClientSocket : MonoBehaviour
{
    private static ClientSocket m_Instance;
    public static ClientSocket Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private const string IP = "127.0.0.1";
    private const int PORT = 6688;
    private Socket clientSocket;

    private MessageMgr msg = new MessageMgr();

    private void Awake()
    {
        m_Instance = this;
        //初始化协议管理器
        ProtocolManager.Instance.Init();
    }
    private void Start()
    {        
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(IP, PORT);
            //开始监听服务器数据
            BeginReceive();
        }
        catch (Exception e)
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
            Debug.LogError("无法连接到服务器，请检查网络！/n" + e);
        }
    }

    private void BeginReceive()
    {
        if (clientSocket != null)
        {
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }
    }

    /// <summary>
    /// 异步接收服务器数据
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        //try
        //{
        if (clientSocket == null || clientSocket.Connected == false)
        {
            return;
        }
        //获取到服务器数据
        int count = clientSocket.EndReceive(ar);
        //解包并交给某个协议方法处理数据后续业务 
        msg.ReadMessage(count); //疑问：在Message中的该方法，开启了一个while(true) 那么下面的代码会等待while退出才知道吗？            
                                //继续监听服务器端的数据传递
        BeginReceive();
        //}
        //catch (Exception e)
        //{
        //    if (clientSocket != null)
        //    {
        //        clientSocket.Close();
        //    }
        //    Debug.Log(e);
        //}
    }

    /// <summary>
    /// 发送请求协议给服务器
    /// </summary>
    /// <param name="requestData"></param>
    public void SendRequest(byte[] requestData)
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            //加上数据长度（必须！！！！）我哭了。。。
            byte[] dataCountBytes = BitConverter.GetBytes(requestData.Length);
            clientSocket.Send(dataCountBytes.Concat(requestData).ToArray());
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("无法关闭与服务器的连接/n" + e);
        }
    }
}
