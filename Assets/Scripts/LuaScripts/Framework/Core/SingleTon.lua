-- ******************************************************************
--       /\ /|       @file       SingleTon.lua
--       \ V/        @brief      单例
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2022-05-20 12:37
--    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
-- ******************************************************************

---@brief
---@class SingleTon
SingleTon = Class("SingleTon")

---@brief 获取实例
---@public
---@return SingleTon
function SingleTon:GetInstance()
    self.instance = self.instance or self()
    return self.instance
end

return SingleTon