-- ******************************************************************
--       /\ /|       @file       Object.lua
--       \ V/        @brief      
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2022-05-26 18:17
--    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
-- ******************************************************************

---@brief 判断CS对象是否为null 注意transform传进来肯定是false
---@public
function IsNull(obj)
    local isNull = CS.Rabi.EngineEx.GameObjectEx.IsNull(obj)
    return isNull
end