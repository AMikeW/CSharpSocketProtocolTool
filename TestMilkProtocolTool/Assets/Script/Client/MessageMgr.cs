using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageMgr
{
    private List<byte> tempList = new List<byte>();
    private byte[] data = new byte[1024];
    private int startIndex = 0;
    public byte[] Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }
    public int StartIndex
    {
        get
        {
            return startIndex;
        }
        set
        {
            startIndex = value;
        }
    }
    public int RemainSize
    {
        get
        {
            return data.Length - startIndex;
        }
    }

    public MessageMgr()
    {

    }
    /// <summary>
    /// 处理客户端接收服务器数据的解包
    /// </summary>
    /// <param name="count"></param>
    public void ReadMessage(int count)
    {
        startIndex += count;
        while (true)
        {
            //通用解包和发包                
            if (startIndex > 12)
            {
                //仅有没有缓存情况才考虑完整数据
                if (tempList.Count <= 0)
                {
                    int dataCount = BitConverter.ToInt32(data, 0);
                    //1.数据完整情况
                    if (startIndex - 4 >= dataCount)
                    {
                        ProtocolEventType type = ProtocolEventType.Null;
                        object o = ProtocolManager.Instance.StartUnPack(ref data, ref type);
                        if (type != ProtocolEventType.Null)
                        {
                            EventSystem.Instance.Dispatch(type, (ProtocolData)o);
                        }
                        startIndex -= (4 + dataCount);
                    }
                    else//2.数据不完整情况,存[0,startIndex-1]字节缓存list
                    {
                        for (int i = 0; i < startIndex; i++)
                        {
                            tempList.Add(data[i]);
                        }
                        //出现数据不完整情况，就必定是数据不完整是最后面的一组协议,故后面应该是没有任何字节的了
                        Array.Clear(data, 0, data.Length);//全清（可能会引发bug）
                        startIndex = 0;
                    }
                }
                else//3.肯定是缓存的数据后续部分(一个协议后续部分数据)到来的情况，要继续获取那一部分数据
                {
                    HandleTempList();
                }
            }
            else
            {
                //有缓存的情况下，可能是缓存的相关数据到来                    (目前仅考虑)
                if (tempList.Count > 0)
                {
                    HandleTempList();
                }
                else
                {
                    //协议异常直接退出
                    return;
                }
            }
        }
    }
    private void HandleTempList()
    {
        //现有缓存数据长度
        int curLength = tempList.Count - 4;
        //总数据长度 = 保存于缓存[0，3]字节
        int sumLength = BitConverter.ToInt32(tempList.ToArray(), 0);
        //所需数据长度
        int needLength = sumLength - curLength;
        //可取长度
        int canGetLength = Math.Min(this.startIndex, needLength);//其实这里可能肯定就是startIndex，不然就是协议异常了..
        for (int i = 0; i < canGetLength; i++)
        {
            tempList.Add(data[i]);
        }
        //去除已取出部分字节
        CommonToolUtility.ByteSub(ref data, canGetLength);
        this.startIndex -= canGetLength;
        curLength = tempList.Count - 4;
        //获取到完整数据开始解析(正常情况下，肯定是完整了）
        if (sumLength - curLength <= 0)
        {
            ProtocolEventType type = ProtocolEventType.Null;
            byte[] bytesArr = tempList.ToArray();
            object o = ProtocolManager.Instance.StartUnPack(ref bytesArr, ref type);
            if (type != ProtocolEventType.Null)
            {
                EventSystem.Instance.Dispatch(type, (ProtocolData)o);
            }            
            //清空缓存
            tempList.Clear();
        }
    }
}
