-- ******************************************************************
--       /\ /|       @file       Class.lua
--       \ V/        @brief      lua面向对象模拟
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2021-04-27 16:59
--    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
-- ******************************************************************
local assert = assert
local type = type
local ipairs = ipairs
local rawget = rawget
local table = table
local setmetatable = setmetatable
local unpack = unpack or table.unpack

---@brief 定义一个类
---@public
---@param className string 类名字
---@param superClass table 父类
function Class(className, superClass)
    assert(type(className) == 'string', "className类型为string, 当前为" .. type(className))
    assert(type(superClass) == 'table' or type(superClass) == 'nil', "superClass类型为table, 当前为" .. type(superClass))
    local class = {}
    class.className = className
    class.superClass = superClass
    --设置实例创建方法
    local metaTable = {
        __call = function(self, dataTable)
            return Create(self, dataTable)
        end,
        __index = superClass
    }
    setmetatable(class, metaTable)
    return class
end

---@brief 销毁实例
---@public
function DestroyInstance(instance)
    assert(type(instance) == 'table', "instance类型为table, 当前为" .. type(instance))
    local superClassList = GetSuperClassList(instance.class)
    for _, superClass in ipairs(superClassList) do
        --存在析构函数
        local __delete = rawget(superClass, "__delete")
        if __delete then
            __delete(instance)
        end
    end
end

---@brief 创建实例
---@private
function Create(class, dataTable)
    dataTable = dataTable or {}
    assert(type(class) == 'table', "superClass类型为table, 当前为" .. type(class))
    local instance = {} --创建类实例
    instance.class = class
    --设置类的方法为实例的元表
    setmetatable(instance, {
        __index = class,
    })
    local superClassList = GetSuperClassList(instance.class)
    for _, superClass in ipairs(superClassList) do
        --存在构造函数
        local __init = rawget(superClass, "__init")
        if __init then
            __init(instance, unpack(dataTable))
        end
    end
    return instance
end

---@brief 获取父类列表
---@private
function GetSuperClassList(class)
    local t = {}
    if not class then
        return
    end
    local currentClass = class
    repeat
        table.insert(t, 1, currentClass)
        currentClass = currentClass.superClass
    until not currentClass
    return t
end