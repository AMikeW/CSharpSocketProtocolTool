public class User : ClassBase
{
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
        AddMemberObject((DataID)1);
        this.userID = userID;
        AddMemberObject(this.userID);
        this.userName = userName;
        AddMemberObject(this.userName);
        this.userPassword = userPassword;
        AddMemberObject(this.userPassword);
        this.photo = photo;
        AddMemberObject(this.photo);
    }
}
