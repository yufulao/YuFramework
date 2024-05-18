-- table extends
local table, pairs, type = table, pairs, type
local assert = assert
local next = next

function table.nums(t)
    local count = 0
    for _, _ in pairs(t) do
        count = count + 1
    end
    return count
end

function table.keys(hashtable)
    local keys = {}
    for k, _ in pairs(hashtable) do
        keys[#keys + 1] = k
    end
    return keys
end

function table.values(hashtable)
    local values = {}
    for _, v in pairs(hashtable) do
        values[#values + 1] = v
    end
    return values
end

function table.isarray(t)
    if type(t) ~= "table" then
        return false
    end
    if not table.issametype(table.keys(t)) then
        return false
    end
    return table.nums(t) == #t
end

function table.ishashtable(t)
    if type(t) ~= "table" then
        return false
    end
    if not table.issametype(table.keys(t)) then
        return false
    end
    return table.nums(t) ~= #t
end

function table.issametype(t)
    local kt
    for _, v in ipairs(t) do
        if not kt then
            kt = type(v)
        end
        if type(v) ~= kt then
            return false
        end
    end
    return true
end

function table.merge(dest, src)
    for k, v in pairs(src) do
        dest[k] = v
    end
    return dest
end

function table.containsKey(dictionary, key)
    for k, _ in pairs(dictionary) do
        if k == key then
            return true
        end
    end
    return false
end

function table.indexof(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then
            return i
        end
    end
    return false
end

function table.keyof(hashtable, value)
    for k, v in pairs(hashtable) do
        if v == value then
            return k
        end
    end
    return nil
end

function table.keyvalueindexof(array, key, value)
    for i = 1, #array do
        if array[i][key] == value then
            return i
        end
    end
    return false
end

function table.removebyvalue(array, value, removeall)
    local c, i, max = 0, 1, #array
    while i <= max do
        if array[i] == value then
            table.remove(array, i)
            c = c + 1
            i = i - 1
            max = max - 1
            if not removeall then
                break
            end
        end
        i = i + 1
    end
    return c
end

function table.find(t, fn)
    for key, val in pairs(t) do
        if fn(val) then
            return val, key
        end
    end
end

function table.convert(t, fn)
    for k, v in pairs(t) do
        t[k] = fn(v)
    end
    return t
end

function table.walk(t, fn)
    for k, v in pairs(t) do
        fn(v, k)
    end
end

function table.foreach(t, fn)
    for idx, v in ipairs(t) do
        fn(v, idx)
    end
end

function table.filter(t, fn)
    for k, v in pairs(t) do
        if not fn(v, k) then
            t[k] = nil
        end
    end
end

function table.pick(t, fn)
    local ret = {}
    for k, v in pairs(t) do
        if fn(v, k) then
            table.insert(ret, v)
        end
    end
    return ret
end

function table.select(t, fn)
    local tFN = type(fn)
    assert(tFN == "function" or tFN == "string")

    local ret = {}
    for k, v in pairs(t) do
        if tFN == "string" then
            table.insert(ret, v[fn])
        else
            table.insert(ret, fn(v, k))
        end
    end
    return ret
end

function table.getrange(t, startidx, endidx)
    local ret = {}
    for i = startidx, endidx do
        table.insert(ret, t[i])
    end
    return ret
end

function table.unique(t, bArray)
    local check = {}
    local n = {}
    local idx = 1
    for k, v in pairs(t) do
        if not check[v] then
            if bArray then
                n[idx] = v
                idx = idx + 1
            else
                n[k] = v
            end
            check[v] = true
        end
    end
    return n
end

function table.deepclone(t)
    local ret = {}
    for k, v in pairs(t) do
        if type(v) == "table" then
            ret[k] = table.deepclone(v)
        else
            ret[k] = v
        end
    end
    return ret
end

function table.clone(t)
    local ret = {}
    for k, v in pairs(t) do
        ret[k] = v
    end
    return ret
end

function table.elementequals(t1, t2)
    assert(type(t1) == "table", "t1 must be a table")
    assert(type(t2) == "table", "t2 must be a table")

    local cnt1 = #t1
    local cnt2 = #t2
    if cnt1 ~= cnt2 then
        return false
    end

    for i = 1, cnt1 do
        if t1[i] ~= t2[i] then
            return false
        end
    end
    return true
end

function table.invert(t)
    assert(table.isarray(t), "Only array can be inverted.")

    local i, j = 1, #t
    while i < j do
        t[i], t[j] = t[j], t[i]
        i = i + 1
        j = j - 1
    end
    return t
end

-- 按指定的排序方式遍历：不修改表格
function table.walkSort(tb, sort_func, walk_func)
    local keys = table.keys(tb)
    table.sort(keys, function(lKey, rkey)
        return sort_func(lKey, rkey)
    end)
    for i = 1, table.length(keys) do
        walk_func(keys[i], tb[keys[i]])
    end
end

-- 获取哈希表所有键
function table.keys(t)
    local keys = {}
    for k, _ in pairs(t) do
        keys[#keys + 1] = k
    end
    return keys
end

-- 计算数据长度
function table.length(array)
    if array.n ~= nil then
        return array.n
    end

    local count = 0
    for i, _ in pairs(array) do
        if count < i then
            count = i
        end
    end
    return count
end

function table.clear(t)
    for k, _ in pairs(t) do
        t[k] = nil
        next(t, k) -- 将指针移到下一个键上
    end
end