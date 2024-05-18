-- ******************************************************************
--       /\ /|       @file       Stack.lua
--       \ V/        @brief      栈结构
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2022-05-20 19:19
--    *(__\_\        @Copyright  Copyright (c) 2022, Shadowrabbit
-- ******************************************************************

local table = table
local next = next
local unpack = unpack or table.unpack

---@brief
---@class Stack
Stack = Class("Stack")

---@brief
---@protected
function Stack:__init()
    self.stack = {}
end

---@brief 压入
---@public
function Stack:Push(...)
    local arg = { ... }
    if next(arg) then
        for i = 1, #arg do
            table.insert(self.stack, arg[i])
        end
    end
end

---@brief 弹出
---@public
function Stack:Pop(num)
    num = num or 1
    AssertUtils.AssertIsMoreThan("num必须为正整数", 0, num)
    local temp = {}
    for _ = 1, num do
        table.insert(temp, self.stack[#self.stack])
        table.remove(self.stack)
    end
    return unpack(temp)
end

---@brief 获取数量
---@public
function Stack:GetCount()
    return #self.stack
end

---@brief
---@public
function Stack:Clear()
    self.stack = {}
end

return Stack
