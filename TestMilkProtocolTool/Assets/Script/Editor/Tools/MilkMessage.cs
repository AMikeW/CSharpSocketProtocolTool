﻿using System;
using System.Runtime.InteropServices;

public class MilkMessage
{
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);//具体方法
}
