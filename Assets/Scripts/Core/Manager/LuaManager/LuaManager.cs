// ******************************************************************
//@file         LuaManager.cs
//@brief        lua管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:27:17
// ******************************************************************

using System.IO;
using UnityEngine;
using XLua;

namespace Yu
{
    public class LuaManager : BaseSingleTon<LuaManager>, IMonoManager
    {
        public readonly LuaEnv LuaEnv = new LuaEnv();
        private LuaUpdater _luaUpdater; //计时器


        public void OnInit()
        {
            var gameObject = new GameObject("LuaManager");
            gameObject.transform.SetParent(GameManager.Instance.gameObject.transform);
            LuaEnv.AddLoader(CustomLoader);
            //计时器
            //lua入口启动
            SafeDoString($"require('{GlobalDef.MainScriptPath}')");
            _luaUpdater = gameObject.GetComponent<LuaUpdater>() ?? gameObject.AddComponent<LuaUpdater>();
            _luaUpdater.OnInit(LuaEnv);
        }

        public void Update()
        {
            LuaEnv?.Tick();
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
        /// 自定义加载器
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static byte[] CustomLoader(ref string fileName)
        {
            if (fileName.Equals("emmy_core"))
            {
                return default;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                Debug.Log($"lua文件读取失败 fileName:{fileName}");
                return default;
            }

            if (Application.isEditor)
            {
                //编辑器读本地源代码
                fileName = fileName.Replace(".", "/") + ".lua";
                var scriptPath = $"{GlobalDef.LuaScriptFolder}/{fileName}";
                return File.ReadAllBytes(scriptPath);
            }

            //运行时
            fileName = fileName.Replace(".", "/") + ".bytes";
            var assetPath = $"{GlobalDef.LuaScriptAAFolder}/{fileName}";
            //解密
            var luaDecrypt = Utils.AesDecrypt(AssetManager.Instance.LoadAsset<TextAsset>(assetPath).bytes, "yufulao@qq.com");
            return luaDecrypt;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="scriptContent"></param>
        public void SafeDoString(string scriptContent)
        {
            if (LuaEnv == null)
            {
                return;
            }

            try
            {
                LuaEnv.DoString(scriptContent);
            }
            catch (System.Exception ex)
            {
                var msg = $"xLua exception : {ex.Message}\n {ex.StackTrace}";
                Debug.LogError(msg);
            }
        }
    }
}