-- ******************************************************************
--       /\ /|       @file       Slot.lua
--       \ V/        @brief      闭包绑定
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2021-01-08 14:52
--    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
-- ******************************************************************
local assert, setmetatable, pairs = assert, setmetatable, pairs
local unpack = unpack or table.unpack
local table, ipairs = table, ipairs
local print = print
local __kmt = { __mode = "k" }
local __vmt = { __mode = "v" }
local __slots = {}
local __slots_cache = { cache = setmetatable({}, __kmt) }
local __parameterized_slots_cache = { cache = setmetatable({}, __kmt) }
local __slots_with_runner_cache = { cache = setmetatable({}, __kmt) }
local __parameterized_slots_with_runner_cache = { cache = setmetatable({}, __kmt) }
local __cache_args = {}
setmetatable(__slots, __vmt)
setmetatable(__slots_cache, __kmt)
setmetatable(__parameterized_slots_cache, __kmt)
setmetatable(__slots_with_runner_cache, __kmt)
setmetatable(__parameterized_slots_with_runner_cache, __kmt)
--- PRIVATE METHOD------------------------------------------------------------------------------------------------------
---@brief 打印缓存队列
---@param root table
---@param level number
---@private
local function __Dump(root, level)
    for k, v in pairs(root) do
        print(("%s key: %s, cache: %s, slot: %s"):format(('    '):rep(level), k, v.cache, v.slot))
        if v.cache then
            __Dump(v.cache, level + 1)
        end
    end
end

---@brief 尝试从缓存中获取Slot
---@param root table
---@return function or nil
---@private
local function TryGetSlotFromCache(root, ...)
    local found = true
    local iter = { ... }
    local i, cnt = 0, #iter
    local ctx = root
    while i < cnt do
        i = i + 1
        assert(iter[i] ~= nil, "'nil' cannot use as a slot param.")
        ctx = root[iter[i]]
        if not ctx then
            found = false
            break
        end
        root = ctx.cache
    end

    if found and i == cnt then
        return __slots[ctx]
    end
end

---@brief 保存Slot到缓存中
---@param slot function
---@vararg
---@private
local function SaveSlotToCache(root, slot, ...)
    local iter = { ... }
    local i, cnt = 0, #iter
    local ctx = root
    while i < cnt do
        i = i + 1
        ctx = root[iter[i]]
        if not ctx then
            ctx = setmetatable({ cache = setmetatable({}, __kmt) }, __kmt)
            root[iter[i]] = ctx
        end
        root = ctx.cache
    end
    __slots[ctx] = slot
end

---@brief 参数合并
---@param arg1 table
---@param arg2 table
local function MergeArgs(arg1, arg2)
    for k in pairs(__cache_args) do
        __cache_args[k] = nil
    end
    for _, v in ipairs(arg1) do
        table.insert(__cache_args, v)
    end
    for _, v in ipairs(arg2) do
        table.insert(__cache_args, v)
    end
    return __cache_args
end
--- PUBLIC METHOD-------------------------------------------------------------------------------------------------------
---@brief 生成类成员函数闭包
---@param inst table 类实例
---@param method function
---@return function
function Slot(inst, method)
    assert(inst, "Slot() - param 'inst' cannot be nil.")
    assert(method, "Slot() - param 'method' cannot be nil.")

    local slot = TryGetSlotFromCache(__slots_cache, inst, method)
    if slot then
        return slot
    end

    slot = function(...)
        return method(inst, ...)
    end
    SaveSlotToCache(__slots_cache, slot, inst, method)
    return slot
end

---@brief 生成可指定参数的类成员函数闭包
---@param inst table 类实例
---@param method function
---@vararg
---@return function
function ParameterizedSlot(inst, method, ...)
    assert(inst, "ParameterizedSlot() - param 'inst' cannot be nil.")
    assert(method, "ParameterizedSlot() - param 'method' cannot be nil.")

    local args = { ... }

    local slot = TryGetSlotFromCache(__parameterized_slots_cache, inst, method, ...)
    if slot then
        return slot
    end

    slot = function(...)
        return method(inst, unpack(MergeArgs(args, { ... })))
    end
    SaveSlotToCache(__parameterized_slots_cache, slot, inst, method, ...)
    return slot
end

---@brief 生成类成员函数闭包, 并且使用指定执行者执行
---@param inst table 类实例
---@param method function
---@param runner function
---@return function
function SlotWithRunner(inst, method, runner)
    assert(inst, "Slot() - param 'inst' cannot be nil.")
    assert(method, "Slot() - param 'method' cannot be nil.")

    local slot = TryGetSlotFromCache(__slots_with_runner_cache, inst, method, runner)
    if slot then
        return slot
    end

    local wrapper = function(...)
        return method(inst, ...)
    end

    slot = function(...)
        return runner(wrapper, ...)
    end
    SaveSlotToCache(__slots_with_runner_cache, slot, inst, method, runner)
    return slot
end

---@brief 生成可指定参数的类成员函数闭包, 并且使用指定执行者执行
---@param inst table 类实例
---@param method function
---@param runner function
---@vararg
---@return function
function ParameterizedSlotWithRunner(inst, method, runner, ...)
    assert(inst, "ParameterizedSlotWithRunner() - param 'inst' cannot be nil.")
    assert(method, "ParameterizedSlotWithRunner() - param 'method' cannot be nil.")

    local args = { ... }

    local slot = TryGetSlotFromCache(__parameterized_slots_with_runner_cache, inst, method, runner, ...)
    if slot then
        return slot
    end

    local wrapper = function(...)
        return method(inst, unpack(MergeArgs(args, { ... })))
    end

    slot = function(...)
        return runner(wrapper, ...)
    end
    SaveSlotToCache(__parameterized_slots_with_runner_cache, slot, inst, method, runner, ...)
    return slot
end

---@brief 打印缓存队列
---@param t string "Slot" | "ParameterizedSlot" | "SlotWithRunner" | "ParameterizedSlotWithRunner"
---@public
function DumpSlots(t)
    if t == "Slot" then
        print("====Dump Slot Begin====")
        __Dump(__slots_cache, 1)
        print("====Dump Slot Ended====")
    elseif t == "ParameterizedSlot" then
        print("====Dump Parameterized Slot Begin====")
        __Dump(__parameterized_slots_cache, 1)
        print("====Dump Parameterized Slot Ended====")
    elseif t == "SlotWithRunner" then
        print("====Dump Slot With Runner Begin====")
        __Dump(__slots_with_runner_cache, 1)
        print("====Dump Slot With Runner Ended====")
    elseif t == "ParameterizedSlotWithRunner" then
        print("====Dump Parameterized Slot With Runner Begin====")
        __Dump(__parameterized_slots_with_runner_cache, 1)
        print("====Dump Parameterized Slot With Runner Ended====")
    end
end