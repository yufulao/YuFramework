-- ******************************************************************
--@file         UIBaseCtrl.lua
--@brief        窗口controller基类
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:22
-- ******************************************************************
---@brief
---@class UIBaseCtrl
UIBaseCtrl = Class("UIBaseCtrl")

---@brief
---@protected
function UIBaseCtrl:__init(model, view)
    ---@type UIBaseView
    self.view = view
    ---@type UIBaseModel
    self.model = model
end

---@brief
---@protected
function UIBaseCtrl:__delete()
    DestroyInstance(self.view)
    DestroyInstance(self.model)
end

---@brief 初始化回调
---@public
function UIBaseCtrl:OnInit(...)
end

---@brief 打开窗口
---@public
function UIBaseCtrl:OpenRoot(...)
end

---@brief 关闭窗口
---@public
function UIBaseCtrl:CloseRoot()
end

---@brief 绑定事件
---@public
function UIBaseCtrl:BindEvent()
end

return UIBaseCtrl