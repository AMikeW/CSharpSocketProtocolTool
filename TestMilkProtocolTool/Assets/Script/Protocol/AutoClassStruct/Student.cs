public class Student : ClassBase
{
    //默认备注
    public string studentName;
    //默认备注
    public string sex;
    //默认备注
    public string hoby;
    //默认备注
    public float score;
    public Student(string studentName,string sex,string hoby,float score)
    {
        AddMemberObject((DataID)3);
        this.studentName = studentName;
        AddMemberObject(this.studentName);
        this.sex = sex;
        AddMemberObject(this.sex);
        this.hoby = hoby;
        AddMemberObject(this.hoby);
        this.score = score;
        AddMemberObject(this.score);
    }
}
