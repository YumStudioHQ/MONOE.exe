---@diagnostic disable: missing-return-value, return-type-mismatch
local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.stobj
---@field uid integer
---Represents a static object that can move, scale, and be freed / removed / attached. Similar to monoe.entitys, this one if designed for objects.
monoe.stobj = {}
monoe.stobj.__inde = monoe.stobj

function monoe.stobj.new()
  local uid = engine.import('monoe.exe.Core.Bridge.Types.StaticObject')

  if uid ~= -1 then
    error('got invalid UID when creating monoe.stobj !')
  end

  return setmetatable({ uid = uid }, monoe.stobj)
end

---Gets or sets the position of the stobj in 2D space.
---If `x` and `y` are provided, the stobj is moved to that position.
---@param x number|nil X-coordinate to set (optional)
---@param y number|nil Y-coordinate to set (optional)
---@return number current_x The current or new X-coordinate
---@return number current_y The current or new Y-coordinate
function monoe.stobj:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---Moves the stobj by applying a velocity.
---Automatically calls `MoveAndSlide` to update the stobj's position according to physics rules.
---@param x number Horizontal velocity
---@param y number Vertical velocity
---@warn Do not use this function outside the physics event.
function monoe.stobj:move(x, y)
  engine.call(self.uid, 'Deplace', x, y)
end

---Scales the stobj in X and Y directions.
---@param x number Scale factor along the X-axis
---@param y number Scale factor along the Y-axis
---@return number new_x The resulting scale along X-axis
---@return number new_y The resulting scale along Y-axis
function monoe.stobj:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

---Releases engine resources used by this stobj.
---After calling this, the stobj should no longer be used.
function monoe.stobj:free()
  engine.call(self.uid, 'Free')
end

---Removes the instance from the rendering side.
function monoe.stobj:remove()
  engine.call(self.uid, 'Remove')
end

---Attaches another object (sprite, animation, or another stobj) to this stobj.
---The attached object will move and scale with the stobj.
---@diagnostic disable-next-line: undefined-doc-name
---@param obj monoe.image|monoe.sprite|monoe.animation|monoe.stobj Object to attach
function monoe.stobj:attach(obj)
  engine.call(self.uid, 'Attach', obj.uid)
end

_G.monoe = monoe
_G.monoe.stobj = monoe.stobj

return monoe.stobj