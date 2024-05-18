 -- ******************************************************************
--       /\ /|       @file       Event.lua
--       \ V/        @brief      事件
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2022-05-19 21:10
--    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
-- ******************************************************************

local setmetatable = setmetatable
local table = table
local pairs = pairs

---@brief
---@class Event : SingleTon
Event = Class("Event", SingleTon)

---@brief
---@protected
function Event:__init()
    self.events = {} --eventKey,funcList
end

---@brief
---@protected
function Event:__delete()
    self.events = nil
end

---@brief 添加监听
---@public
function Event:AddListener(eventKey, func)
    if (eventKey == nil or func == nil) then
        return
    end
    if (self.events[eventKey] == nil) then
        self.events[eventKey] = setmetatable({}, { __mode = "k" }) --初始化
    end
    table.insert(self.events[eventKey], func) --添加事件
end

---@brief 撤销监听
---@public
function Event:RemoveListener(eventKey, func)
    if (eventKey == nil or func == nil) then
        return
    end
    local funcList = self.events[eventKey]
    if not funcList then
        return
    end
    for _, v in pairs(funcList or {}) do
        if (v == func) then
            --撤销事件
            table.removebyvalue(funcList,v)
        end
    end
end

---@brief 通过eventKey移除
---@public
function Event:RemoveByEventKey(eventKey)
    if (eventKey == nil) then
        return
    end
    self.events[eventKey] = nil
end

---@brief 派送事件
---@public
function Event:Dispatch(eventKey, ...)
    if not eventKey then
        return
    end
    if not self.events[eventKey] then
        return
    end
    local funcList = self.events[eventKey]
    for _, func in pairs(funcList) do
        if func then
            func(...)
        end
    end
end

---@brief 清理
---@public
function Event:Clear()
    self.events = {}
end

return Event