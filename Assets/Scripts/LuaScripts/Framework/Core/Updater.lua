-- ******************************************************************
--       /\ /|       @file       Updater.lua
--       \ V/        @brief      负责派送Unity层的引擎事件
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2022-05-20 12:10
--    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
-- ******************************************************************

---@brief
---@private
function Update()
    Event:Dispatch(EventDef.Update)
end

---@brief
---@private
function LateUpdate()
    Event:Dispatch(EventDef.LateUpdate)
end

---@brief
---@private
function FixedUpdate()
    Event:Dispatch(EventDef.FixedUpdate)
end