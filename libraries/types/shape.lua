local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.shape
---@field uid integer
monoe.shape = {}
monoe.shape.__index = monoe.shape

---creates a new shape
---@param shape string|'rectangle:WIDTHxHEIGHT'|'circle:RADIUS'|'capsule:RADIUSxHEIGHT'
---@return monoe.shape
function monoe.shape.new(shape)
  local uid = engine.import('monoe.exe.Core.Bridge.Types.MCollisionShape2D')

  if uid == -1 then
    error('got invalid UID when creating an instance of base monoe.exe.Core.Bridge.Types.MCollisionShape2D')
  end

  engine.call(uid, 'Shape', shape)

  if engine.debug and engine.debug == true then
    engine.call(uid, 'Debug', 0xFF0000FF)
  end
  
  return setmetatable({ uid = uid }, monoe.shape)
end

---adds outline
---@param hex integer
function monoe.shape:debug(hex)
  engine.call(self.uid, 'Debug', hex)
end

_G.monoe = monoe
_G.monoe.shape = monoe.shape
return monoe.shape