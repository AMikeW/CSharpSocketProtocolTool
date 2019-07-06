using System.Collections.Generic;
public class Student
{
    public const DataID DATAID = (DataID)3;
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
        this.studentName = studentName;
        this.sex = sex;
        this.hoby = hoby;
        this.score = score;
    }
    public static Student ToClass(List<object> objectList, ref int index)
    {
        return new Student((string)objectList[index++], (string)objectList[index++], (string)objectList[index++], (float)objectList[index++]);
    }
}
