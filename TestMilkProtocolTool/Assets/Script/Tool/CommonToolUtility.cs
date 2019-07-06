using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class CommonToolUtility
{  

    /// <summary>
    /// 截取bytes的startIndex开始的元素,bytes数组长度不变
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="startIndex"></param>
    public static void ByteSub(ref byte[] bytes, int startIndex)
    {
        //截取startIndex开始的字节到新的bytes数组
        byte[] temps = new byte[bytes.Length];
        for (int i = 0; i < temps.Length - startIndex; i++)
        {
            temps[i] = bytes[startIndex + i];
        }
        bytes = temps;
    }

    /// <summary>
    /// 截取bytes数组startIndex开始的count个到temp数组，以tempStartIndex为开始位置
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="startIndex"></param>
    /// <param name="temp"></param>
    /// <param name="tempStartIndex"></param>
    /// <param name="count"></param>
    public static void ByteSub(byte[] bytes, int startIndex, ref byte[] temp, int tempStartIndex, int count)
    {
        if (startIndex + count > bytes.Length)
        {
            Debug.LogError("被拷贝数据的截取范围已超出数组范围!");
            return;
        }
        if (tempStartIndex + count > bytes.Length)
        {
            Debug.LogError("拷贝数组的范围已超出数组范围!");
            return;
        }
        //截取startIndex开始的字节到新的bytes数组
        temp = new byte[bytes.Length];
        for (int i = tempStartIndex; i < count; i++)
        {
            temp[i] = bytes[startIndex + i];
        }
        bytes = temp;
    }    
}
