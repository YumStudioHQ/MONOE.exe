local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.shape
---@field uid integer Unique ID for the engine-side shape object
monoe.shape = {}
monoe.shape.__index = monoe.shape

---Creates a new shape object for collisions.  
---Supports predefined types: `"rectangle:WIDTHxHEIGHT"`, `"circle:RADIUS"`, `"capsule:RADIUSxHEIGHT"`.
---@param shape string Shape description
---@return monoe.shape
function monoe.shape.new(shape)
  local uid = engine.import('monoe.exe.Core.Bridge.Types.MCollisionShape2D')

  if uid == -1 then
    error('Failed to create monoe.shape object: invalid UID!')
  end

  engine.call(uid, 'Shape', shape)

  -- Automatically debug draw in engine debug mode
  if engine.debug == true then
    engine.call(uid, 'Debug', 0xFF0000FF)
  end

  return setmetatable({ uid = uid }, monoe.shape)
end

---Sets a debug outline color for the shape.
---@param hex integer Color in 0xRRGGBBAA format
function monoe.shape:debug(hex)
  engine.call(self.uid, 'Debug', hex)
end

---Removes the instance from the rendering side.
function monoe.shape:remove()
  engine.call(self.uid, 'Remove')
end

---Reshapes the actual shape.
---@param shape string
function monoe.shape:shape(shape)
  engine.call(self.uid, 'ReShape', shape)
end

---Frees the shape. (won't be usable after that, unless you create a new one)
function monoe.shape:free()
  engine.call(self.uid, 'Free')
end

-- Expose globally
_G.monoe = monoe
_G.monoe.shape = monoe.shape

return monoe.shape
