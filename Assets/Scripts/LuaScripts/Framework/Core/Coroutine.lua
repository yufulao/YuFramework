-- ******************************************************************
--@file         Coroutine.lua
--@brief        lua协程实现
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:20
-- ******************************************************************
local unpack = unpack or table.unpack
---@type UnityEngine.GameObject
local coroutineRunnerObj = CS.UnityEngine.GameObject('CoroutineRunner')
CS.UnityEngine.Object.DontDestroyOnLoad(coroutineRunnerObj)
local coroutineRunner = coroutineRunnerObj:AddComponent(typeof(CS.XLuaTest.Coroutine_Runner))
local move_end = {}
local generator_mt = {
    __index = {
        MoveNext = function(self)
            self.Current = self.co()
            if self.Current == move_end then
                self.Current = nil
                return false
            else
                return true
            end
        end;
        Reset = function(self)
            self.co = coroutine.wrap(self.w_func)
        end
    }
}

local function csGenerator(func, ...)
    local params = {...}
    local generator = setmetatable({
        w_func = function()
            func(unpack(params))
            return move_end
        end
    }, generator_mt)
    generator:Reset()
    return generator
end

return {
    start = function(...)
        return coroutineRunner:StartCoroutine(csGenerator(...))
    end;

    stop = function(coroutine)
        coroutineRunner:StopCoroutine(coroutine)
    end
}
