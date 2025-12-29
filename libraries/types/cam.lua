local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.cam
---@field uid integer
monoe.cam = {}
monoe.cam.__index = monoe.cam

function monoe.cam.new()
  local uid = engine.import('monoe.exe.Core.Bridge.Types.QuickCamera2D')
  return setmetatable({ uid = uid }, monoe.cam)
end

_G.monoe = monoe
_G.monoe.cam = monoe.cam

return monoe.cam