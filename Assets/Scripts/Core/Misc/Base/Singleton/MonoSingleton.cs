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
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }
    }
}