using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;

public enum WindowType
{
    ClassStruct,
    Protocol
}
public class ProtocolWindow : EditorWindow
{
    private Color COLOR_BLACK = new Color(49 / 255, 49 / 255, 49 / 255, 1);
    private Color COLOR_GRAY = new Color(100 / 255, 100 / 255, 100 / 255, 1);
    [MenuItem("Tools/Editor/ProtocolWindow")]
    public static void Window()
    {
        EditorWindow window = GetWindowWithRect(typeof(ProtocolWindow), new Rect(Screen.width, Screen.height - 200, 1000, 630), false);
        window.title = "协议可视化窗口";
        window.maxSize = new Vector2(1000, 630);
        window.minSize = new Vector2(1000, 630);
        window.Show();
    }

    #region ClassStruct相关变量
    Vector2 memberScrollPos = Vector2.zero;
    Vector2 protocolScrollPos = Vector2.zero;
    ClassStruct classStruct = new ClassStruct();
    int memberListLength = 0;
    int structTypeIndex = 0;
    string[] structTypeArr = { "UnPack", "Pack" };
    int H = 2;
    bool isPack = false;
    string msg = "";
    bool classNameIsValid;
    bool memberDataArrIsValid;
    bool dataIDValid;
    #endregion

    #region ProtocolStruct相关变量
    Vector2 memberScrollPos1 = Vector2.zero;
    Vector2 protocolScrollPos1 = Vector2.zero;
    Vector2 memberPackScrollPos1 = Vector2.zero;
    Vector2 packMemberTypeScrollPos = Vector2.zero;
    ProtocolStruct protocolStruct = new ProtocolStruct();
    string[] packMemberTypeArr = { "int", "float", "string", "double", "char", "class" };
    List<string> packMemberTypeList = new List<string> { "int", "float", "string", "double", "char", "class" };
    //int[] packMemberSelectedIndexArr;
    //int packMemberTypeLength = 0;    
    int memberListLength1 = 0;
    int packemberListLength = 0;
    bool protocolNameIsValid;
    bool protocolIDValid;
    #endregion

    WindowType wType = WindowType.ClassStruct;

