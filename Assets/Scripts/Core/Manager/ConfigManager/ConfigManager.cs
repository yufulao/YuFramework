// ******************************************************************
//@file         ConfigManager.cs
//@brief        配置系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:23:38
// ******************************************************************

using Luban;
using UnityEngine;

namespace Yu
{
    public class ConfigManager : BaseSingleTon<ConfigManager>, IMonoManager
    {
        public static Tables Tables;


        public void OnInit()
        {
            if (Tables != null)
            {
                return;
            }

            Tables = new Tables(LoadByteBuf);
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }

        /// <summary>
        /// 从file中读取字节流
        /// </summary>
        private static ByteBuf LoadByteBuf(string file)
        {
            return new ByteBuf(AssetManager.LoadAsset<TextAsset>($"Assets/AddressableAssets/Config/{file}.bytes").bytes);
        }
    }
}