// ******************************************************************
//@file         TransformTimeUser.cs
//@brief        timeUser封装的transform
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 20:07:31
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class TransformTimeUser: ComponentTimeUser<Transform>
    {
        public TransformTimeUser(TimeUser timeUser, Transform component) : base(timeUser, component) { }
    }
}