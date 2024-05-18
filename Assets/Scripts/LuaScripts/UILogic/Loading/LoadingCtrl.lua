-- ******************************************************************
--@file         LoadingCtrl.lua
--@brief        加载窗口的controller
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:25
-- ******************************************************************
---@brief
---@class LoadingCtrl : UIBaseCtrl
LoadingCtrl = Class("LoadingCtrl", UIBaseCtrl)

---@brief 初始化回调
---@public
function LoadingCtrl:OnInit(...)
end

---@brief 打开窗口
---@public
function LoadingCtrl:OpenRoot(...)
    self.view:OpenRoot(...)
end

---@brief 关闭窗口
---@public
function LoadingCtrl:CloseRoot()
    self.view:CloseRoot()
end

---@brief 绑定事件
---@public
function LoadingCtrl:BindEvent()
end


return LoadingCtrl