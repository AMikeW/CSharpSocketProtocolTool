using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GameServer.Protocol.Data;
using GameServer.SocketFile;
using Assets.Script.Protocol;

namespace GameServer.Protocol
{
    public class ProtocolMgr
    {
        private static ProtocolMgr m_Instance;
        public static ProtocolMgr Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new ProtocolMgr();
                    }
                }
                return m_Instance;
            }
        }
        
        //缓存所有协议类对象模板 - <协议号, 协议类对象>
        private Dictionary<ProtocolNo, object> protocolObjectDict = new Dictionary<ProtocolNo, object>();
        //缓存所有协议类【解包】成员列表 -Unpack :接收到客户端数据时用于解包
        private Dictionary<ProtocolNo, List<object>> protocolUnPackMemberDict = new Dictionary<ProtocolNo, List<object>>();
        //缓存所有协议类【封包】成员列表 -Pack :给客户端发送数据时用于封包
        private Dictionary<ProtocolNo, List<object>> protocolPackMemberDict = new Dictionary<ProtocolNo, List<object>>();
        //缓存所有解包时会遇到的类结构List<object>形式存储,以DataID来区分每个类
        private Dictionary<DataID, List<object>> unPackClassStructDict = new Dictionary<DataID, List<object>>();

        private ProtocolMgr(){}

        /// <summary>
        /// 初始化ProtocolMgr
        /// </summary>
        public void Init()
        {
            InitSystem.Init(this);
        }

        #region 缓存数据相关
        /// <summary>
        /// 添加协议解包成员列表
        /// </summary>
        /// <param name="protocolNo">协议号</param>
        /// <param name="objectList">解包成员列表</param>
        public void AddProtocolObjectToDict(ProtocolNo protocolNo, object o)
        {
            if (protocolObjectDict.ContainsKey(protocolNo)) { return; }
            protocolObjectDict.Add(protocolNo, o);
        }
        /// <summary>
        /// 添加协议解包成员列表
        /// </summary>
        /// <param name="protocolNo">协议号</param>
        /// <param name="objectList">解包成员列表</param>
        public void AddProtocolUnPackMemberListToDict(ProtocolNo protocolNo, List<object> objectList)
        {
            if (protocolUnPackMemberDict.ContainsKey(protocolNo)) { return; }
            protocolUnPackMemberDict.Add(protocolNo, objectList);
        }

        /// <summary>
        /// 添加协议封包成员列表
        /// </summary>
        /// <param name="protocolNo">协议号</param>
        /// <param name="objectList">封包成员列表</param>
        public void AddProtocolPackMemberListToDict(ProtocolNo protocolNo, List<object> objectList)
        {
            if (protocolPackMemberDict.ContainsKey(protocolNo)) { return; }
            protocolPackMemberDict.Add(protocolNo, objectList);
        }

        /// <summary>
        /// 添加解包中的类结构List<object>形式存储,DataID区分
        /// </summary>
        /// <param name="protocolNo">协议号</param>
        /// <param name="objectList">封包成员列表</param>
        public void AddUnPackClassStructToDict(DataID dataID, List<object> objectList)
        {
            if (unPackClassStructDict.ContainsKey(dataID)) { return; }
            unPackClassStructDict.Add(dataID, objectList);
        }

        /// <summary>
        /// 根据协议号获取解包成员列表
        /// </summary>
        public List<object> GetProtocolUnPackMemberListByProtocolNo(ProtocolNo protocolNo)
        {
            List<object> list = null;
            protocolUnPackMemberDict.TryGetValue(protocolNo, out list);
            return list;
        }

        /// <summary>
        /// 根据协议号获取发包成员列表
        /// </summary>
        public List<object> GetProtocolPackMemberListByProtocolNo(ProtocolNo protocolNo)
        {
            List<object> list = null;
            protocolPackMemberDict.TryGetValue(protocolNo, out list);
            return list;
        }

        /// <summary>
        /// 根据DataID获取类结构
        /// </summary>
        public List<object> GetUnPackClassStructByDataID(DataID dataID)
        {
            List<object> list = null;
            unPackClassStructDict.TryGetValue(dataID, out list);
            return list;
        }      
        #endregion

        #region 通用解包方法

        /// <summary>
        /// 通用解包方法 需保证bytes数组完整性
        /// </summary>        
        public object StartUnPack(ref byte[] bytes,ref ProtocolEventType type)
        {
            if (bytes.Length <= 12) return null;
            int dataCount = ProtocolUtility.UnPackInt(ref bytes);//数据长度
            type = (ProtocolEventType)ProtocolUtility.UnPackInt(ref bytes);//事件类型
            ProtocolNo protoNo = (ProtocolNo)ProtocolUtility.UnPackInt(ref bytes);//协议号
            object protocolObject = null;
            protocolObjectDict.TryGetValue(protoNo, out protocolObject);
            if (protocolObject != null)
            {
                List<object> list = GetProtocolUnPackMemberListByProtocolNo(protoNo);
                List<object> unPackObjectList = new List<object>();    
                //递归通用解析（即遇到是类结构的成员会继续解析该类成员)，所有的数据会被会解析出来并放入List<object>第三个参数
                CommonUnPack(ref bytes, list, ref unPackObjectList);
                //多线程时要加锁!这个缓存的protocolObject协议对象仅仅是为了拷贝 only for copy!when multiple thread, it must be locked!
                ((ProtocolData)protocolObject).SetData(unPackObjectList);
                //深拷贝方式 deepCopy method(way)
                return ((ProtocolData)protocolObject).Clone();
            }
            else
            {
                throw new Exception("protocolNo" + protoNo + ",not exist protocolObject in protocolObjectDict!");
            }
        }

        /// <summary>
        /// 通用递归解析方法
        /// </summary>
        /// <param name="bytes">要解析的数据字节数组</param>
        /// <param name="list">要解析的数据结构</param>
        /// <param name="unPackObjectList">解析出数据结构每个成员后保存的列表</param>
        private void CommonUnPack(ref byte[] bytes, List<object> list, ref List<object> unPackObjectList)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is string)
                {
                    //解析string是特殊的，需要先获取int（string数据长度)再解析出指定数据长度的string
                    int stringLength = ProtocolUtility.UnPackInt(ref bytes);
                    unPackObjectList.Add(ProtocolUtility.UnPackString(ref bytes, stringLength));
                }
                else if (list[i] is int)
                {
                    unPackObjectList.Add(ProtocolUtility.UnPackInt(ref bytes));
                }
                else if (list[i] is float)
                {
                    unPackObjectList.Add(ProtocolUtility.UnPackFloat(ref bytes));
                }
                else if (list[i] is double)
                {
                    unPackObjectList.Add(ProtocolUtility.UnPackDouble(ref bytes));
                }
                else if (list[i] is bool)
                {
                    unPackObjectList.Add(ProtocolUtility.UnPackBool(ref bytes));
                }
                else if (list[i] is char)
                {
                    unPackObjectList.Add(ProtocolUtility.UnPackChar(ref bytes));
                }
                else if (list[i].GetType() == typeof(ClassBase))
                {
                    //先解析出它的DataID
                    int dataID = ProtocolUtility.UnPackInt(ref bytes);
                    //根据dataID获取类结构List<object>根据它,解析该类的内部信息存入unPackObjectList
                    CommonUnPack(ref bytes, GetUnPackClassStructByDataID((DataID)dataID), ref unPackObjectList);
                }
                else
                {
                    throw new Exception("接收了一个未经处理的类型!" + list[i].GetType());
                }
            }
        }


        //通用发包方法
        public void SendRequest(ProtocolNo protocolNo,ProtocolEventType type, Client client, params object[] objectArr)
        {
            try
            {
                if (objectArr.Length <= 0) { throw new Exception("参数列表为空!"); }
                //根据协议号获取发包成员列表
                List<object> packMemberList = GetProtocolPackMemberListByProtocolNo(protocolNo);
                if (packMemberList.Count != objectArr.Length)
                {
                    throw new Exception(protocolNo + "协议发包成员个数与规定个数不符合!");
                }
                //查看是否满足协议发包成员列表的顺序
                for (int i = 0; i < packMemberList.Count; i++)
                {
                    if (packMemberList[i].GetType() != objectArr[i].GetType() && packMemberList[i].GetType() != objectArr[i].GetType().BaseType)
                    {
                        throw new Exception(protocolNo + "协议发包成员有误!具体为第" + i + 1 + "个成员类型应为" + packMemberList[i].GetType() + ",而不是" + objectArr[i].GetType());
                    }
                }
                byte[] bytes = null;
                CommonPack(ref bytes, objectArr.ToList());
                //协议号字节数组
                byte[] dataBytes = BitConverter.GetBytes((int)protocolNo);
                //事件ID字节数组            
                byte[] eventIDBytes = BitConverter.GetBytes((int)type);
                client.SendResponse(eventIDBytes.Concat(dataBytes.Concat(bytes).ToArray()).ToArray());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private void CommonPack(ref byte[] bytes, List<object> objectList)
        {
            foreach (var o in objectList)
            {
                if (o is int || o is DataID)
                {
                    ProtocolUtility.PacketInt(ref bytes, (int)o);
                }
                else if (o is float)
                {
                    ProtocolUtility.PacketFloat(ref bytes, (float)o);
                }
                else if (o is double)
                {
                    ProtocolUtility.PacketDouble(ref bytes, (double)o);
                }
                else if (o is bool)
                {
                    ProtocolUtility.PacketBool(ref bytes, (bool)o);
                }
                else if (o is string)
                {
                    ProtocolUtility.PacketString(ref bytes, (string)o);
                }
                else if (o.GetType().BaseType == typeof(ClassBase))
                {
                    CommonPack(ref bytes, ((ClassBase)o).objectList);
                }
                else
                {
                    throw new Exception("发送了一个未经处理的类型!" + o.GetType());
                }
            }
        }      
        #endregion
    }
}
