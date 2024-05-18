-- ******************************************************************
--@file         LoadingView.lua
--@brief        加载界面的view
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:26
-- ******************************************************************
---@brief
---@class LoadingView : UIBaseView
LoadingView = Class("LoadingView", UIBaseView)

---@brief 初始化
---@public
function LoadingView:OnInit(...)
    self.animator = self.transform:GetComponent(typeof(CS.UnityEngine.Animator))
    self.mainCanvasGroup = self.transform:Find("Main"):GetComponent(typeof(CS.UnityEngine.CanvasGroup))
end

---@brief 打开窗口
---@public
function LoadingView:OpenRoot(...)
    self.mainCanvasGroup.alpha = 0;
    self.gameObject:SetActive(true);
    GameManager:StartCoroutine(Utils.PlayAnimation(self.animator, "LoadingEnter"));
end

---@brief 关闭窗口
---@public
function LoadingView:CloseRoot()
    Coroutine.start(function() self:CloseRootIEnumerator() end)
end

---@brief 关闭窗口的协程
---@private
function LoadingView:CloseRootIEnumerator()
    self.mainCanvasGroup.alpha = 1
    coroutine.yield(Utils.PlayAnimation(self.animator, "LoadingExit"))
    self.gameObject:SetActive(false)
end



return LoadingView