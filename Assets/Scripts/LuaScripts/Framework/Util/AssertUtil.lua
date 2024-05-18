-- ******************************************************************
--       /\ /|       @file       AssertUtil.lua
--       \ V/        @brief      断言工具类
--       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
--       /  |                    
--      /  \\        @Modified   2020/10/15 18:57
--    *(__\_\        @Copyright  Copyright (c) 2020, Shadowrabbit
-- ******************************************************************
local assert = assert
local type = type
local formatEx = string.formatEx
local string = string

---@class AssertUtils
AssertUtils = {}

---@brief
---@public
function AssertUtils.AssertIsNumber(msg, value)
    AssertUtils.AssertIsType(msg, 'number', value)
end

---@brief
---@public
function AssertUtils.AssertIsNumberOrNil(msg, value)
    AssertUtils.AssertIsTypeOrNil(msg, 'number', value)
end

---@brief
---@public
function AssertUtils.AssertIsString(msg, value)
    AssertUtils.AssertIsType(msg, 'string', value)
end

---@brief
---@public
function AssertUtils.AssertIsStringOrNil(msg, value)
    AssertUtils.AssertIsTypeOrNil(msg, 'string', value)
end

---@brief
---@public
function AssertUtils.AssertIsBoolean(msg, value)
    AssertUtils.AssertIsType(msg, 'boolean', value)
end

---@brief
---@public
function AssertUtils.AssertIsBooleanOrNil(msg, value)
    AssertUtils.AssertIsTypeOrNil(msg, 'boolean', value)
end

---@brief
---@public
function AssertUtils.AssertIsTable(msg, value)
    AssertUtils.AssertIsType(msg, 'table', value)
end

---@brief
---@public
function AssertUtils.AssertIsTableOrNil(msg, value)
    AssertUtils.AssertIsTypeOrNil(msg, 'table', value)
end

---@brief
---@public
function AssertUtils.AssertIsFunction(msg, value)
    AssertUtils.AssertIsType(msg, 'function', value)
end

---@brief
---@public
function AssertUtils.AssertIsFunctionOrNil(msg, value)
    AssertUtils.AssertIsTypeOrNil(msg, 'function', value)
end

---@brief
---@public
function AssertUtils.AssertIsThread(msg, value)
    AssertUtils.AssertIsType(msg, 'thread', value)
end

---@brief
---@public
function AssertUtils.AssertIsThreadOrNil(msg, value)
    AssertUtils.AssertIsTypeOrNil(msg, 'thread', value)
end

---@brief
---@public
function AssertUtils.AssertIsNotNil(msg, value)
    AssertUtils.AssertIsNotType(msg, 'nil', value)
end

---@brief
---@public
function AssertUtils.AssertIsNotNilOrEmpty(msg, value)
    AssertUtils.AssertIsString(msg, value)
    assert(not string.isNullOrEmpty(value),
            formatEx("{0} expected: not nil or empty, actual: {1}", msg, value))
end

---@brief
---@public
function AssertUtils.AssertIsEqualsValue(msg, expectedValue, actualValue)
    AssertUtils.AssertIsString("msg", msg)
    assert(expectedValue == actualValue,
            formatEx("{0}, expected: value {1}, actual: {2}", msg, expectedValue, actualValue))
end

---@brief
---@public
function AssertUtils.AssertIsNotEqualsValue(msg, expectedValue, actualValue)
    AssertUtils.AssertIsString("msg", msg)
    assert(type(expectedValue) == type(actualValue),
            formatEx("{0}, expected a {1} value, actual: {2}", expectedValue, type(expectedValue), actualValue))
    assert(expectedValue ~= actualValue,
            formatEx("{0}, expected: not value {1}, actual: {2}", msg, expectedValue, actualValue))
end

---@brief
---@public
function AssertUtils.AssertIsMoreThan(msg, expectedValue, actualValue)
    AssertUtils.AssertIsString("msg", msg)
    AssertUtils.AssertIsNumber("expectedValue", expectedValue)
    AssertUtils.AssertIsNumber("actualValue", actualValue)
    assert(actualValue > expectedValue,
            formatEx("{0}, expected: a value more than {1}, actual: {2}", msg, expectedValue, actualValue))
end

---@brief
---@public
function AssertUtils.AssertIsLessThan(msg, expectedValue, actualValue)
    AssertUtils.AssertIsString("msg", msg)
    AssertUtils.AssertIsNumber("expectedValue", expectedValue)
    AssertUtils.AssertIsNumber("actualValue", actualValue)
    assert(actualValue < expectedValue,
            formatEx("{0}, expected: a value less than {1}, actual: {2}", msg, expectedValue, actualValue))
end

---@brief 断言value在某个区间
---@public
---@param msg string 参数名称
---@param min number 最小值
---@param max number 最大值
---@param actualValue number 实际值 
---@param hasBound boolean 是否包含边界 
function AssertUtils.AssertIsInRange(msg, min, max, actualValue, hasBound)
    AssertUtils.AssertIsString("msg", msg)
    AssertUtils.AssertIsNumber("min", min)
    AssertUtils.AssertIsNumber("amx", max)
    AssertUtils.AssertIsNumber("actualValue", actualValue)
    AssertUtils.AssertIsBooleanOrNil("hasBound", hasBound)
    if hasBound then
        assert(actualValue >= min and actualValue <= max,
                formatEx("{0}, expected in [{1},{2}], actual: {3}", msg, min, max, actualValue))
        return
    end
    assert(assert(actualValue > min and actualValue < max),
            formatEx("{0}, expected in ({1},{2}), actual: {3}", msg, min, max, actualValue))
end

---@brief 断言value是某个类型
---@private
---@param msg string 参数名称
---@param expectedType string 期望类型
---@param value any 值
function AssertUtils.AssertIsType(msg, expectedType, value)
    assert(type(msg) == 'string',
            formatEx("msg, expected: a string value , actual: {0}", type(msg)))
    assert(type(expectedType) == 'string',
            formatEx("expectedType, expected: a string value, actual: {0}", type(expectedType)))
    assert(type(value) == expectedType,
            formatEx("{0}, expected: a {1} value, actual: {2}", msg, expectedType, type(value)))
end

---@brief 断言value是某个类型或nil
---@private
---@param msg string 参数名称
---@param expectedType string 期望类型
---@param value any 值
function AssertUtils.AssertIsTypeOrNil(msg, expectedType, value)
    assert(type(msg) == 'string',
            formatEx("msg, expected: a string value , actual: {0}", type(msg)))
    assert(type(expectedType) == 'string',
            formatEx("expectedType, expected: a string value, actual: {0}", type(expectedType)))
    assert(type(value) == expectedType or 'nil',
            formatEx("{0}, expected: a {1} value or nil, actual: {2}", msg, expectedType, type(value)))
end

---@brief 断言value是某个类型或nil
---@private
---@param msg string 参数名称
---@param expectedType string 不期望类型
---@param value any 值
function AssertUtils.AssertIsNotType(msg, expectedType, value)
    assert(type(msg) == 'string',
            formatEx("msg, expected: a string value , actual: {0}", type(msg)))
    assert(type(expectedType) == 'string',
            formatEx("expectedType, expected: a string value, actual: {0}", type(expectedType)))
    assert(type(value) ~= expectedType,
            formatEx("{0}, expected: not a {1} value, actual: {2}", msg, expectedType, type(value)))
end

return AssertUtils