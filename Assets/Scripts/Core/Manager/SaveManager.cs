// ******************************************************************
//@file         SaveManager.cs
//@brief        存储系统（依赖EasySave插件）
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:29:39
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class SaveManager : BaseSingleTon<SaveManager>, IMonoManager
    {
        private static readonly Dictionary<string, ES3Settings> SettingDic = new Dictionary<string, ES3Settings>();

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
        /// </summary>
        public static bool ExistKey(string key, string fileName = null) => ES3.KeyExists(key, fileName);

        /// <summary>
        /// 查询是否有该文件
        /// </summary>
        /// <returns></returns>
        public static bool ExistFile(string fileName) => ES3.FileExists(fileName);

        /// <summary>
        /// 新建指定文件
        /// </summary>
        public static void CreateFile(string fileName)
        {
            if (ExistFile(fileName))
            {
                Debug.LogError("文件已存在" + fileName);
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
        /// </summary>
        public static void Set<T>(string key, T t, string fileName)
        {
            if (ExistFile(fileName))
            {
                ES3.Save(key, t, fileName);
                return;
            }

            Debug.LogError("文件名不存在");
        }

        /// <summary>
        /// 获取泛型，指定文件
        /// </summary>
        public static T Get<T>(string key, T defaultValue, string fileName)
        {
            if (ExistFile(fileName))
            {
                return GetWithoutFileCheck(key, defaultValue, fileName);
            }

            Debug.LogError("文件名不存在");
            return defaultValue;
        }

        /// <summary>
        /// 获取泛型，指定文件，不检测文件是否存在
        /// </summary>
        public static T GetWithoutFileCheck<T>(string key, T defaultValue, string fileName)
        {
            if (ExistKey(key, fileName))
            {
                return ES3.Load<T>(key, defaultValue, GetES3Settings(fileName));
            }

            Set(key, defaultValue, fileName);
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

            if (SettingDic.ContainsKey(fileName))
            {
                return SettingDic[fileName];
            }

            var newSetting = new ES3Settings(fileName);
            SettingDic.Add(fileName, newSetting);
            return newSetting;
        }
    }
}