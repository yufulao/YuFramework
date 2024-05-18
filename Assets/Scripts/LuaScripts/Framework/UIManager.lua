-- ******************************************************************
--@file         UIManager.lua
--@brief        lua层UI管理器
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:25
-- ******************************************************************
local formatEx = string.formatEx
local pairs = pairs

local UIWindowDef = require "Framework.Def.UIWindowDef"

---@brief
---@class UIManager : SingleTon
UIManager = Class("UIManager", SingleTon)

---@brief
---@protected
function UIManager:__init()
    self.CfgUI = ConfigManager.Tables.CfgUI;
    ---@type UnityEngine.Transform
    self.UIRoot = CS.UnityEngine.GameObject.Find("UIRoot").transform --使用UI相机渲染的UI根节点
    self.Layers = {}
    self.Layers["SceneLayer"] = CS.UnityEngine.GameObject.Find("SceneLayer").transform
    self.Layers["NormalLayer"] = CS.UnityEngine.GameObject.Find("NormalLayer").transform
    self.Layers["TopLayer"] = CS.UnityEngine.GameObject.Find("TopLayer").transform
    self.LayerStacks = {}
    self.LayerStacks["SceneLayer"] = Stack()
    self.LayerStacks["NormalLayer"] = Stack()
    self.LayerStacks["TopLayer"] = Stack()
    self.AllWindow = {}
end

---@brief
---@protected
function UIManager:__delete()
    self:CloseAllWindows()
end

---@brief 获取ui的controller
---@public
function UIManager:GetCtrl(windowName, params)
    if self.AllWindow[windowName] then
        return self.AllWindow[windowName]
    end

    return self:CreatNewView(windowName, params)
end

---@brief 打开页面
---@public
function UIManager:OpenWindow(windowName, params)
    local ctrl=self:GetCtrl(windowName,params)
    self.LayerStacks[self.CfgUI:Get(windowName).Layer]:Push(ctrl)
    --Debug.Log(ctrl.view.transform)
    ctrl:OpenRoot(SafeUnpack(params));
end

---@brief 关闭页面
---@public
function UIManager:CloseWindow(windowName)
    local ctrl = self.AllWindow[windowName]
    if not ctrl then
        return
    end
    local layerName = self.CfgUI:Get(windowName).Layer
    ---@type Stack
    local stack = self.LayerStacks[layerName]
    local stackCount = stack:GetCount()
    for i = 1, stackCount do
        local ctrlBefore = stack:Pop(1)
        if ctrlBefore == ctrl then
            break
        end
        ctrlBefore:CloseRoot()
    end
    ctrl:CloseRoot()
end

---@brief 关闭指定层级的所有页面
---@public
function UIManager:CloseAllLayerWindows(layerName)
    ---@type Stack
    local stack = self.LayerStacks[layerName]
    if stack then
        local stackCount = stack:GetCount()
        for i = 1, stackCount do
            local ctrl = stack:Pop(1)
            ctrl:CloseRoot()
        end
    end
    stack:Clear()
end

---@brief 关闭所有页面
---@public
function UIManager:CloseAllWindows()
    for _, ctrl in pairs(self.AllWindow) do
        if ctrl then
            ctrl:CloseRoot()
        end
    end

    self.AllWindow = {}
    for _, stack in pairs(self.LayerStacks) do
        stack:Clear()
    end
end

---@brief 创建一个新的ui
---@private
function UIManager:CreatNewView(windowName, params)
    local rowCfgUi = self.CfgUI:Get(windowName)
    local rootObj = CS.UnityEngine.Object.Instantiate(AssetManager:LoadAssetGameObject(rowCfgUi.UiPath), self.Layers[rowCfgUi.Layer])

    rootObj:SetActive(false)
    ---@type UnityEngine.Canvas
    local canvas = rootObj:GetComponent(typeof(CS.UnityEngine.Canvas))
    canvas.worldCamera = CameraManager:GetUICamera()
    canvas.sortingOrder = rowCfgUi.SortOrder

    --mvc结构
    local viewClass = UIWindowDef[windowName].view
    local ctrlClass = UIWindowDef[windowName].ctrl
    local modelClass = UIWindowDef[windowName].model
    if not viewClass then
        Debug.LogError(formatEx("找不到viewClass window:{0}", windowName))
        return
    end
    if not ctrlClass then
        Debug.LogError(formatEx("找不到ctrlClass window:{0}", windowName))
        return
    end
    if not modelClass then
        Debug.LogError(formatEx("找不到modelClass window:{0}", windowName))
        return
    end

    --Debug.Log("modelClass:   "..modelClass.className)
    --view和model的OnInit不执行时，请先查找有无其他脚本写错了Function的前缀，导致重写了该view或model的OnInit函数
    ---@type UIBaseModel
    local model = modelClass()
    ---@type UIBaseView
    local view = viewClass { rootObj }
    ---@type UIBaseCtrl
    local ctrl = ctrlClass { model, view }
    --Debug.Log(ctrl.view.transform)
    local safeParams=SafeUnpack(params)
    ctrl:OnInit(safeParams)
    model:OnInit(safeParams)
    view:OnInit(safeParams)
    ctrl:BindEvent()
    self.AllWindow[windowName] = ctrl
    local stack = self.LayerStacks[self.CfgUI:Get(windowName).Layer]
    stack:Push(ctrl)
    return ctrl
end

return UIManager