using System.Collections.Generic;
public class User
{
    public const DataID DATAID = (DataID)1;
    //默认备注
    public int userID;
    //默认备注
    public string userName;
    //默认备注
    public string userPassword;
    //默认备注
    public string photo;
    public User(int userID,string userName,string userPassword,string photo)
    {
        this.userID = userID;
        this.userName = userName;
        this.userPassword = userPassword;
        this.photo = photo;
    }
    public static User ToClass(List<object> objectList, ref int index)
    {
        return new User((int)objectList[index++], (string)objectList[index++], (string)objectList[index++], (string)objectList[index++]);
    }
}
