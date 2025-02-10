// ******************************************************************
//@file       ExportLuaFile.cs
//@brief      导出lua文件
//@author     yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:12:40
// ******************************************************************

using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Yu
{
    public static class ExportLuaFile
    {
        /// <summary>
        /// lua转bytes文件
        /// </summary>
        [MenuItem("XLua/生成lua包资源")]
        public static void Main()
        {
            var startTime = EditorApplication.timeSinceStartup;
            try
            {
                AssetDatabase.StartAssetEditing();
                //清理lua包资源目录
                RabiFileUtil.SafeDeleteFolder(GlobalDef.LuaScriptAAFolder);
                //拷贝lua资源到lua包目录
                FileUtil.CopyFileOrDirectoryFollowSymlinks(GlobalDef.LuaScriptFolder, GlobalDef.LuaScriptAAFolder);
                //删除非lua文件
                var notLuaFiles = RabiFileUtil.GetSpecifyFilesInFolder(GlobalDef.LuaScriptAAFolder, new[] {".lua"}, true);
                if (notLuaFiles != null && notLuaFiles.Length > 0)
                {
                    foreach (var notLuaFileName in notLuaFiles)
                    {
                        RabiFileUtil.SafeDeleteFile(notLuaFileName);
                    }
                }

                //lua文件
                var luaFiles = RabiFileUtil.GetSpecifyFilesInFolder(GlobalDef.LuaScriptAAFolder, new[] {".lua"});
                if (luaFiles != null && luaFiles.Length > 0)
                {
                    foreach (var luaFile in luaFiles)
                    {
                        var newName = luaFile.Replace(".lua", ".bytes");
                        RabiFileUtil.SafeRenameFile(luaFile, newName);
                        EncodeLua(newName);
                    }
                }

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                throw;
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            var endTime = EditorApplication.timeSinceStartup;
            Debug.Log($"lua源文件转二进制完成 time:{endTime - startTime} ");
        }

        /// <summary>
        /// 加密lua文件
        /// </summary>
        /// <param name="fileName"></param>
        private static void EncodeLua(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            try
            {
                //加密
                var content = File.ReadAllBytes(fileName);
                content = Utils.AesEncrypt(content, "yufulao@qq.com");
                File.WriteAllBytes(fileName, content);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}