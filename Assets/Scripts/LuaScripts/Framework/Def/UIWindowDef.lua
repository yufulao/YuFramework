-- ******************************************************************
--@file         UIWindowDef.lua
--@brief        窗口定义
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:21
-- ******************************************************************
---@brief
---@class UIWindowDef
UIWindowDef = {
    LoadingView = {
        ctrl = require "UILogic/Loading/LoadingCtrl",
        model = require "UILogic/Loading/LoadingModel",
        view = require "UILogic/Loading/LoadingView",
    },
}

return UIWindowDef