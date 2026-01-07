local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.cam
---@field uid integer Unique engine-side identifier for the camera
---Represents a 2D camera that can be used to view a portion of the scene.
monoe.cam = {}
monoe.cam.__index = monoe.cam

---Creates a new `monoe.cam` instance.
---@return monoe.cam Newly created camera object
function monoe.cam.new()
  local uid = engine.import('monoe.exe.Core.Bridge.Types.QuickCamera2D')
  return setmetatable({ uid = uid }, monoe.cam)
end

---Removes the instance from the rendering side.
function monoe.cam:remove()
  engine.call(self.uid, 'Remove')
end

_G.monoe = monoe
_G.monoe.cam = monoe.cam

return monoe.cam