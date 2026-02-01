local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.csbuff
---@field uid integer
---This is an interface that helps using C# specific monoe.exe.Core.Bridge.Types.Internals.LazyReadonlyBuffer type.
monoe.csbuff = {}
monoe.csbuff.__index = monoe.csbuff

---Creates a new interface in order to use a C# LazyBuffer in Lua. You generally may not use this class by yourself, unless building a specific C# API, as the monoe C# API tries to avoid using this type in end users code.
---@param uid integer
---@return monoe.csbuff
function monoe.csbuff.new(uid)
  return setmetatable({ uid = uid }, monoe.csbuff)
end

---Returns the element at the given index (1-based indexes)
---@param index integer
---@return any
function monoe.csbuff:get(index)
  return engine.call(self.uid, 'AtIndex', index)
end

---Returns the size of the C# buffer
---@return integer
function monoe.csbuff:size()
  ---@diagnostic disable-next-line: return-type-mismatch
  return engine.call(self.uid, 'Size')
end

---Returns an array, now editable
---@return any[]
function monoe.csbuff:unpack()
  local arr = {}

  for i = 1, self:size(), 1 do
    table.insert(arr, self:get(i))
  end

  return arr
end

---Frees the resource (you may free it after using it)
function monoe.csbuff:free()
  engine.call(self.uid, 'Free')
end

monoe.csbuff = monoe.csbuff
return monoe.csbuff