    private void OnGUI()
    {
        string ClassStructPath = Application.dataPath + "/Script/Editor/Config/ClassStruct";
        string ClassStructPackPath = Application.dataPath + "/Script/Editor/Config/ClassStruct/Pack";
        string ClassStructUnPackPath = Application.dataPath + "/Script/Editor/Config/ClassStruct/UnPack";
        string ProtocolStructPath = Application.dataPath + "/Script/Editor/Config/ProtocolStruct";
        string ServerClassStructPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct";
        string ServerClassStructPackPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct/Pack";
        string ServerClassStructUnPackPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct/UnPack";
        string ServerProtocolStructPath = Application.dataPath + "/Script/Editor/ServerConfig/ProtocolStruct";
        GUIStyle lefBtnStyle = new GUIStyle("button");
        lefBtnStyle.wordWrap = true;
        if (GUI.Button(new Rect(0, 0, 48, 70), "ClassStruct", lefBtnStyle))
        {
            wType = WindowType.ClassStruct;
        }
        if (GUI.Button(new Rect(0, 80, 48, 70), "Protocol", lefBtnStyle))
        {
            wType = WindowType.Protocol;
        }
        if (wType == WindowType.ClassStruct)
        {
            #region ClassStruct可视化编辑窗口
            classNameIsValid = true;
            memberDataArrIsValid = true;
            dataIDValid = true;
            #region GUIStyle(CSS)
            //成员列表项区域样式
            GUIStyle memberStyle = new GUIStyle();
            memberStyle.fontStyle = FontStyle.BoldAndItalic;
            //顶层区域样式
            GUIStyle topStyle = new GUIStyle();
            topStyle.margin = new RectOffset(50, 50, 10, 10);
            topStyle.padding = new RectOffset(20, 20, 20, 20);
            //普通Label样式
            GUIStyle labelStyle = new GUIStyle("Label");
            labelStyle.fontStyle = FontStyle.Bold;
            //添加成员btn样式
            GUIStyle addMemberBtnStyle = new GUIStyle("button");
            addMemberBtnStyle.fontStyle = FontStyle.Bold;
            addMemberBtnStyle.border = new RectOffset(10, 10, 10, 10);
            addMemberBtnStyle.padding = new RectOffset(10, 10, 10, 10);
            //普通按钮文本样式
            GUIStyle btstyle = new GUIStyle("button");
            btstyle.alignment = TextAnchor.MiddleLeft;
            #endregion

            Rect topBorderRect = new Rect(48, 8, 904f, 104f);
            EditorGUI.DrawRect(topBorderRect, Color.black);//最顶层区域的外边框
            Rect topRect = new Rect(50, 10, 900f, 100f);//最顶层的区域       
            EditorGUI.DrawRect(topRect, Color.white);//最顶层区域的外边框

            GUILayout.BeginArea(topRect, topStyle);
            {
                H = 2;

                Rect dataIDAreaRect = new Rect(10, H, 800, 25);
                GUILayout.BeginArea(dataIDAreaRect);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("DataID", labelStyle, GUILayout.Width(100));
                        classStruct.dataID = EditorGUILayout.TextField(classStruct.dataID, GUILayout.Width(100));
                        dataIDValid = CheckDataID(classStruct.dataID, out msg);
                        GUIStyle msgStyle = new GUIStyle();
                        msgStyle.normal.textColor = dataIDValid ? Color.black : Color.red;
                        msgStyle.fontStyle = FontStyle.Bold;
                        msgStyle.fontSize = 13;
                        EditorGUILayout.LabelField(msg, msgStyle, GUILayout.Width(200));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndArea();

                H += 25;
                //StructType
                Rect structTypeAreaRect = new Rect(10, H, 800, 40);
                GUILayout.BeginArea(structTypeAreaRect);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("StructType", labelStyle, GUILayout.Width(100));
                        GUIStyle selectionStyle = new GUIStyle("textfield");
                        GUIStyle buttonStyle = new GUIStyle("button");
                        selectionStyle.active = buttonStyle.active;
                        selectionStyle.onNormal = buttonStyle.onNormal;
                        selectionStyle.fixedWidth = 100;
                        structTypeIndex = GUILayout.SelectionGrid(structTypeIndex, structTypeArr, 1, selectionStyle);
                        classStruct.structType = structTypeArr[structTypeIndex];
                        isPack = classStruct.structType == "Pack";
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndArea();

                H += 40;

                Rect classNameAreaRect = new Rect(10, H, 800, 25);
                GUILayout.BeginArea(classNameAreaRect);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("ClassName", labelStyle, GUILayout.Width(100));
                        classStruct.className = EditorGUILayout.TextField(classStruct.className, GUILayout.Width(100));
                        classNameIsValid = CheckClassNameValid(classStruct.className, isPack, out msg);
                        GUIStyle msgStyle = new GUIStyle();
                        msgStyle.normal.textColor = classNameIsValid ? Color.black : Color.red;
                        msgStyle.fontStyle = FontStyle.Bold;
                        msgStyle.fontSize = 13;
                        EditorGUILayout.LabelField(msg, msgStyle, GUILayout.Width(200));
                        GUILayout.Space(10);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(490, 2, 410, 90));
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Clear This ClassStruct", addMemberBtnStyle, GUILayout.Width(200)))
                    {
                        classStruct = new ClassStruct();
                        memberListLength = 0;
                    }
                    if (!string.IsNullOrEmpty(classStruct.className))
                    {
                        if (GUILayout.Button("Delete This ClassStruct", addMemberBtnStyle, GUILayout.Width(200)))
                        {
                            if (MilkMessage.MessageBox(IntPtr.Zero, "Delete This ClassStruct:" + classStruct.className, "提示", 1) == 1)
                            {
                                //生成客户端对应的ClassStruct相关json文件                    
                                string savePath = ClassStructPath + "/" + classStruct.structType + "/" + classStruct.dataID + "_" + classStruct.className + ".json";
                                if (File.Exists(savePath))
                                {
                                    File.Delete(savePath);
                                }
                                //反转信息然后生成服务器对应的ClassStruct相关json文件
                                if (classStruct.structType == "UnPack")
                                {
                                    classStruct.structType = "Pack";
                                }
                                else
                                {
                                    classStruct.structType = "UnPack";
                                }
                                savePath = ServerClassStructPath + "/" + classStruct.structType + "/" + classStruct.dataID + "_" + classStruct.className + ".json";
                                if (File.Exists(savePath))
                                {
                                    File.Delete(savePath);
                                }
                                classStruct = new ClassStruct();
                                memberListLength = 0;
                                AssetDatabase.Refresh();
                                AssetDatabase.SaveAssets();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
                H += 25;
            }
            GUILayout.EndArea();

            EditorGUI.DrawRect(new Rect(48, 109, 904, 304f), Color.black);
            EditorGUI.DrawRect(new Rect(50, 111, 900f, 300f), Color.white);
            Rect centerRect = new Rect(50, 111, 900f, 300f);
            GUILayout.BeginArea(centerRect);
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add MemberData", addMemberBtnStyle, GUILayout.Width(200)))
                {
                    memberListLength++;
                }
                if (GUILayout.Button("Clear All MemberData", addMemberBtnStyle, GUILayout.Width(200)))
                {
                    classStruct.memberDataList = null;
                    memberListLength = 0;
                }
                GUILayout.EndHorizontal();

                if (memberListLength > 0)
                {
                    if (classStruct.memberDataList == null || (classStruct.memberDataList != null && classStruct.memberDataList.Length != memberListLength))
                    {
                        if (classStruct.memberDataList != null)
                        {
                            MemberData[] tempMemberData = new MemberData[classStruct.memberDataList.Length];
                            tempMemberData = classStruct.memberDataList;
                            MemberData[] mdarr = new MemberData[memberListLength];
                            for (int i = 0; i < tempMemberData.Length; i++)
                            {
                                mdarr[i] = tempMemberData[i];
                            }
                            classStruct.memberDataList = mdarr;
                            memberListLength = classStruct.memberDataList.Length;
                        }
                        else
                        {
                            classStruct.memberDataList = new MemberData[memberListLength];
                        }
                    }
                    memberScrollPos = GUI.BeginScrollView(new Rect(0, 35, 900, 8 * 24), memberScrollPos, new Rect(0, 35, 900, 24 * (memberListLength)), false, false);
                    {
                        H = 35;
                        List<int> removeIndex = new List<int>();
                        for (int i = 0; i < classStruct.memberDataList.Length; i++)
                        {
                            MemberData md = classStruct.memberDataList[i];
                            if (md == null)
                            {
                                md = new MemberData();
                                md.val_describe = "默认备注";
                                md.val_name = "默认变量名";
                                md.val_type = "默认变量类型";
                                classStruct.memberDataList[i] = md;
                            }
                            Rect memberItemRect = new Rect(10, H, 900, 20);
                            GUILayout.BeginArea(memberItemRect);
                            {
                                GUI.Label(new Rect(30, 0, 90, 20), "val_describe", memberStyle);
                                md.val_describe = GUI.TextField(new Rect(122, 0, 100, 18), md.val_describe);
                                GUI.Label(new Rect(227, 0, 75, 20), "val_name", memberStyle);
                                md.val_name = GUI.TextField(new Rect(297, 0, 100, 18), md.val_name);
                                GUI.Label(new Rect(402, 0, 75, 20), "val_type", memberStyle);
                                md.val_type = GUI.TextField(new Rect(472, 0, 100, 18), md.val_type);
                                bool tempValid;
                                tempValid = CheckMemberDataValid(md, isPack, true, out msg);
                                GUIStyle msgStyle = new GUIStyle();
                                msgStyle.normal.textColor = tempValid ? Color.black : Color.red;
                                msgStyle.fontStyle = FontStyle.Bold;
                                msgStyle.fontSize = 12;
                                memberDataArrIsValid = memberDataArrIsValid && tempValid;
                                GUI.Label(new Rect(575, 0, 200, 18), msg, msgStyle);
                            }
                            GUILayout.EndArea();
                            if (GUI.Button(new Rect(10, H, 28, 18), "X"))
                            {
                                removeIndex.Add(i);
                            }
                            H += 24;
                        }
                        List<MemberData> mdList = new List<MemberData>();
                        for (int i = 0; i < classStruct.memberDataList.Length; i++)
                        {
                            if (!removeIndex.Contains(i))
                            {
                                mdList.Add(classStruct.memberDataList[i]);
                            }
                        }
                        classStruct.memberDataList = mdList.ToArray();
                        memberListLength = classStruct.memberDataList.Length;
                    }
                    GUI.EndScrollView();
                }
                GUILayout.Space(10);
                GUIStyle btnStyle = new GUIStyle("button");
                btnStyle.fontStyle = FontStyle.Bold;
                btnStyle.fontSize = 17;
                btnStyle.alignment = TextAnchor.MiddleCenter;
                btnStyle.border = new RectOffset(15, 15, 15, 15);

                bool isOk = true;
                if (string.IsNullOrEmpty(classStruct.dataID))
                {
                    isOk = false;
                }
                if (classStruct.structType != "Pack" && classStruct.structType != "UnPack")
                {
                    isOk = false;
                }
                if (string.IsNullOrEmpty(classStruct.className) && CheckClassNameValid(classStruct.className) == false)
                {
                    isOk = false;
                }
                isOk = isOk && memberDataArrIsValid;

                EditorGUI.BeginDisabledGroup(!isOk);
                if (GUI.Button(new Rect(350, 24 * 8 + 50, 200, 40), "SaveJson", btnStyle))
                {
                    //生成客户端对应的ClassStruct相关json文件
                    string jsonTxt = JsonUtility.ToJson(classStruct);
                    string savePath = ClassStructPath + "/" + classStruct.structType + "/" + classStruct.dataID + "_" + classStruct.className + ".json";
                    if (File.Exists(savePath))
                    {
                        File.Delete(savePath);
                    }
                    //TODO 如果发现单独修改ID或Name之后，并不是修改原有文件，而是新增了一个文件，那么你可以在这里做检测进行删除之前的那个文件.
                    //因为我没有存储之前的文件路径，所以就没有做这一步，每次更新了ID或Name保存后会生成一个新文件的原因是 文件名是根据这2个来命名的，ID_Name 所以...                    
                    File.WriteAllText(savePath, jsonTxt);
                    //反转信息然后生成服务器对应的ClassStruct相关json文件
                    if (classStruct.structType == "UnPack")
                    {
                        classStruct.structType = "Pack";
                    }
                    else
                    {
                        classStruct.structType = "UnPack";
                    }
                    jsonTxt = JsonUtility.ToJson(classStruct);
                    savePath = ServerClassStructPath + "/" + classStruct.structType + "/" + classStruct.dataID + "_" + classStruct.className + ".json";
                    if (File.Exists(savePath))
                    {
                        File.Delete(savePath);
                    }
                    //TODO 如果发现单独修改ID或Name之后，并不是修改原有文件，而是新增了一个文件，那么你可以在这里做检测进行删除之前的那个文件.
                    //因为我没有存储之前的文件路径，所以就没有做这一步，每次更新了ID或Name保存后会生成一个新文件的原因是 文件名是根据这2个来命名的，ID_Name 所以...
                    File.WriteAllText(savePath, jsonTxt);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    //恢复
                    if (classStruct.structType == "UnPack")
                    {
                        classStruct.structType = "Pack";
                    }
                    else
                    {
                        classStruct.structType = "UnPack";
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndArea();

            EditorGUI.DrawRect(new Rect(48, 413, 454, 193f), Color.black);
            EditorGUI.DrawRect(new Rect(50, 415, 450, 189f), Color.white);
            Rect bottomLeftRect = new Rect(50, 415, 450, 189f);
            GUILayout.BeginArea(bottomLeftRect);
            {
                string[] filePathArr = Directory.GetFiles(ClassStructPackPath, "*.json", SearchOption.AllDirectories);
                List<string> filePathList = new List<string>();
                for (int i = 0; i < filePathArr.Length; i++)
                {
                    filePathList.Add(filePathArr[i].Replace('\\', '/'));
                }
                filePathList.Sort(new ListStringCompare());
                protocolScrollPos = GUI.BeginScrollView(new Rect(20, 1, 450, 189), protocolScrollPos, new Rect(20, 28, 450, (filePathArr.Length / 3 + 1) * 30), false, false);
                {
                    for (int i = 0; i < filePathList.Count; i++)
                    {
                        FileInfo fileInfo = new FileInfo(filePathList[i]);
                        FileStream fs = new FileStream(filePathList[i], FileMode.Open);
                        StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                        string[] arr = fileInfo.Name.Split('_');
                        string className = arr[1].Substring(0, arr[1].Length - 5);
                        if (GUI.Button(new Rect(30 + (i % 3) * 135, 35 + (i / 3) * 27, 120, 25), arr[0] + "_" + className, btstyle))
                        {
                            classStruct = JsonUtility.FromJson<ClassStruct>(sr.ReadToEnd());
                            memberListLength = 0;
                            if (classStruct.memberDataList != null)
                            {
                                memberListLength = classStruct.memberDataList.Length;
                            }
                            structTypeIndex = classStruct.structType == "UnPack" ? 0 : 1;
                        }
                        sr.Close();
                        fs.Close();
                    }
                }
                GUI.EndScrollView();
            }
            GUILayout.EndArea();

            EditorGUI.DrawRect(new Rect(497, 413, 454, 193f), Color.black);
            EditorGUI.DrawRect(new Rect(499, 415, 450, 189f), Color.white);
            Rect bottomRightRect = new Rect(498, 415, 450f, 189f);
            GUILayout.BeginArea(bottomRightRect);
            {
                string[] filePathArr = Directory.GetFiles(ClassStructUnPackPath, "*.json", SearchOption.AllDirectories);
                List<string> filePathList = new List<string>();
                for (int i = 0; i < filePathArr.Length; i++)
                {
                    filePathList.Add(filePathArr[i].Replace('\\', '/'));
                }
                filePathList.Sort(new ListStringCompare());
                protocolScrollPos = GUI.BeginScrollView(new Rect(20, 1, 450, 189), protocolScrollPos, new Rect(20, 28, 450, (filePathArr.Length / 3 + 1) * 30), false, false);
                {
                    for (int i = 0; i < filePathList.Count; i++)
                    {
                        FileInfo fileInfo = new FileInfo(filePathList[i]);
                        FileStream fs = new FileStream(filePathList[i], FileMode.Open);
                        StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                        string[] arr = fileInfo.Name.Split('_');
                        string className = arr[1].Substring(0, arr[1].Length - 5);
                        if (GUI.Button(new Rect(30 + (i % 3) * 135, 35 + (i / 3) * 27, 120, 25), arr[0] + "_" + className, btstyle))
                        {
                            classStruct = JsonUtility.FromJson<ClassStruct>(sr.ReadToEnd());
                            memberListLength = 0;
                            if (classStruct.memberDataList != null)
                            {
                                memberListLength = classStruct.memberDataList.Length;
                            }
                            structTypeIndex = classStruct.structType == "UnPack" ? 0 : 1;
                        }
                        sr.Close();
                        fs.Close();
                    }
                }
                GUI.EndScrollView();
            }
            GUILayout.EndArea();
            #endregion
        }


        if (wType == WindowType.Protocol)
        {
            #region ProtocolStruct可视化编辑窗口
            protocolNameIsValid = true;
            memberDataArrIsValid = true;
            protocolIDValid = true;
            #region GUIStyle(CSS)
            //成员列表项区域样式
            GUIStyle memberStyle = new GUIStyle();
            memberStyle.fontStyle = FontStyle.BoldAndItalic;
            memberStyle.normal.textColor = Color.white;
            //顶层区域样式
            GUIStyle topStyle = new GUIStyle();
            topStyle.margin = new RectOffset(50, 50, 10, 10);
            topStyle.padding = new RectOffset(20, 20, 20, 20);
            //普通Label样式
            GUIStyle labelStyle = new GUIStyle("Label");
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = Color.white;
            //添加成员btn样式
            GUIStyle addMemberBtnStyle = new GUIStyle("button");
            addMemberBtnStyle.fontStyle = FontStyle.Bold;
            addMemberBtnStyle.border = new RectOffset(10, 10, 10, 10);
            addMemberBtnStyle.padding = new RectOffset(10, 10, 10, 10);
            addMemberBtnStyle.alignment = TextAnchor.MiddleCenter;
            //普通按钮文本样式
            GUIStyle btstyle = new GUIStyle("button");
            btstyle.alignment = TextAnchor.MiddleLeft;
            #endregion

            Rect topBorderRect = new Rect(48, 8, 904f, 104f);
            EditorGUI.DrawRect(topBorderRect, Color.gray);//最顶层区域的外边框
            Rect topRect = new Rect(50, 10, 900f, 100f);//最顶层的区域       
            EditorGUI.DrawRect(topRect, COLOR_BLACK);//最顶层区域的外边框

            GUILayout.BeginArea(topRect, topStyle);
            {
                H = 10;

                EditorGUI.DrawRect(new Rect(20, H - 1, 860, 20), Color.gray);
                Rect dataIDAreaRect = new Rect(30, H, 860, 20);
                GUILayout.BeginArea(dataIDAreaRect);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("ProtocolID", labelStyle, GUILayout.Width(100));
                        protocolStruct.protocolID = EditorGUILayout.TextField(protocolStruct.protocolID, GUILayout.Width(300));
                        protocolIDValid = CheckProtocolID(protocolStruct.protocolID, out msg);
                        GUIStyle msgStyle = new GUIStyle();
                        msgStyle.normal.textColor = protocolIDValid ? Color.black : Color.red;
                        msgStyle.fontStyle = FontStyle.Bold;
                        msgStyle.fontSize = 13;
                        EditorGUILayout.LabelField(msg, msgStyle, GUILayout.Width(200));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndArea();

                H += 25;

                //ProtocolName
                EditorGUI.DrawRect(new Rect(20, H - 1, 860, 20), Color.gray);
                Rect structTypeAreaRect = new Rect(20, H, 860, 20);
                GUILayout.BeginArea(structTypeAreaRect);
                {
                    Rect dataIDLabelRect = new Rect(10, 0, 100, 20);
                    GUI.Label(dataIDLabelRect, "ProtocolName", labelStyle);
                    Rect dataIDFieldRect = new Rect(115, 0, 300, 18);
                    protocolStruct.protocolName = GUI.TextField(dataIDFieldRect, protocolStruct.protocolName);
                    string msg = "";
                    protocolNameIsValid = CheckProtocolName(protocolStruct.protocolName, out msg);
                    GUIStyle msgStyle = new GUIStyle();
                    msgStyle.normal.textColor = protocolNameIsValid ? Color.black : Color.red;
                    msgStyle.fontStyle = FontStyle.Bold;
                    msgStyle.fontSize = 12;
                    GUI.Label(new Rect(425, 0, 860 - 415 - 10, 18), msg, msgStyle);
                }
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
            //Pack发包数据
            EditorGUI.DrawRect(new Rect(48, 75, 904, 194f + 14), Color.gray);
            EditorGUI.DrawRect(new Rect(50, 77, 900f, 190f + 14), COLOR_BLACK);
            Rect centerTopRect = new Rect(50, 77, 900f, 190f + 14);
            GUILayout.BeginArea(centerTopRect);
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add PackMemberData", addMemberBtnStyle, GUILayout.Width(200)))
                {
                    packemberListLength++;
                }
                if (GUILayout.Button("Clear All PackMemberData", addMemberBtnStyle, GUILayout.Width(200)))
                {
                    protocolStruct.packMemberArr = null;
                    packemberListLength = 0;
                }
                GUILayout.EndHorizontal();

                if (packemberListLength > 0)
                {
                    if (protocolStruct.packMemberArr == null || (protocolStruct.packMemberArr != null && protocolStruct.packMemberArr.Length != packemberListLength))
                    {
                        if (protocolStruct.packMemberArr != null)
                        {
                            MemberData[] tempMemberData = new MemberData[protocolStruct.packMemberArr.Length];
                            tempMemberData = protocolStruct.packMemberArr;
                            MemberData[] mdarr = new MemberData[packemberListLength];
                            for (int i = 0; i < tempMemberData.Length; i++)
                            {
                                mdarr[i] = tempMemberData[i];
                            }
                            protocolStruct.packMemberArr = mdarr;
                            packemberListLength = protocolStruct.packMemberArr.Length;
                        }
                        else
                        {
                            protocolStruct.packMemberArr = new MemberData[packemberListLength];
                        }
                    }
                    EditorGUI.DrawRect(new Rect(0, 35, 900, 155 + 14), Color.gray);
                    memberPackScrollPos1 = GUI.BeginScrollView(new Rect(0, 35, 900, 155 + 14), memberPackScrollPos1, new Rect(0, 35, 900, 24 * (packemberListLength)), false, false);
                    {
                        H = 35;
                        List<int> removeIndex = new List<int>();
                        for (int i = 0; i < protocolStruct.packMemberArr.Length; i++)
                        {
                            MemberData md = protocolStruct.packMemberArr[i];
                            if (md == null)
                            {
                                md = new MemberData();
                                md.val_describe = "默认备注";
                                md.val_name = "默认变量名";
                                md.val_type = "默认变量类型";
                                protocolStruct.packMemberArr[i] = md;
                            }
                            Rect memberItemRect = new Rect(10, H, 900, 20);
                            GUILayout.BeginArea(memberItemRect);
                            {
                                GUI.Label(new Rect(30, 0, 90, 20), "val_describe", memberStyle);
                                md.val_describe = GUI.TextField(new Rect(122, 0, 100, 18), md.val_describe);
                                GUI.Label(new Rect(227, 0, 75, 20), "val_name", memberStyle);
                                md.val_name = GUI.TextField(new Rect(297, 0, 100, 18), md.val_name);
                                GUI.Label(new Rect(402, 0, 75, 20), "val_type", memberStyle);
                                md.val_type = GUI.TextField(new Rect(472, 0, 100, 18), md.val_type);
                                bool tempValid;
                                tempValid = CheckMemberDataValid(md, true, true, out msg);
                                GUIStyle msgStyle = new GUIStyle();
                                msgStyle.normal.textColor = tempValid ? Color.black : Color.red;
                                msgStyle.fontStyle = FontStyle.Bold;
                                msgStyle.fontSize = 12;
                                memberDataArrIsValid = memberDataArrIsValid && tempValid;
                                GUI.Label(new Rect(575, 0, 200, 18), msg, msgStyle);
                            }
                            GUILayout.EndArea();
                            if (GUI.Button(new Rect(10, H, 28, 18), "X"))
                            {
                                removeIndex.Add(i);
                            }
                            H += 24;
                        }
                        List<MemberData> mdList = new List<MemberData>();
                        for (int i = 0; i < protocolStruct.packMemberArr.Length; i++)
                        {
                            if (!removeIndex.Contains(i))
                            {
                                mdList.Add(protocolStruct.packMemberArr[i]);
                            }
                        }
                        protocolStruct.packMemberArr = mdList.ToArray();
                        packemberListLength = protocolStruct.packMemberArr.Length;
                    }
                    GUI.EndScrollView();
                }
            }
            GUILayout.EndArea();

            int offsetCenterY = 80;
            EditorGUI.DrawRect(new Rect(48, 199 + offsetCenterY, 904, 214f), Color.gray);
            EditorGUI.DrawRect(new Rect(50, 201 + offsetCenterY, 900f, 210f), COLOR_BLACK);
            Rect centerRect = new Rect(50, 201 + offsetCenterY, 900f, 210f);
            GUILayout.BeginArea(centerRect);
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add UnpackMemberData", addMemberBtnStyle, GUILayout.Width(200)))
                {
                    memberListLength1++;
                }
                if (GUILayout.Button("Clear All UnpackMemberData", addMemberBtnStyle, GUILayout.Width(220)))
                {
                    protocolStruct.unPackMemberArr = null;
                    memberListLength1 = 0;
                }
                GUILayout.EndHorizontal();

                if (memberListLength1 > 0)
                {
                    if (protocolStruct.unPackMemberArr == null || (protocolStruct.unPackMemberArr != null && protocolStruct.unPackMemberArr.Length != memberListLength1))
                    {
                        if (protocolStruct.unPackMemberArr != null)
                        {
                            MemberData[] tempMemberData = new MemberData[protocolStruct.unPackMemberArr.Length];
                            tempMemberData = protocolStruct.unPackMemberArr;
                            MemberData[] mdarr = new MemberData[memberListLength1];
                            for (int i = 0; i < tempMemberData.Length; i++)
                            {
                                mdarr[i] = tempMemberData[i];
                            }
                            protocolStruct.unPackMemberArr = mdarr;
                            memberListLength1 = protocolStruct.unPackMemberArr.Length;
                        }
                        else
                        {
                            protocolStruct.unPackMemberArr = new MemberData[memberListLength1];
                        }
                    }
                    EditorGUI.DrawRect(new Rect(0, 35, 900, 140), Color.gray);
                    memberScrollPos1 = GUI.BeginScrollView(new Rect(0, 35, 900, 140), memberScrollPos1, new Rect(0, 35, 900, 24 * (memberListLength1)), false, false);
                    {
                        H = 35;
                        List<int> removeIndex = new List<int>();
                        for (int i = 0; i < protocolStruct.unPackMemberArr.Length; i++)
                        {
                            MemberData md = protocolStruct.unPackMemberArr[i];
                            if (md == null)
                            {
                                md = new MemberData();
                                md.val_describe = "默认备注";
                                md.val_name = "默认变量名";
                                md.val_type = "默认变量类型";
                                protocolStruct.unPackMemberArr[i] = md;
                            }
                            Rect memberItemRect = new Rect(10, H, 900, 20);
                            GUILayout.BeginArea(memberItemRect);
                            {
                                GUI.Label(new Rect(30, 0, 90, 20), "val_describe", memberStyle);
                                md.val_describe = GUI.TextField(new Rect(122, 0, 100, 18), md.val_describe);
                                GUI.Label(new Rect(227, 0, 75, 20), "val_name", memberStyle);
                                md.val_name = GUI.TextField(new Rect(297, 0, 100, 18), md.val_name);
                                GUI.Label(new Rect(402, 0, 75, 20), "val_type", memberStyle);
                                md.val_type = GUI.TextField(new Rect(472, 0, 100, 18), md.val_type);
                                bool tempValid;
                                tempValid = CheckMemberDataValid(md, false, true, out msg);
                                GUIStyle msgStyle = new GUIStyle();
                                msgStyle.normal.textColor = tempValid ? Color.black : Color.red;
                                msgStyle.fontStyle = FontStyle.Bold;
                                msgStyle.fontSize = 12;
                                memberDataArrIsValid = memberDataArrIsValid && tempValid;
                                GUI.Label(new Rect(575, 0, 200, 18), msg, msgStyle);
                            }
                            GUILayout.EndArea();
                            if (GUI.Button(new Rect(10, H, 28, 18), "X"))
                            {
                                removeIndex.Add(i);
                            }
                            H += 24;
                        }
                        List<MemberData> mdList = new List<MemberData>();
                        for (int i = 0; i < protocolStruct.unPackMemberArr.Length; i++)
                        {
                            if (!removeIndex.Contains(i))
                            {
                                mdList.Add(protocolStruct.unPackMemberArr[i]);
                            }
                        }
                        protocolStruct.unPackMemberArr = mdList.ToArray();
                        memberListLength1 = protocolStruct.unPackMemberArr.Length;
                    }
                    GUI.EndScrollView();
                }
                GUILayout.Space(10);
                GUIStyle btnStyle = new GUIStyle("button");
                btnStyle.fontStyle = FontStyle.Bold;
                btnStyle.fontSize = 15;
                btnStyle.alignment = TextAnchor.MiddleCenter;

                bool isOk = true;
                if (string.IsNullOrEmpty(protocolStruct.protocolID))
                {
                    isOk = false;
                }
                if (string.IsNullOrEmpty(protocolStruct.protocolName))
                {
                    isOk = false;
                }
                isOk = isOk && memberDataArrIsValid;

                
                GUILayout.BeginArea(new Rect(0, 175, 900, 35));
                {                    
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginDisabledGroup(!isOk);
                        if (GUILayout.Button("SaveJson", btnStyle))
                        {
                            //生成客户端对应的ClassStruct相关json文件                    
                            string jsonTxt = JsonUtility.ToJson(protocolStruct);
                            string savePath = ProtocolStructPath + "/" + protocolStruct.protocolID + "_" + protocolStruct.protocolName + ".json";
                            if (File.Exists(savePath))
                            {
                                File.Delete(savePath);
                            }
                            //TODO 如果发现单独修改ID或Name之后，并不是修改原有文件，而是新增了一个文件，那么你可以在这里做检测进行删除之前的那个文件.
                            //因为我没有存储之前的文件路径，所以就没有做这一步，每次更新了ID或Name保存后会生成一个新文件的原因是 文件名是根据这2个来命名的，ID_Name 所以...
                            File.WriteAllText(savePath, jsonTxt);

                            MemberData[] tempMemberArr = protocolStruct.packMemberArr;
                            protocolStruct.packMemberArr = new MemberData[protocolStruct.unPackMemberArr.Length];
                            protocolStruct.packMemberArr = protocolStruct.unPackMemberArr;
                            packemberListLength = protocolStruct.packMemberArr.Length;
                            protocolStruct.unPackMemberArr = new MemberData[tempMemberArr.Length];
                            protocolStruct.unPackMemberArr = tempMemberArr;
                            memberListLength1 = protocolStruct.unPackMemberArr.Length;
                            jsonTxt = JsonUtility.ToJson(protocolStruct);
                            savePath = ServerProtocolStructPath + "/" + protocolStruct.protocolID + "_" + protocolStruct.protocolName + ".json";
                            if (File.Exists(savePath))
                            {
                                File.Delete(savePath);
                            }
                            //TODO 如果发现单独修改ID或Name之后，并不是修改原有文件，而是新增了一个文件，那么你可以在这里做检测进行删除之前的那个文件.
                            //因为我没有存储之前的文件路径，所以就没有做这一步，每次更新了ID或Name保存后会生成一个新文件的原因是 文件名是根据这2个来命名的，ID_Name 所以...
                            File.WriteAllText(savePath, jsonTxt);
                            MemberData[] tempMemberArr1 = protocolStruct.packMemberArr;
                            protocolStruct.packMemberArr = new MemberData[protocolStruct.unPackMemberArr.Length];
                            protocolStruct.packMemberArr = protocolStruct.unPackMemberArr;
                            packemberListLength = protocolStruct.packMemberArr.Length;
                            protocolStruct.unPackMemberArr = new MemberData[tempMemberArr1.Length];
                            protocolStruct.unPackMemberArr = tempMemberArr1;
                            memberListLength1 = protocolStruct.unPackMemberArr.Length;
                            AssetDatabase.Refresh();
                            AssetDatabase.SaveAssets();
                        }
                        EditorGUI.EndDisabledGroup();
                        if (GUILayout.Button("DeleteJson", btnStyle))
                        {
                            if (MilkMessage.MessageBox(IntPtr.Zero, "Delete This Protocol:" + protocolStruct.protocolName, "提示", 1) == 1)
                            {
                                string savePath = ProtocolStructPath + "/" + protocolStruct.protocolID + "_" + protocolStruct.protocolName + ".json";
                                if (File.Exists(savePath))
                                {
                                    File.Delete(savePath);
                                }
                                savePath = ServerProtocolStructPath + "/" + protocolStruct.protocolID + "_" + protocolStruct.protocolName + ".json";
                                if (File.Exists(savePath))
                                {
                                    File.Delete(savePath);
                                }
                                protocolStruct = new ProtocolStruct();
                                memberListLength1 = 0;
                                packemberListLength = 0;
                                AssetDatabase.Refresh();
                                AssetDatabase.SaveAssets();
                            }
                        }

                        if (GUILayout.Button("Clear", btnStyle))
                        {
                            protocolStruct = new ProtocolStruct();
                            memberListLength1 = 0;
                            packemberListLength = 0;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();                
            }
            GUILayout.EndArea();

            int offsetBottomY = 78;
            EditorGUI.DrawRect(new Rect(48, 413 + offsetBottomY, 904, 193f - offsetBottomY), Color.gray);
            EditorGUI.DrawRect(new Rect(50, 415 + offsetBottomY, 900, 189f - offsetBottomY), COLOR_BLACK);
            Rect bottomLeftRect = new Rect(50, 415 + offsetBottomY, 900, 189f - offsetBottomY);
            GUILayout.BeginArea(bottomLeftRect);
            {
                string[] filePathArr = Directory.GetFiles(ProtocolStructPath, "*.json", SearchOption.AllDirectories);
                List<string> filePathList = new List<string>();
                for (int i = 0; i < filePathArr.Length; i++)
                {
                    filePathList.Add(filePathArr[i].Replace('\\', '/'));
                }
                filePathList.Sort(new ListStringCompare());
                protocolScrollPos1 = GUI.BeginScrollView(new Rect(0, 1, 900, 189 - offsetBottomY), protocolScrollPos1, new Rect(0, 25, 900, (filePathArr.Length / 6 + 1) * 30), false, false);
                {
                    for (int i = 0; i < filePathList.Count; i++)
                    {
                        FileInfo fileInfo = new FileInfo(filePathList[i]);
                        FileStream fs = new FileStream(filePathList[i], FileMode.Open);
                        StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);
                        string[] arr = fileInfo.Name.Split('_');
                        string className = arr[1].Substring(0, arr[1].Length - 5);
                        if (GUI.Button(new Rect(0 + (i % 6) * 142, 25 + (i / 6) * 27, 140, 25), arr[0] + "_" + className, btstyle))
                        {
                            protocolStruct = JsonUtility.FromJson<ProtocolStruct>(sr.ReadToEnd());
                            memberListLength1 = 0;
                            if (protocolStruct.unPackMemberArr != null)
                            {
                                memberListLength1 = protocolStruct.unPackMemberArr.Length;
                            }
                            packemberListLength = 0;
                            if (protocolStruct.packMemberArr != null)
                            {
                                packemberListLength = protocolStruct.packMemberArr.Length;
                            }
                        }
                        sr.Close();
                        fs.Close();
                    }
                }
                GUI.EndScrollView();
            }
            GUILayout.EndArea();
            #endregion
        }
    }

    public bool CheckProtocolName(string protocolName, out string msg)
    {
        Regex valNameRegex = new Regex(@"^[a-zA-Z]{1,99}$");
        if (string.IsNullOrEmpty(protocolName) || !valNameRegex.IsMatch(protocolName))
        {
            msg = "ProtocolName只能由大小写字母组成!";
            return false;
        }
        string path = "";
        path = Application.dataPath + "/Script/Editor/Config/ProtocolStruct";
        if (Directory.Exists(path))
        {
            string[] filePathArr = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < filePathArr.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(filePathArr[i]);
                string[] arr = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5).Split('_');
                //arr[0] = ID , arr[1] = protocolName
                if (protocolName == arr[1])
                {
                    msg = "协议名已存在!";
                    return false;
                }
            }
        }
        else
        {
            Debug.LogError("协议结构目录不存在!" + path);
        }
        msg = " √";
        return true;
    }

    public bool CheckClassNameValid(string className)
    {
        Regex valNameRegex = new Regex(@"^[a-zA-Z]{1,99}$");
        if (string.IsNullOrEmpty(className) || !valNameRegex.IsMatch(className))
        {
            msg = "ClassName只能由大小写字母组成!";
            return false;
        }
        return true;
    }

    public bool CheckClassNameValid(string className, bool isPack, out string msg)
    {
        Regex valNameRegex = new Regex(@"^[a-zA-Z]{1,99}$");
        if (string.IsNullOrEmpty(className) || !valNameRegex.IsMatch(className))
        {
            msg = "ClassName只能由大小写字母组成!";
            return false;
        }
        string path = "";
        if (isPack)
        {
            path = Application.dataPath + "/Script/Editor/Config/ClassStruct/Pack";
            //path = ClassStructPackPath;
        }
        else
        {
            path = Application.dataPath + "/Script/Editor/Config/ClassStruct/UnPack";
            //path = ClassStructUnPackPath;
        }
        if (Directory.Exists(path))
        {
            string[] filePathArr = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < filePathArr.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(filePathArr[i]);
                string[] arr = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5).Split('_');
                //arr[0] = ID , arr[1] = className
                if (className == arr[1])
                {
                    msg = "类名已存在!";
                    return false;
                }
            }
        }
        else
        {
            Debug.LogError("Pack封包或UnPack解包类结构存放目录不存在" + path);
        }
        msg = " √";
        return true;
    }
    public bool CheckProtocolID(string protocolID, out string msg)
    {
        if (string.IsNullOrEmpty(protocolID))
        {
            msg = "ID不能为空!";
            return false;
        }
        string path = "";
        path = Application.dataPath + "/Script/Editor/Config/ProtocolStruct";
        if (Directory.Exists(path))
        {
            string[] filePathArr = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            for (int i = 0; i < filePathArr.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(filePathArr[i]);
                string[] arr = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5).Split('_');
                //arr[0] = ID , arr[1] = className
                if (protocolID == arr[0])
                {
                    msg = "ProtocolID已存在!";
                    return false;
                }
            }
        }
        else
        {
            Debug.LogError("ProtocolStruct目录不存在" + path);
        }
        msg = " √";
        return true;
    }

    public bool CheckDataID(string dataID, out string msg)
    {
        if (string.IsNullOrEmpty(dataID))
        {
            msg = "ID不能为空!";
            return false;
        }
        string path = "";
        path = Application.dataPath + "/Script/Editor/Config/ClassStruct";
        if (Directory.Exists(path))
        {
            string[] filePathArr = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            for (int i = 0; i < filePathArr.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(filePathArr[i]);
                string[] arr = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5).Split('_');
                //arr[0] = ID , arr[1] = className
                if (dataID == arr[0])
                {
                    msg = "DataID已存在!";
                    return false;
                }
            }
        }
        else
        {
            Debug.LogError("ClassStruct目录不存在" + path);
        }
        msg = " √";
        return true;
    }
    /// <summary>
    /// 检查单个成员有效性
    /// </summary>
    /// <returns></returns>
    public bool CheckMemberDataValid(MemberData memberData, bool isPack, bool isClient, out string msg)
    {
        string str = isClient ? "客户端" : "服务端";
        str += isPack ? "Pack问题" : "UnPack问题";
        //1.检查名称的有效性
        Regex valNameRegex = new Regex(@"^[a-zA-Z][\w+]{0,99}$");
        if (string.IsNullOrEmpty(memberData.val_name) || !valNameRegex.IsMatch(memberData.val_name))
        {
            msg = "val_name必须由字母开头,且只由下划线、数字和字母组成";
            return false;
        }
        //2.检查类型的有效性     
        if (memberData.val_type != "string" && memberData.val_type != "int"
            && memberData.val_type != "bool" && memberData.val_type != "float"
            && memberData.val_type != "char" && memberData.val_type != "double"
            && CheckClassType(memberData.val_type, isPack, isClient) == false)
        {
            msg = str + " val_type不存在!请检查其是否拼写正确!";
            return false;
        }

        msg = " √";
        return true;
    }

    /// <summary>
    /// 检查类型是否存在
    /// </summary>
    /// <returns></returns>
    public bool CheckClassType(string valType, bool isPack, bool isClient)
    {
        string ClassStructPackPath = Application.dataPath + "/Script/Editor/Config/ClassStruct/Pack";
        string ClassStructUnPackPath = Application.dataPath + "/Script/Editor/Config/ClassStruct/UnPack";
        string ServerClassStructPackPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct/Pack";
        string ServerClassStructUnPackPath = Application.dataPath + "/Script/Editor/ServerConfig/ClassStruct/UnPack";
        bool isExist = false;
        if (isClient)
        {
            if (isPack)
            {
                isExist = CheckClassTypeInDirectory(valType, ClassStructPackPath);
            }
            else
            {
                isExist = CheckClassTypeInDirectory(valType, ClassStructUnPackPath);
            }
        }
        else
        {
            if (isPack)
            {
                isExist = CheckClassTypeInDirectory(valType, ServerClassStructPackPath);
                //isExist = CheckClassTypeInDirectory(valType, @"G:/服务器-应用控制台程序/MilkGameServer/Server/Protocol/ClassStruct/Pack");
            }
            else
            {
                isExist = CheckClassTypeInDirectory(valType, ServerClassStructUnPackPath);
                //isExist = CheckClassTypeInDirectory(valType, @"G:/服务器-应用控制台程序/MilkGameServer/Server/Protocol/ClassStruct/UnPack");
            }
        }
        return isExist;
    }

    /// <summary>
    /// 检查数据类型在json目录下是否存在
    /// </summary>
    public bool CheckClassTypeInDirectory(string valType, string dirPath)
    {
        string[] packPaths = Directory.GetFiles(dirPath, "*.json", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < packPaths.Length; i++)
        {
            FileInfo fileInfo = new FileInfo(packPaths[i]);
            string[] arr = fileInfo.Name.Split('_');
            string className = arr[1].Substring(0, arr[1].Length - 5);
            if (valType == className)
            {
                return true;
            }
        }
        return false;
    }
}
public class ListStringCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        int xNum = int.Parse(x.Substring(x.LastIndexOf('/') + 1).Split('_')[0]);
        int yNum = int.Parse(y.Substring(y.LastIndexOf('/') + 1).Split('_')[0]);
        return xNum >= yNum ? 1 : -1;
    }
}