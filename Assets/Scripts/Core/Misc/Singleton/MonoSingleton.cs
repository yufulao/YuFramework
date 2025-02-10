// ******************************************************************
//@file         MonoSingleton.cs
//@brief        MonoBehaviour单例基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:11:24
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }
    }
}