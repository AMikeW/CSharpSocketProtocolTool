using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEditor;

public class ParseJsonSystem
{
    private static string ServerPath = @"G:/服务器-应用控制台程序";
    private const string LINK = "\n";
    private const string TAB = "    ";
    private const string LEFT = "{\n";
    private const string RIGHT = "}\n";
    [MenuItem("Tools/Editor/Config/自动生成客户端协议脚本")]
    public static void ParseClient()
    {
        ParseAllClass();
        ParseAllProtocol();
        ParseInitSystem();
        ParseEnum();
    }
    //[MenuItem("Tools/Editor/Config/客户端/自动生成协议相关数据类脚本")]
    public static void ParseAllClass()
    {
        if (!Directory.Exists(Application.dataPath + "/Script/Editor/Config/ClassStruct"))
        {
            Debug.LogError(Application.dataPath + "/Script/Editor/Config/ClassStruct is not exist!");
            return;
        }
        string path = Application.dataPath + "/Script/Protocol/AutoClassStruct/";
        string[] files = Directory.GetFiles(Application.dataPath + "/Script/Editor/Config/ClassStruct", "*.json", SearchOption.AllDirectories);
        for (int index = 0; index < files.Length; index++)
        {
            FileStream fileStream = File.OpenRead(files[index]);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string text = streamReader.ReadToEnd();
            //Debug.Log(text);
            if (text != null)
            {
                ClassStruct classStruct = JsonUtility.FromJson<ClassStruct>(text);
                StringBuilder sb = new StringBuilder();
                StringBuilder memberSb = new StringBuilder();
                StringBuilder initSb = new StringBuilder();
                string str = "";
                switch (classStruct.structType)
                {
                    case "Pack":
                        sb.Append(string.Format("public class {0} : ClassBase", classStruct.className)).Append(LINK);
                        sb.Append(LEFT);
                        initSb.Append(TAB).Append(TAB).Append(string.Format("AddMemberObject((DataID){0});", classStruct.dataID)).Append(LINK);
                        foreach (var v in classStruct.memberDataList)
                        {
                            sb.Append(TAB).Append("//" + v.val_describe).Append(LINK);
                            sb.Append(TAB).Append(string.Format("public {0} {1};", v.val_type, v.val_name)).Append(LINK);
                            memberSb.Append(v.val_type + " " + v.val_name + ",");
                            initSb.Append(TAB).Append(TAB).Append(string.Format("this.{0} = {1};", v.val_name, v.val_name)).Append(LINK);
                            initSb.Append(TAB).Append(TAB).Append(string.Format("AddMemberObject(this.{0});", v.val_name)).Append(LINK);
                        }
                        str = memberSb.ToString();
                        if (str.Length > 0)
                        {
                            str = str.Substring(0, str.Length - 1);
                        }
                        sb.Append(TAB).Append(string.Format("public {0}({1})", classStruct.className, str)).Append(LINK);
                        sb.Append(TAB).Append(LEFT);
                        sb.Append(initSb.ToString());
                        sb.Append(TAB).Append(RIGHT);
                        sb.Append(RIGHT);
                        File.WriteAllText(path + classStruct.className + ".cs", sb.ToString());
                        break;
                    case "UnPack":
                        sb.Append("using System.Collections.Generic;").Append(LINK);
                        sb.Append(string.Format("public class {0}", classStruct.className)).Append(LINK);
                        sb.Append(LEFT);
                        sb.Append(TAB).Append(string.Format("public const DataID DATAID = (DataID){0};", classStruct.dataID)).Append(LINK);
                        foreach (var v in classStruct.memberDataList)
                        {
                            sb.Append(TAB).Append("//" + v.val_describe).Append(LINK);
                            sb.Append(TAB).Append(string.Format("public {0} {1};", v.val_type, v.val_name)).Append(LINK);
                            memberSb.Append(v.val_type + " " + v.val_name + ",");
                            initSb.Append(TAB).Append(TAB).Append(string.Format("this.{0} = {1};", v.val_name, v.val_name)).Append(LINK);
                        }
                        str = memberSb.ToString();
                        if (str.Length > 0)
                        {
                            str = str.Substring(0, str.Length - 1);
                        }
                        sb.Append(TAB).Append(string.Format("public {0}({1})", classStruct.className, str)).Append(LINK);
                        sb.Append(TAB).Append(LEFT);
                        sb.Append(initSb.ToString());
                        sb.Append(TAB).Append(RIGHT);
                        sb.Append(TAB).Append(string.Format("public static {0} ToClass(List<object> objectList, ref int index)", classStruct.className)).Append(LINK);
                        sb.Append(TAB).Append(LEFT);
                        //sb.Append(TAB).Append(TAB).Append(string.Format("index += {0};", classStruct.memberDataList.Length)).Append(LINK);
                        StringBuilder memberListSb = new StringBuilder();
                        for (int i = 0; i < classStruct.memberDataList.Length; i++)
                        {
                            if (classStruct.memberDataList[i].val_type != "int" && classStruct.memberDataList[i].val_type != "float"
                                && classStruct.memberDataList[i].val_type != "double" && classStruct.memberDataList[i].val_type != "chat"
                                && classStruct.memberDataList[i].val_type != "string")
                            {
                                memberListSb.Append(classStruct.memberDataList[i].val_type + ".ToClass(objectList, ref index)");
                            }
                            else
                            {
                                memberListSb.Append(string.Format("({0})objectList[index++]", classStruct.memberDataList[i].val_type));
                            }
                            if (i != classStruct.memberDataList.Length - 1)
                            {
                                memberListSb.Append(", ");
                            }
                        }
                        sb.Append(TAB).Append(TAB).Append(string.Format("return new {0}({1});", classStruct.className, memberListSb.ToString())).Append(LINK);
                        sb.Append(TAB).Append(RIGHT);
                        sb.Append(RIGHT);
                        File.WriteAllText(path + classStruct.className + ".cs", sb.ToString());
                        break;
                }
                streamReader.Close();
                fileStream.Close();
                streamReader.Dispose();
                fileStream.Dispose();
                streamReader = null;
                fileStream = null;
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //[MenuItem("Tools/Editor/Config/客户端/自动生成协议类脚本")]
    public static void ParseAllProtocol()
    {
        string path = Application.dataPath + "/Script/Protocol/AutoProtocolStruct/";
        float time = 0;
        try
        {
            EditorUtility.DisplayProgressBar("自动生成协议类脚本", "开始自动生成", 0);
            if (!Directory.Exists(Application.dataPath + "/Script/Editor/Config/ProtocolStruct"))
            {
                Debug.LogError(Application.dataPath + "/Script/Editor/Config/ProtocolStruct is not exist!");
                return;
            }
            string[] files = Directory.GetFiles(Application.dataPath + "/Script/Editor/Config/ProtocolStruct", "*.json", SearchOption.TopDirectoryOnly);
            for (int index = 0; index < files.Length; index++)
            {
                string str = "";
                time += Time.deltaTime * 3;
                for (int k = 0; k < time % 7; k++)
                {
                    str += ".";
                }
                EditorUtility.DisplayProgressBar("自动生成协议类脚本", "正在进行中..." + str, (float)index / files.Length);
                FileStream fileStream = File.OpenRead(files[index]);
                StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    ProtocolStruct protocolStruct = JsonUtility.FromJson<ProtocolStruct>(text);
                    string fullPath = path + protocolStruct.protocolName + ".cs";
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    //FileStream fs = File.Open(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("using System;").Append(LINK);
                    sb.Append("using System.Collections;").Append(LINK);
                    sb.Append("using System.Collections.Generic;").Append(LINK);
                    sb.Append("using UnityEngine;").Append(LINK);
                    sb.Append(string.Format("public class {0} : ProtocolData, ICloneable", protocolStruct.protocolName)).Append(LINK);
                    sb.Append(LEFT);
                    StringBuilder classMemberSb = new StringBuilder();
                    classMemberSb.Append(TAB).Append(TAB).Append("List<object> unPackMemberList = new List<object>();").Append(LINK);
                    for (int i = 0; i < protocolStruct.unPackMemberArr.Length; i++)
                    {
                        string tempType = "";
                        switch (protocolStruct.unPackMemberArr[i].val_type)
                        {
                            case "int":
                                tempType = "INT32";
                                break;
                            case "string":
                                tempType = "STRING";
                                break;
                            case "float":
                                tempType = "FLOAT";
                                break;
                            case "double":
                                tempType = "DOUBLE";
                                break;
                            case "char":
                                tempType = "CHAR";
                                break;
                            default:
                                tempType = "CLASS";
                                break;
                        }
                        sb.Append(TAB).Append("//" + protocolStruct.unPackMemberArr[i].val_describe).Append(LINK);
                        sb.Append(TAB).Append(string.Format("public {0} {1};", protocolStruct.unPackMemberArr[i].val_type, protocolStruct.unPackMemberArr[i].val_name)).Append(LINK);
                        classMemberSb.Append(TAB).Append(TAB).Append(string.Format("unPackMemberList.Add(DefaultVar.{0});", tempType)).Append(LINK);
                    }
                    classMemberSb.Append(TAB).Append(TAB).Append(string.Format("ProtocolManager.Instance.AddProtocolUnPackMemberListToDict((ProtocolNo){0}, unPackMemberList);", protocolStruct.protocolID)).Append(LINK);
                    sb.Append(TAB).Append(string.Format("public {0} ()", protocolStruct.protocolName)).Append(LINK);
                    sb.Append(TAB).Append(LEFT);
                    sb.Append(classMemberSb.ToString()).Append(LINK);
                    sb.Append(TAB).Append(TAB).Append("List<object> packMemberList = new List<object>();").Append(LINK);
                    for (int i = 0; i < protocolStruct.packMemberArr.Length; i++)
                    {
                        string tempType = "";
                        switch (protocolStruct.packMemberArr[i].val_type)
                        {
                            case "int":
                                tempType = "INT32";
                                break;
                            case "string":
                                tempType = "STRING";
                                break;
                            case "float":
                                tempType = "FLOAT";
                                break;
                            case "double":
                                tempType = "DOUBLE";
                                break;
                            case "char":
                                tempType = "CHAR";
                                break;
                            default:
                                tempType = "CLASS";
                                break;
                        }
                        sb.Append(TAB).Append(TAB).Append(string.Format("packMemberList.Add(DefaultVar.{0});", tempType)).Append(LINK);
                    }
                    sb.Append(TAB).Append(TAB).Append(string.Format("ProtocolManager.Instance.AddProtocolPackMemberListToDict((ProtocolNo){0}, packMemberList);", protocolStruct.protocolID)).Append(LINK);
                    sb.Append(TAB).Append(RIGHT);
                    sb.Append(LINK);
                    sb.Append(TAB).Append("public override void SetData(List<object> objectList)").Append(LINK);
                    sb.Append(TAB).Append(LEFT);
                    sb.Append(TAB).Append(TAB).Append("int index = 0;").Append(LINK);
                    for (int i = 0; i < protocolStruct.unPackMemberArr.Length; i++)
                    {
                        if (protocolStruct.unPackMemberArr[i].val_type == "int" || protocolStruct.unPackMemberArr[i].val_type == "string"
                            || protocolStruct.unPackMemberArr[i].val_type == "float" || protocolStruct.unPackMemberArr[i].val_type == "double"
                            || protocolStruct.unPackMemberArr[i].val_type == "char")
                        {
                            sb.Append(TAB).Append(TAB).Append(string.Format("{0} = ({1})objectList[index];", protocolStruct.unPackMemberArr[i].val_name, protocolStruct.unPackMemberArr[i].val_type)).Append(LINK);
                            sb.Append(TAB).Append(TAB).Append("index += 1;").Append(LINK);
                        }
                        else
                        {
                            sb.Append(TAB).Append(TAB).Append(string.Format("{0} = {1}.ToClass(objectList, ref index);", protocolStruct.unPackMemberArr[i].val_name, protocolStruct.unPackMemberArr[i].val_type)).Append(LINK);
                        }
                    }
                    sb.Append(TAB).Append(RIGHT);
                    sb.Append(LINK);
                    sb.Append(TAB).Append("public override object Clone()").Append(LINK);
                    sb.Append(TAB).Append(LEFT);
                    sb.Append(TAB).Append(TAB).Append("return MemberwiseClone();").Append(LINK);
                    sb.Append(TAB).Append(RIGHT);
                    sb.Append(RIGHT);
                    File.WriteAllText(fullPath, sb.ToString());
                    sb = null;
                    streamReader.Close();
                    fileStream.Close();
                    streamReader.Dispose();
                    fileStream.Dispose();
                    streamReader = null;
                    fileStream = null;
                }
            }
            EditorUtility.DisplayProgressBar("自动生成协议类脚本", "已完成!", 1);
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError(e.ToString());
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //[MenuItem("Tools/Editor/Config/客户端/自动生成InitSystem")]
    public static void ParseInitSystem()
    {
        string path = Application.dataPath + "/Script/Protocol/AutoInit/InitSystem.cs";
        string protocolDataJsonPath = Application.dataPath + "/Script/Editor/Config/ProtocolStruct";
        string classStructUnPackJsonPath = Application.dataPath + "/Script/Editor/Config/ClassStruct/UnPack";
        if (!Directory.Exists(protocolDataJsonPath))
        {
            Debug.LogError("存放协议类结构的Json目录不存在!" + protocolDataJsonPath);
        }
        if (!Directory.Exists(classStructUnPackJsonPath))
        {
            Debug.LogError("存放协议相关数据类结构的Json目录不存在!" + classStructUnPackJsonPath);
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("using System.Collections.Generic;").Append(LINK);
        sb.Append("namespace Assets.Script.Protocol").Append(LINK);
        sb.Append(LEFT);
        sb.Append(TAB).Append("public static class InitSystem").Append(LINK);
        sb.Append(TAB).Append(LEFT);
        sb.Append(TAB).Append(TAB).Append("public static void Init(ProtocolManager protocolManager)").Append(LINK);
        sb.Append(TAB).Append(TAB).Append(LEFT);

        string[] protocolDataFilePaths = Directory.GetFiles(protocolDataJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < protocolDataFilePaths.Length; i++)
        {
            string tempPath = protocolDataFilePaths[i];
            tempPath = tempPath.Substring(tempPath.LastIndexOf("\\") + 1);
            tempPath = tempPath.Substring(0, tempPath.Length - 5);//去掉后缀.json            
            string[] arr = tempPath.Split('_');
            sb.Append(TAB).Append(TAB).Append(TAB).Append(string.Format("protocolManager.AddProtocolObjectToDict((ProtocolNo){0}, new {1}());", arr[0], arr[1])).Append(LINK);
        }
        string[] classStructFilePaths = Directory.GetFiles(classStructUnPackJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < classStructFilePaths.Length; i++)
        {
            FileStream fileStream = File.OpenRead(classStructFilePaths[i]);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string text = streamReader.ReadToEnd();
            if (!string.IsNullOrEmpty(text))
            {
                ClassStruct classStruct = JsonUtility.FromJson<ClassStruct>(text);
                StringBuilder tempSb = new StringBuilder();
                for (int j = 0; j < classStruct.memberDataList.Length; j++)
                {
                    string tempType = "";
                    MemberData memberData = classStruct.memberDataList[j];
                    switch (memberData.val_type)
                    {
                        case "int":
                            tempType = "INT32";
                            break;
                        case "string":
                            tempType = "STRING";
                            break;
                        case "float":
                            tempType = "FLOAT";
                            break;
                        case "double":
                            tempType = "DOUBLE";
                            break;
                        case "char":
                            tempType = "CHAR";
                            break;
                        default:
                            tempType = "CLASS";
                            break;
                    }
                    tempSb.Append(string.Format("DefaultVar.{0}", tempType));
                    if (j != classStruct.memberDataList.Length - 1)
                    {
                        tempSb.Append(",");
                    }
                }
                sb.Append(TAB).Append(TAB).Append(TAB).Append(string.Format("protocolManager.AddUnPackClassStructToDict({0}.DATAID, new List<object> {2} {1} {3});", classStruct.className, tempSb.ToString(), "{", "}")).Append(LINK);
            }
            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();
            streamReader = null;
            fileStream = null;
        }
        sb.Append(TAB).Append(TAB).Append(RIGHT);
        sb.Append(TAB).Append(RIGHT);
        sb.Append(RIGHT);
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //[MenuItem("Tools/Editor/Config/客户端/自动生成相关枚举")]
    public static void ParseEnum()
    {
        string savePath = Application.dataPath + "/Script/Protocol/AutoEnum/CommonEnum.cs";
        string protocolDataJsonPath = Application.dataPath + "/Script/Editor/Config/ProtocolStruct";
        string classStructJsonPath = Application.dataPath + "/Script/Editor/Config/ClassStruct";
        string[] protocolFilePaths = Directory.GetFiles(protocolDataJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        StringBuilder sb = new StringBuilder();
        sb.Append("public enum ProtocolNo").Append(LINK);
        sb.Append(LEFT);
        for (int i = 0; i < protocolFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(protocolFilePaths[i]);
            string[] arr = file.Name.Split('_');
            arr[1] = arr[1].Substring(0, arr[1].Length - 5);
            sb.Append(TAB).Append(arr[1] + " = " + arr[0] + ",").Append(LINK);
        }
        sb.Append(RIGHT);

        sb.Append("public enum ProtocolEventType").Append(LINK);
        sb.Append(LEFT);
        sb.Append(TAB).Append("Null,").Append(LINK);
        for (int i = 0; i < protocolFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(protocolFilePaths[i]);
            string[] arr = file.Name.Split('_');
            arr[1] = arr[1].Substring(0, arr[1].Length - 5);
            sb.Append(TAB).Append(arr[1] + " = " + arr[0] + ",").Append(LINK);
        }
        sb.Append(RIGHT);

        string[] classStructFilePaths = Directory.GetFiles(classStructJsonPath, "*.json", SearchOption.AllDirectories);
        sb.Append("public enum DataID").Append(LINK);
        sb.Append(LEFT);
        for (int i = 0; i < classStructFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(classStructFilePaths[i]);
            string[] arr = file.Name.Split('_');
            arr[1] = arr[1].Substring(0, arr[1].Length - 5);
            sb.Append(TAB).Append(arr[1] + " = " + arr[0] + ",").Append(LINK);
        }
        sb.Append(RIGHT);
        File.WriteAllText(savePath, sb.ToString());
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Editor/Config/自动生成服务器协议脚本")]
    public static void ParseServer()
    {
        ParseAllClassServer();
        ParseAllProtocolServer();
        ParseInitSystemServer();
        ParseEnumServer();
        ParseHandlerServer();
    }

    //[MenuItem("Tools/Editor/Config/服务端/自动生成协议相关数据类脚本")]
    public static void ParseAllClassServer()
    {
        if (!Directory.Exists(Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct"))
        {
            Debug.LogError(Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct is not exist!");
            return;
        }
        //string path = Application.dataPath + "/Script/Protocol/AutoClassStruct/";
        string path = ServerPath + "/MilkGameServer/Server/Protocol/ClassStruct/";
        string[] files = Directory.GetFiles(Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct", "*.json", SearchOption.AllDirectories);
        for (int index = 0; index < files.Length; index++)
        {
            FileStream fileStream = File.OpenRead(files[index]);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string text = streamReader.ReadToEnd();
            //Debug.Log(text);
            if (text != null)
            {
                ClassStruct classStruct = JsonUtility.FromJson<ClassStruct>(text);
                StringBuilder sb = new StringBuilder();
                StringBuilder memberSb = new StringBuilder();
                StringBuilder initSb = new StringBuilder();
                string str = "";
                switch (classStruct.structType)
                {
                    case "Pack":
                        sb.Append(string.Format("public class {0} : ClassBase", classStruct.className)).Append(LINK);
                        sb.Append(LEFT);
                        initSb.Append(TAB).Append(TAB).Append(string.Format("AddMemberObject((DataID){0});", classStruct.dataID)).Append(LINK);
                        foreach (var v in classStruct.memberDataList)
                        {
                            sb.Append(TAB).Append("//" + v.val_describe).Append(LINK);
                            sb.Append(TAB).Append(string.Format("public {0} {1};", v.val_type, v.val_name)).Append(LINK);
                            memberSb.Append(v.val_type + " " + v.val_name + ",");
                            initSb.Append(TAB).Append(TAB).Append(string.Format("this.{0} = {1};", v.val_name, v.val_name)).Append(LINK);
                            initSb.Append(TAB).Append(TAB).Append(string.Format("AddMemberObject(this.{0});", v.val_name)).Append(LINK);
                        }
                        str = memberSb.ToString();
                        if (str.Length > 0)
                        {
                            str = str.Substring(0, str.Length - 1);
                        }
                        sb.Append(TAB).Append(string.Format("public {0}({1})", classStruct.className, str)).Append(LINK);
                        sb.Append(TAB).Append(LEFT);
                        sb.Append(initSb.ToString());
                        sb.Append(TAB).Append(RIGHT);
                        sb.Append(RIGHT);
                        File.WriteAllText(path + classStruct.className + ".cs", sb.ToString());
                        break;
                    case "UnPack":
                        sb.Append("using System.Collections.Generic;").Append(LINK);
                        sb.Append(string.Format("public class {0}", classStruct.className)).Append(LINK);
                        sb.Append(LEFT);
                        sb.Append(TAB).Append(string.Format("public const DataID DATAID = (DataID){0};", classStruct.dataID)).Append(LINK);
                        foreach (var v in classStruct.memberDataList)
                        {
                            sb.Append(TAB).Append("//" + v.val_describe).Append(LINK);
                            sb.Append(TAB).Append(string.Format("public {0} {1};", v.val_type, v.val_name)).Append(LINK);
                            memberSb.Append(v.val_type + " " + v.val_name + ",");
                            initSb.Append(TAB).Append(TAB).Append(string.Format("this.{0} = {1};", v.val_name, v.val_name)).Append(LINK);
                        }
                        str = memberSb.ToString();
                        if (str.Length > 0)
                        {
                            str = str.Substring(0, str.Length - 1);
                        }
                        sb.Append(TAB).Append(string.Format("public {0}({1})", classStruct.className, str)).Append(LINK);
                        sb.Append(TAB).Append(LEFT);
                        sb.Append(initSb.ToString());
                        sb.Append(TAB).Append(RIGHT);
                        sb.Append(TAB).Append(string.Format("public static {0} ToClass(List<object> objectList, ref int index)", classStruct.className)).Append(LINK);
                        sb.Append(TAB).Append(LEFT);
                        //sb.Append(TAB).Append(TAB).Append(string.Format("index += {0};", classStruct.memberDataList.Length)).Append(LINK);
                        StringBuilder memberListSb = new StringBuilder();
                        for (int i = 0; i < classStruct.memberDataList.Length; i++)
                        {
                            if (classStruct.memberDataList[i].val_type != "int" && classStruct.memberDataList[i].val_type != "float"
                                && classStruct.memberDataList[i].val_type != "double" && classStruct.memberDataList[i].val_type != "chat"
                                && classStruct.memberDataList[i].val_type != "string")
                            {
                                memberListSb.Append(classStruct.memberDataList[i].val_type + ".ToClass(objectList, ref index)");
                            }
                            else
                            {
                                memberListSb.Append(string.Format("({0})objectList[index++]", classStruct.memberDataList[i].val_type));
                            }
                            if (i != classStruct.memberDataList.Length - 1)
                            {
                                memberListSb.Append(", ");
                            }
                        }
                        sb.Append(TAB).Append(TAB).Append(string.Format("return new {0}({1});", classStruct.className, memberListSb.ToString())).Append(LINK);
                        sb.Append(TAB).Append(RIGHT);
                        sb.Append(RIGHT);
                        File.WriteAllText(path + classStruct.className + ".cs", sb.ToString());
                        break;
                }
                streamReader.Close();
                fileStream.Close();
                streamReader.Dispose();
                fileStream.Dispose();
                streamReader = null;
                fileStream = null;
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //[MenuItem("Tools/Editor/Config/服务端/自动生成协议类脚本")]
    public static void ParseAllProtocolServer()
    {
        //string path = Application.dataPath + "/Script/Protocol/AutoProtocolStruct/";
        string path = ServerPath + "/MilkGameServer/Server/Protocol/AutoProtocolStruct/";
        float time = 0;
        try
        {
            EditorUtility.DisplayProgressBar("自动生成协议类脚本", "开始自动生成", 0);
            if (!Directory.Exists(Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct"))
            {
                Debug.LogError(Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct is not exist!");
                return;
            }
            string[] files = Directory.GetFiles(Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct", "*.json", SearchOption.TopDirectoryOnly);
            for (int index = 0; index < files.Length; index++)
            {
                string str = "";
                time += Time.deltaTime * 3;
                for (int k = 0; k < time % 7; k++)
                {
                    str += ".";
                }
                EditorUtility.DisplayProgressBar("自动生成协议类脚本", "正在进行中..." + str, (float)index / files.Length);
                FileStream fileStream = File.OpenRead(files[index]);
                StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    ProtocolStruct protocolStruct = JsonUtility.FromJson<ProtocolStruct>(text);
                    string fullPath = path + protocolStruct.protocolName + ".cs";
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    //FileStream fs = File.Open(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("using System;").Append(LINK);
                    sb.Append("using System.Collections;").Append(LINK);
                    sb.Append("using System.Collections.Generic;").Append(LINK);
                    sb.Append("using GameServer.Protocol.Data;").Append(LINK);
                    sb.Append("using GameServer.Protocol;").Append(LINK);
                    sb.Append(string.Format("public class {0} : ProtocolData, ICloneable", protocolStruct.protocolName)).Append(LINK);
                    sb.Append(LEFT);
                    StringBuilder classMemberSb = new StringBuilder();
                    classMemberSb.Append(TAB).Append(TAB).Append("List<object> unPackMemberList = new List<object>();").Append(LINK);
                    for (int i = 0; i < protocolStruct.unPackMemberArr.Length; i++)
                    {
                        string tempType = "";
                        switch (protocolStruct.unPackMemberArr[i].val_type)
                        {
                            case "int":
                                tempType = "INT32";
                                break;
                            case "string":
                                tempType = "STRING";
                                break;
                            case "float":
                                tempType = "FLOAT";
                                break;
                            case "double":
                                tempType = "DOUBLE";
                                break;
                            case "char":
                                tempType = "CHAR";
                                break;
                            default:
                                tempType = "CLASS";
                                break;
                        }
                        sb.Append(TAB).Append("//" + protocolStruct.unPackMemberArr[i].val_describe).Append(LINK);
                        sb.Append(TAB).Append(string.Format("public {0} {1};", protocolStruct.unPackMemberArr[i].val_type, protocolStruct.unPackMemberArr[i].val_name)).Append(LINK);
                        classMemberSb.Append(TAB).Append(TAB).Append(string.Format("unPackMemberList.Add(DefaultVar.{0});", tempType)).Append(LINK);
                    }
                    classMemberSb.Append(TAB).Append(TAB).Append(string.Format("ProtocolMgr.Instance.AddProtocolUnPackMemberListToDict((ProtocolNo){0}, unPackMemberList);", protocolStruct.protocolID)).Append(LINK);
                    sb.Append(TAB).Append(string.Format("public {0} ()", protocolStruct.protocolName)).Append(LINK);
                    sb.Append(TAB).Append(LEFT);
                    sb.Append(classMemberSb.ToString()).Append(LINK);
                    sb.Append(TAB).Append(TAB).Append("List<object> packMemberList = new List<object>();").Append(LINK);
                    for (int i = 0; i < protocolStruct.packMemberArr.Length; i++)
                    {
                        string tempType = "";
                        switch (protocolStruct.packMemberArr[i].val_type)
                        {
                            case "int":
                                tempType = "INT32";
                                break;
                            case "string":
                                tempType = "STRING";
                                break;
                            case "float":
                                tempType = "FLOAT";
                                break;
                            case "double":
                                tempType = "DOUBLE";
                                break;
                            case "char":
                                tempType = "CHAR";
                                break;
                            default:
                                tempType = "CLASS";
                                break;
                        }
                        sb.Append(TAB).Append(TAB).Append(string.Format("packMemberList.Add(DefaultVar.{0});", tempType)).Append(LINK);
                    }
                    sb.Append(TAB).Append(TAB).Append(string.Format("ProtocolMgr.Instance.AddProtocolPackMemberListToDict((ProtocolNo){0}, packMemberList);", protocolStruct.protocolID)).Append(LINK);
                    sb.Append(TAB).Append(RIGHT);
                    sb.Append(LINK);
                    sb.Append(TAB).Append("public override void SetData(List<object> objectList)").Append(LINK);
                    sb.Append(TAB).Append(LEFT);
                    sb.Append(TAB).Append(TAB).Append("int index = 0;").Append(LINK);
                    for (int i = 0; i < protocolStruct.unPackMemberArr.Length; i++)
                    {
                        if (protocolStruct.unPackMemberArr[i].val_type == "int" || protocolStruct.unPackMemberArr[i].val_type == "string"
                            || protocolStruct.unPackMemberArr[i].val_type == "float" || protocolStruct.unPackMemberArr[i].val_type == "double"
                            || protocolStruct.unPackMemberArr[i].val_type == "char")
                        {
                            sb.Append(TAB).Append(TAB).Append(string.Format("{0} = ({1})objectList[index];", protocolStruct.unPackMemberArr[i].val_name, protocolStruct.unPackMemberArr[i].val_type)).Append(LINK);
                            sb.Append(TAB).Append(TAB).Append("index += 1;").Append(LINK);
                        }
                        else
                        {
                            sb.Append(TAB).Append(TAB).Append(string.Format("{0} = {1}.ToClass(objectList, ref index);", protocolStruct.unPackMemberArr[i].val_name, protocolStruct.unPackMemberArr[i].val_type)).Append(LINK);
                        }
                    }
                    sb.Append(TAB).Append(RIGHT);
                    sb.Append(LINK);
                    sb.Append(TAB).Append("public override object Clone()").Append(LINK);
                    sb.Append(TAB).Append(LEFT);
                    sb.Append(TAB).Append(TAB).Append("return MemberwiseClone();").Append(LINK);
                    sb.Append(TAB).Append(RIGHT);
                    sb.Append(RIGHT);
                    File.WriteAllText(fullPath, sb.ToString());
                    sb = null;
                    streamReader.Close();
                    fileStream.Close();
                    streamReader.Dispose();
                    fileStream.Dispose();
                    streamReader = null;
                    fileStream = null;
                }
            }
            EditorUtility.DisplayProgressBar("自动生成协议类脚本", "已完成!", 1);
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError(e.ToString());
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //[MenuItem("Tools/Editor/Config/服务端/自动生成InitSystem")]
    public static void ParseInitSystemServer()
    {
        //string path = Application.dataPath + "/Script/Protocol/AutoInit/InitSystem.cs";
        string path = ServerPath + "/MilkGameServer/Server/Protocol/AutoInit/InitSystem.cs";
        string protocolDataJsonPath = Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct";
        string classStructUnPackJsonPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct/UnPack";
        if (!Directory.Exists(protocolDataJsonPath))
        {
            Debug.LogError("存放协议类结构的Json目录不存在!" + protocolDataJsonPath);
        }
        if (!Directory.Exists(classStructUnPackJsonPath))
        {
            Debug.LogError("存放协议相关数据类结构的Json目录不存在!" + classStructUnPackJsonPath);
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        StringBuilder sb = new StringBuilder();
        sb.Append("using System.Collections.Generic;").Append(LINK);
        sb.Append("using GameServer.Handle;").Append(LINK);
        sb.Append("namespace Assets.Script.Protocol").Append(LINK);
        sb.Append(LEFT);
        sb.Append(TAB).Append("public static class InitSystem").Append(LINK);
        sb.Append(TAB).Append(LEFT);
        sb.Append(TAB).Append(TAB).Append("public static void Init(GameServer.Protocol.ProtocolMgr protocolManager)").Append(LINK);
        sb.Append(TAB).Append(TAB).Append(LEFT);

        string[] protocolDataFilePaths = Directory.GetFiles(protocolDataJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < protocolDataFilePaths.Length; i++)
        {
            string tempPath = protocolDataFilePaths[i];
            tempPath = tempPath.Substring(tempPath.LastIndexOf("\\") + 1);
            tempPath = tempPath.Substring(0, tempPath.Length - 5);//去掉后缀.json            
            string[] arr = tempPath.Split('_');
            sb.Append(TAB).Append(TAB).Append(TAB).Append(string.Format("protocolManager.AddProtocolObjectToDict((ProtocolNo){0}, new {1}());", arr[0], arr[1])).Append(LINK);
            sb.Append(TAB).Append(TAB).Append(TAB).Append(string.Format("new {0}Handler();", arr[1])).Append(LINK);
        }
        string[] classStructFilePaths = Directory.GetFiles(classStructUnPackJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < classStructFilePaths.Length; i++)
        {
            FileStream fileStream = File.OpenRead(classStructFilePaths[i]);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string text = streamReader.ReadToEnd();
            if (!string.IsNullOrEmpty(text))
            {
                ClassStruct classStruct = JsonUtility.FromJson<ClassStruct>(text);
                StringBuilder tempSb = new StringBuilder();
                for (int j = 0; j < classStruct.memberDataList.Length; j++)
                {
                    string tempType = "";
                    MemberData memberData = classStruct.memberDataList[j];
                    switch (memberData.val_type)
                    {
                        case "int":
                            tempType = "INT32";
                            break;
                        case "string":
                            tempType = "STRING";
                            break;
                        case "float":
                            tempType = "FLOAT";
                            break;
                        case "double":
                            tempType = "DOUBLE";
                            break;
                        case "char":
                            tempType = "CHAR";
                            break;
                        default:
                            tempType = "CLASS";
                            break;
                    }
                    tempSb.Append(string.Format("DefaultVar.{0}", tempType));
                    if (j != classStruct.memberDataList.Length - 1)
                    {
                        tempSb.Append(",");
                    }
                }
                sb.Append(TAB).Append(TAB).Append(TAB).Append(string.Format("protocolManager.AddUnPackClassStructToDict({0}.DATAID, new List<object> {2} {1} {3});", classStruct.className, tempSb.ToString(), "{", "}")).Append(LINK);
            }
            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();
            streamReader = null;
            fileStream = null;
        }
        sb.Append(TAB).Append(TAB).Append(RIGHT);
        sb.Append(TAB).Append(RIGHT);
        sb.Append(RIGHT);
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //[MenuItem("Tools/Editor/Config/服务端/自动生成相关枚举")]
    public static void ParseEnumServer()
    {
        //string savePath = Application.dataPath + "/Script/Protocol/AutoEnum/CommonEnum.cs";
        string savePath = ServerPath + "/MilkGameServer/Server/Protocol/AutoEnum/CommonEnum.cs";
        string protocolDataJsonPath = Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct";
        string classStructJsonPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct";
        string[] protocolFilePaths = Directory.GetFiles(protocolDataJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        StringBuilder sb = new StringBuilder();
        sb.Append("public enum ProtocolNo").Append(LINK);
        sb.Append(LEFT);
        for (int i = 0; i < protocolFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(protocolFilePaths[i]);
            string[] arr = file.Name.Split('_');
            arr[1] = arr[1].Substring(0, arr[1].Length - 5);
            sb.Append(TAB).Append(arr[1] + " = " + arr[0] + ",").Append(LINK);
        }
        sb.Append(RIGHT);

        sb.Append("public enum ProtocolEventType").Append(LINK);
        sb.Append(LEFT);
        sb.Append(TAB).Append("Null,").Append(LINK);
        for (int i = 0; i < protocolFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(protocolFilePaths[i]);
            string[] arr = file.Name.Split('_');
            arr[1] = arr[1].Substring(0, arr[1].Length - 5);
            sb.Append(TAB).Append(arr[1] + " = " + arr[0] + ",").Append(LINK);
        }
        sb.Append(RIGHT);

        string[] classStructFilePaths = Directory.GetFiles(classStructJsonPath, "*.json", SearchOption.AllDirectories);
        sb.Append("public enum DataID").Append(LINK);
        sb.Append(LEFT);
        for (int i = 0; i < classStructFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(classStructFilePaths[i]);
            string[] arr = file.Name.Split('_');
            arr[1] = arr[1].Substring(0, arr[1].Length - 5);
            sb.Append(TAB).Append(arr[1] + " = " + arr[0] + ",").Append(LINK);
        }
        sb.Append(RIGHT);
        File.WriteAllText(savePath, sb.ToString());
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    //自动生成协议事件处理器Handler
    public static void ParseHandlerServer()
    {
        string protocolDataJsonPath = Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct";
        string[] protocolFilePaths = Directory.GetFiles(protocolDataJsonPath, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < protocolFilePaths.Length; i++)
        {
            FileInfo file = new FileInfo(protocolFilePaths[i]);
            string[] arr = file.Name.Split('_');
            string protocolName = arr[1].Substring(0, arr[1].Length - 5);
            string handlerName = arr[1].Substring(0, arr[1].Length - 5) + "Handler";
            string savePath = ServerPath + "/MilkGameServer/Server/Handle/" + handlerName + ".cs";
            if (!File.Exists(savePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("using GameServer.Protocol;").Append(LINK);
                sb.Append("using GameServer.SocketFile;").Append(LINK);
                sb.Append("using System;").Append(LINK);
                sb.Append("namespace GameServer.Handle").Append(LINK);
                sb.Append(LEFT);
                sb.Append(TAB).Append("class " + handlerName).Append(LINK);
                sb.Append(TAB).Append(LEFT);
                sb.Append(TAB).Append(TAB).Append("public " + handlerName + "()").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(LEFT);
                sb.Append(TAB).Append(TAB).Append(TAB).Append("EventSystem.Instance.AddListener(ProtocolEventType." + protocolName + ", OnRequest);").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(RIGHT);
                sb.Append(TAB).Append(TAB).Append("~" + handlerName + "()").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(LEFT);
                sb.Append(TAB).Append(TAB).Append(TAB).Append("RemoveAllListener();").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(RIGHT);
                sb.Append(TAB).Append(TAB).Append("private void OnRequest(object data, Client client)").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(LEFT);
                sb.Append(TAB).Append(TAB).Append(TAB).Append(protocolName + " tempData = (" + protocolName + ")data;").Append(LINK);
                sb.Append(LINK);
                sb.Append(LINK);
                sb.Append(TAB).Append(TAB).Append(RIGHT);
                sb.Append(TAB).Append(TAB).Append("public void RemoveAllListener()").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(LEFT);
                sb.Append(TAB).Append(TAB).Append(TAB).Append("if (EventSystem.Instance != null)").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(TAB).Append(LEFT);
                sb.Append(TAB).Append(TAB).Append(TAB).Append(TAB).Append("EventSystem.Instance.RemoveListener(ProtocolEventType." + protocolName + ", OnRequest);").Append(LINK);
                sb.Append(TAB).Append(TAB).Append(TAB).Append(RIGHT);
                sb.Append(TAB).Append(TAB).Append(RIGHT);
                sb.Append(TAB).Append(RIGHT);
                sb.Append(RIGHT);
                File.WriteAllText(savePath, sb.ToString());
            }
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
}

[Serializable]
public class ProtocolStruct
{
    public string protocolID;
    public string protocolName;
    //public string[] packMemberArr;//只保存发包类型(int,float,double,string,char,class)
    public MemberData[] packMemberArr;//为了可视化，而做了这个..实际上关键的是val_type，其他的都可以不进行安全监测
    public MemberData[] unPackMemberArr;
}

[Serializable]
public class ClassStruct
{
    public string dataID;
    public string structType;
    public string className;
    public MemberData[] memberDataList;
}

[Serializable]
public class MemberData
{
    public string val_describe;
    public string val_name;
    public string val_type;
}