-- ******************************************************************
--@file         Debug.lua
--@brief        转交unity的debug
--@author       yufulao, yufulao@qq.com
--@createTime   2024/5/18 1:25
-- ******************************************************************
---@brief
---@class Debug
Debug = {}

---@brief
---@public
function Debug.Log(msg)
    CS.UnityEngine.Debug.Log(msg)
end

---@brief
---@public
function Debug.LogError(msg)
    CS.UnityEngine.Debug.LogError(msg)
end

---@brief
---@public
function Debug.LogWarning(msg)
    CS.UnityEngine.Debug.LogWarning(msg)
end

return Debug