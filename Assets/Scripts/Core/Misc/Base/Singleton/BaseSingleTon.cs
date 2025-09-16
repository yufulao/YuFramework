// ******************************************************************
//@file         BaseSingleTon.cs
//@brief        单例基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:10:47
// ******************************************************************

namespace Yu
{
    public class BaseSingleTon<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}