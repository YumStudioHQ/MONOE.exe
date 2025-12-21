---@class monoe
---@field invalid_uid integer
monoe = {}
monoe.invalid_uid = -1

---imports a class
---@param class string
---@return integer
function monoe.import(class) return monoe.invalid_uid end

---calls a method
---@param uid integer
---@param method string
---@param ... any
---@return any
function monoe.call(uid, method, ...) end

_G.monoe = monoe
return monoe