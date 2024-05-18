// ******************************************************************
//       /\ /|       @file       LuaUpdater.cs
//       \ V/        @brief      用于向lua传递生命周期回调
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2022-05-19 04:50:38
//    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
// ******************************************************************

using System;
using UnityEngine;
using XLua;

public class LuaUpdater : MonoBehaviour
{
    private Action _luaUpdate;
    private Action _luaLateUpdate;
    private Action _luaFixedUpdate;

    public void OnInit(LuaEnv luaEnv)
    {
        _luaUpdate = luaEnv.Global.Get<Action>("Update");
        _luaLateUpdate = luaEnv.Global.Get<Action>("LateUpdate");
        _luaFixedUpdate = luaEnv.Global.Get<Action>("FixedUpdate");
    }

    private void Update()
    {
        if (_luaUpdate == null)
        {
            return;
        }
        try
        {
            _luaUpdate();
        }
        catch (Exception ex)
        {
            Debug.LogError("luaUpdate err : " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    private void LateUpdate()
    {
        if (_luaLateUpdate == null) return;
        try
        {
            _luaLateUpdate();
        }
        catch (Exception ex)
        {
            Debug.LogError("luaLateUpdate err : " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    private void FixedUpdate()
    {
        if (_luaFixedUpdate == null) return;
        try
        {
            _luaFixedUpdate();
        }
        catch (Exception ex)
        {
            Debug.LogError("luaFixedUpdate err : " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    public void Clear()
    {
        _luaUpdate = null;
        _luaLateUpdate = null;
        _luaFixedUpdate = null;
    }
}
