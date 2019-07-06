using GameServer.ToolUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

public static class ProtocolUtility
{
    #region 封包

    public static byte[] PacketInt(int value)
    {
        return BitConverter.GetBytes(value);
    }

    public static void PacketInt(ref byte[] bytes, int value)
    {
        if (bytes == null)
        {
            bytes = PacketInt(value);
        }
        else
        {
            bytes = bytes.Concat(PacketInt(value)).ToArray();
        }
    }

    public static void PacketInt(ref byte[] bytes, string value)
    {
        if (bytes == null)
        {
            bytes = PacketInt(int.Parse(value));
        }
        else
        {
            bytes = bytes.Concat(PacketInt(int.Parse(value))).ToArray();
        }
    }

    public static void PacketInt<T>(ref byte[] bytes, T value) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
        if (bytes == null)
        {
            bytes = PacketInt(int.Parse(value.ToString()));
        }
        else
        {
            bytes = bytes.Concat(PacketInt(int.Parse(value.ToString()))).ToArray();
        }
    }

    public static byte[] PacketBool(bool value)
    {
        return BitConverter.GetBytes(value);
    }

    public static void PacketBool(ref byte[] bytes, bool value)
    {
        if (bytes == null)
        {
            bytes = BitConverter.GetBytes(value);
        }
        else
        {
            bytes = bytes.Concat(BitConverter.GetBytes(value)).ToArray();
        }
    }


    public static byte[] PacketFloat(float value)
    {
        return BitConverter.GetBytes(value);
    }

    public static void PacketFloat(ref byte[] bytes, float value)
    {
        if (bytes == null)
        {
            bytes = BitConverter.GetBytes(value);
        }
        else
        {
            bytes = bytes.Concat(BitConverter.GetBytes(value)).ToArray();
        }
    }

    public static byte[] PacketDouble(double value)
    {
        return BitConverter.GetBytes(value);
    }

    public static void PacketDouble(ref byte[] bytes, double value)
    {
        if (bytes == null)
        {
            bytes = BitConverter.GetBytes(value);
        }
        else
        {
            bytes = bytes.Concat(BitConverter.GetBytes(value)).ToArray();
        }
    }

    public static byte[] PacketString(string value)
    {
        byte[] stringBytes = Encoding.Unicode.GetBytes(value.ToArray());//字符串字节数组
        byte[] stringLengthBytes = BitConverter.GetBytes(stringBytes.Length);//字符串长度字节数组
        return stringLengthBytes.Concat(stringBytes).ToArray();
    }

    public static void PacketString(ref byte[] bytes, string value)
    {
        byte[] stringBytes = Encoding.Unicode.GetBytes(value.ToArray());//字符串字节数组
        byte[] stringLengthBytes = BitConverter.GetBytes(stringBytes.Length);//字符串长度字节数组
        if (bytes == null)
        {
            bytes = stringLengthBytes.Concat(stringBytes).ToArray();
        }
        else
        {
            bytes = bytes.Concat(stringLengthBytes).Concat(stringBytes).ToArray(); //先拼接字符串长度，再拼接字符串
        }
    }

    #endregion


    #region 解包

    public static int UnPackInt(ref byte[] bytes)
    {
        int outValue = BitConverter.ToInt32(bytes, 0);
        Tool.ByteSub(ref bytes, sizeof(int));
        return outValue;
    }

    public static bool UnPackBool(ref byte[] bytes)
    {
        bool outValue = BitConverter.ToBoolean(bytes, 0);
        Tool.ByteSub(ref bytes, sizeof(bool));
        return outValue;
    }

    public static float UnPackFloat(ref byte[] bytes)
    {
        float outValue = BitConverter.ToSingle(bytes, 0);
        Tool.ByteSub(ref bytes, sizeof(float));
        return outValue;
    }

    public static double UnPackDouble(ref byte[] bytes)
    {
        double outValue = BitConverter.ToDouble(bytes, 0);
        Tool.ByteSub(ref bytes, sizeof(double));
        return outValue;
    }

    public static string UnPackString(ref byte[] bytes, int count)//count是[字节]个数
    {
        string str = Encoding.Unicode.GetString(bytes, 0, count);
        //str = str.Replace("\0", "");// "\0"是byte缺省值，要替换为""
        Tool.ByteSub(ref bytes, count);
        return str;
    }

    public static char UnPackChar(ref byte[] bytes)
    {
        char outValue = BitConverter.ToChar(bytes, 0);
        Tool.ByteSub(ref bytes, sizeof(char));
        return outValue;
    }

    #endregion

}