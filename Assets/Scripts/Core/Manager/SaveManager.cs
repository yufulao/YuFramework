// ******************************************************************
//@file         SaveManager.cs
//@brief        存储系统（依赖EasySave插件）
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:29:39
// ******************************************************************

using System.Collections.Generic;

namespace Yu
{
    public class SaveManager : BaseSingleTon<SaveManager>, IMonoManager
    {
        private static readonly Dictionary<string, ES3Settings> SettingDic = new();

        public void OnInit()
        {
            SettingDic.Clear();
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
        /// 查询是否有该项数据
        /// <param name="checkFile">是否检查文件名存在</param>
        /// </summary>
        public static bool ExistKey(string key, string fileName, bool checkFile = true)
        {
            if (checkFile && ExistFile(fileName))
            {
                return false;
            }
            
            return ES3.KeyExists(key, fileName);
        }

        /// <summary>
        /// 查询是否有该文件
        /// </summary>
        public static bool ExistFile(string fileName) => ES3.FileExists(fileName);

        /// <summary>
        /// 新建指定文件
        /// </summary>
        public static void CreateFile(string fileName)
        {
            if (ExistFile(fileName))
            {
                GameLog.Error("文件已存在" + fileName);
                return;
            }

            ES3.Save("FileName", fileName, fileName);
        }

        /// <summary>
        /// 清除数据项
        /// </summary>
        public static void DeleteKey(string key, string fileName = null)
        {
            ES3.DeleteKey(key, fileName);
        }

        /// <summary>
        /// 清除指定文件
        /// </summary>
        public static void DeleteFile(string fileName) => ES3.DeleteFile(fileName);

        /// <summary>
        /// 复制指定文件
        /// </summary>
        public static void CopyFile(string oldFileName, string newFileName) => ES3.CopyFile(oldFileName, newFileName);

        /// <summary>
        /// 清除指定文件夹
        /// </summary>
        public static void DeleteDir(string dicName) => ES3.DeleteDirectory(dicName);

        /// <summary>
        /// 存泛型，指定文件
        /// <param name="checkFile">是否检查文件名存在</param>
        /// </summary>
        public static void Set<T>(string key, string fileName, T value, bool checkFile = true)
        {
            if (checkFile && !ExistFile(fileName))
            {
                GameLog.Error("文件名不存在");
                return;
            }
            
            ES3.Save(key, value, fileName);
        }

        /// <summary>
        /// 获取泛型，指定文件，不检测文件是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="fileName">文件名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="checkFile">是否检查文件名存在</param>
        /// <param name="checkKey">是否检查key</param>
        /// <param name="autoSetIfNoKey">无key是否自动创建一个key，使用defaultValue作为创建值</param>
        public static T Get<T>(string key, string fileName, T defaultValue = default, bool checkFile = true, bool checkKey = true, bool autoSetIfNoKey = true)
        {
            if (checkFile && !ExistFile(fileName))
            {
                return defaultValue;
            }

            if (!checkKey)
            {
                return ES3.Load(key, defaultValue, GetES3Settings(fileName));
            }

            if (ExistKey(key, fileName, false))
            {
                return ES3.Load(key, defaultValue, GetES3Settings(fileName));
            }

            if (!autoSetIfNoKey)
            {
                return defaultValue;
            }

            ES3.Save(key, defaultValue, fileName);
            return defaultValue;
        }

        /// <summary>
        /// 通过fileName获取ES3Settings
        /// </summary>
        private static ES3Settings GetES3Settings(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return ES3Settings.defaultSettings;
            }

            if (SettingDic.TryGetValue(fileName, out var settings))
            {
                return settings;
            }

            var newSetting = new ES3Settings(fileName);
            SettingDic.Add(fileName, newSetting);
            return newSetting;
        }
    }
}