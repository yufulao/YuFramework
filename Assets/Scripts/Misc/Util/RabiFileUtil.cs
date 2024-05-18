// ******************************************************************
//       /\ /|       @file       RabiFileUtil.cs
//       \ V/        @brief      文件IO操作工具
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |                    
//      /  \\        @Modified   2022-03-14 10:07:36
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RabiFileUtil
{
    /// <summary>
    /// 获取unity本地资源目录
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public static string GetLocalAssetFolder(string assetPath)
    {
        var assetFolderIndex = assetPath.IndexOf("Assets", StringComparison.Ordinal);
        //相对Assets目录
        return assetPath.Substring(assetFolderIndex).Replace('\\', '/');
    }

    /// <summary>
    /// 获取所有非meta文件的地址
    /// </summary>
    /// <param name="modelFolder"></param>
    /// <returns></returns>
    public static List<string> GetAssetPaths(string modelFolder)
    {
        //全部目录
        var allFolders = GetAllFolders(modelFolder);
        var assetPathList = new List<string>();
        foreach (var filePaths in allFolders.Select(Directory.GetFiles))
        {
            assetPathList.AddRange(filePaths.Where(t => !t.EndsWith(".meta")));
        }

        return assetPathList;
    }

    /// <summary>
    /// 在某个目录获取指定后缀的文件
    /// </summary>
    /// <param name="folder">目录</param>
    /// <param name="extensions">后缀名数组</param>
    /// <param name="exclude">是否排除</param>
    /// <returns></returns>
    public static string[] GetSpecifyFilesInFolder(string folder, string[] extensions = null, bool exclude = false)
    {
        if (string.IsNullOrEmpty(folder))
        {
            return null;
        }

        if (extensions == null)
        {
            return Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
        }

        return Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
            .Where(f => !exclude && extensions.Contains(Path.GetExtension(f).ToLower())).ToArray();
    }

    /// <summary>
    /// 安全删除目录
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static bool SafeDeleteFolder(string folder)
    {
        try
        {
            if (string.IsNullOrEmpty(folder))
            {
                return true;
            }

            if (Directory.Exists(folder))
            {
                DeleteDirectory(folder);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SafeDeleteDir failed! path = {folder} with err: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 删除目录
    /// </summary>
    /// <param name="dirPath"></param>
    private static void DeleteDirectory(string dirPath)
    {
        var files = Directory.GetFiles(dirPath);
        var dirs = Directory.GetDirectories(dirPath);

        foreach (var file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(dirPath, false);
    }

    /// <summary>
    /// 安全删除文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool SafeDeleteFile(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return true;
            }

            if (!File.Exists(filePath))
            {
                return true;
            }

            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 安全重命名
    /// </summary>
    /// <param name="sourceFileName"></param>
    /// <param name="destFileName"></param>
    /// <returns></returns>
    public static bool SafeRenameFile(string sourceFileName, string destFileName)
    {
        try
        {
            if (string.IsNullOrEmpty(sourceFileName))
            {
                return false;
            }

            if (!File.Exists(sourceFileName))
            {
                return true;
            }

            SafeDeleteFile(destFileName);
            File.SetAttributes(sourceFileName, FileAttributes.Normal);
            File.Move(sourceFileName, destFileName);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SafeRenameFile failed! path = {sourceFileName} with err: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取某个目录下全部子目录和文件路径
    /// </summary>
    /// <param name="modelFolder"></param>
    /// <returns></returns>
    public static List<string> GetAssetPathsAndFolders(string modelFolder)
    {
        //全部目录
        var allFolders = GetAllFolders(modelFolder);
        var assetPathList = new List<string>();
        foreach (var filePaths in allFolders.Select(Directory.GetFiles))
        {
            assetPathList.AddRange(filePaths.Where(t => !t.EndsWith(".meta")));
        }

        assetPathList.AddRange(allFolders);
        return assetPathList;
    }

    /// <summary>
    /// 获取某个目录下全部子目录和文件路径
    /// </summary>
    /// <param name="modelFolder"></param>
    /// <param name="suffix">匹配的后缀名</param>
    /// <returns></returns>
    public static List<string> GetAssetPathList(string modelFolder, string suffix)
    {
        var assetPathList = new List<string>();
        var filePaths = Directory.GetFiles(modelFolder);
        assetPathList.AddRange(filePaths.Where(t => t.EndsWith($".{suffix}")));
        return assetPathList;
    }

    /// <summary>
    /// 获取全部目录名称列表
    /// </summary>
    /// <param name="modelFolder"></param>
    /// <returns></returns>
    public static List<string> GetAllFolders(string modelFolder)
    {
        var allDirs = new List<string> {GetLocalFolderOrPath(modelFolder)};
        var subDirList = GetSubFolders(modelFolder);
        allDirs.AddRange(subDirList);
        return allDirs;
    }

    /// <summary>
    /// 移除BOM
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] RemoveBom(byte[] data)
    {
        if (data == null)
        {
            return null;
        }

        var bom = Encoding.UTF8.GetPreamble();
        if (data.Length > bom.Length)
        {
            for (var i = 0; i < bom.Length; i++)
            {
                if (data[i] != bom[i])
                {
                    return data;
                }
            }
        }

        var ret = new byte[data.Length - 3];
        Array.Copy(data, 3, ret, 0, data.Length - 3);
        return ret;
    }

    /// <summary>
    /// 递归获取所有子文件夹目录 不包含自身
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    private static IEnumerable<string> GetSubFolders(string folder) // 递归获取所有子文件夹目录
    {
        var subFolders = new List<string>();
        if (string.IsNullOrEmpty(folder))
        {
            return subFolders;
        }

        var folderArray = Directory.GetDirectories(folder);
        if (folderArray.Length <= 0)
        {
            return subFolders;
        }

        subFolders.AddRange(folderArray.Select(GetLocalFolderOrPath));
        foreach (var t in folderArray)
        {
            var list = GetSubFolders(t);
            subFolders.AddRange(list);
        }

        return subFolders;
    }

    /// <summary>
    /// 把资源路径格式转变为Unity本地路径
    /// </summary>
    /// <param name="fullFolder"></param>
    /// <returns></returns>
    private static string GetLocalFolderOrPath(string fullFolder) // 路径会含有反斜杠 所以全部替换掉
    {
        if (string.IsNullOrEmpty(fullFolder))
        {
            return string.Empty;
        }

        fullFolder = fullFolder.Replace("\\", "/");
        var index = fullFolder.IndexOf("Assets/", StringComparison.CurrentCultureIgnoreCase);
        return index < 0 ? fullFolder : fullFolder.Substring(index);
    }
}