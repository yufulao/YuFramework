// ******************************************************************
//@file         IRigidbodyTimeUser.cs
//@brief        rigidbody的接口类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.24 14:34:24
// ******************************************************************
namespace Yu
{
    public interface IRigidbodyTimeUser
    {
        float Mass { get; set; }
        float Drag { get; set; }
        float AngularDrag { get; set; }
    }
}