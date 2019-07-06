using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.ToolUtility
{
    public static class Tool
    {
        /// <summary>
        /// 截取bytes数组的startIndex索引开始的字节，将这段字节返回给它自身，字节数组长度不变!
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex">开始截取索引</param>
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
        /// 截取bytes数组的startIndex开始到temp数组下,count个字节
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="temp"></param>
        /// <param name="tempStartIndex"></param>
        /// <param name="count"></param>
        public static void ByteSub(byte[] bytes, int startIndex, ref byte[] temp, int count)
        {
            if (startIndex + count > bytes.Length)
            {
                Console.WriteLine("被拷贝数据的截取范围已超出数组范围!");
                return;
            }
            //截取startIndex开始的字节到新的bytes数组            
            for (int i = 0; i < count; i++)
            {
                temp[i] = bytes[startIndex + i];
            }
            bytes = temp;
        }
    }
}
