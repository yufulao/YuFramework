-- ******************************************************************
--@file         UIBaseView.lua
--@brief        窗口view基类
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:23
-- ******************************************************************
---@brief
---@class UIBaseView
UIBaseView = Class("UIBaseView")

---@brief
---@protected
function UIBaseView:__init(rootObj)
    ---@type UnityEngine.GameObject
    self.gameObject=rootObj
    ---@type UnityEngine.Transform
    self.transform = rootObj.transform
end

---@brief 初始化
---@public
function UIBaseView:OnInit(...)
end

---@brief 打开窗口
---@public
function UIBaseView:OpenRoot(...)
end

---@brief 打开窗口
---@public
function UIBaseView:CloseRoot()
end

return UIBaseView