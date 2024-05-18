-- ******************************************************************
--@file         Main.lua
--@brief        lua主入口
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:25
-- ******************************************************************
local require = require
--核心机制
require "Framework.Core.Class" --面向对象
require "Framework.Core.SingleTon" --单例
require "Framework.Core.Event" --事件
Coroutine = require "Framework.Core.Coroutine" --协程
---@type Event
Event = Event:GetInstance()
require "Framework.Def.EventDef" --事件定义
require "Framework.Core.Updater" --引擎生命周期
--unity数据结构
Mathf = require "Framework.Struct.UnityEngine.Mathf"
Vector2 = require "Framework.Struct.UnityEngine.Vector2"
Vector3 = require "Framework.Struct.UnityEngine.Vector3"
Vector4 = require "Framework.Struct.UnityEngine.Vector4"
Quaternion = require "Framework.Struct.UnityEngine.Quaternion"
Color = require "Framework.Struct.UnityEngine.Color"
Ray = require "Framework.Struct.UnityEngine.Ray"
Bounds = require "Framework.Struct.UnityEngine.Bounds"
RaycastHit = require "Framework.Struct.UnityEngine.RaycastHit"
Touch = require "Framework.Struct.UnityEngine.Touch"
LayerMask = require "Framework.Struct.UnityEngine.LayerMask"
Plane = require "Framework.Struct.UnityEngine.Plane"
Time = require "Framework.Struct.UnityEngine.Time"
Object = require "Framework.Struct.UnityEngine.Object"
--自定义数据结构
Queue = require "Framework.Struct.Queue"
Stack = require "Framework.Struct.Stack"
--基础扩展
require "Framework.Extension.TableEx" --表扩展
require "Framework.Extension.StringEx" --字符串扩展
--全局工具
require "Framework.Util.LuaUtil" --杂项
require "Framework.Util.AssertUtil" --断言
require "Framework.Util.Slot" --闭包缓存
require "Framework.Util.Debug" --日志
--UI
require "Framework.UIBase.UIBaseCtrl"
require "Framework.UIBase.UIBaseModel"
require "Framework.UIBase.UIBaseView"
require "Framework.Def.UIWindowDef"
--游戏逻辑框架
---@type Yu.GameManager
GameManager = CS.Yu.GameManager.Instance
---@type Yu.ConfigManager
ConfigManager = CS.Yu.ConfigManager
---@type Yu.AssetManager
AssetManager = CS.Yu.AssetManager.Instance
---@type Yu.SaveManager
SaveManager = CS.Yu.SaveManager
---@type Yu.SceneManager
SceneManager = CS.Yu.SceneManager.Instance
---@type Yu.SFXManager
SFXManager = CS.Yu.SFXManager.Instance
---@type Yu.BGMManager
BGMManager = CS.Yu.BGMManager.Instance
---@type Yu.InputManager
InputManager=CS.Yu.InputManager.Instance
---@type Yu.CameraManager
CameraManager = CS.Yu.CameraManager.Instance
require "Framework.UIManager" --UI管理器
---@type UIManager
UIManager = UIManager:GetInstance()
Utils = CS.Yu.Utils