local monolib = require('libraries.engine')
monoe = monoe or {}

---@class monoe.object
---@field uid integer
monoe.object = {}
monoe.object.__index = monoe.object

local _base = "monoe.exe.Core.Export.Types.Object"

function monoe.object.new(base)
  local mbase = (base or _base)
  local uid = monolib.import(mbase)

  if uid == -1 then
    error('Got bad UID when creating monoe.object! On base ' .. mbase)
  end

  return setmetatable({ uid = uid }, monoe.object)
end

function monoe.object:ref()
  return monolib.call(self.uid, "Ref")
end

function monoe.object:free()
  monolib.call(self.uid, "Free")
end

_G.monoe = monoe
_G.monoe.object = monoe.object

return monoe.object