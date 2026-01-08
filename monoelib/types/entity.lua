---@diagnostic disable: missing-return-value, return-type-mismatch

local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.entity
---@field uid integer Unique engine-side identifier for this entity
---@field zindex integer
---Represents a 2D game entity that can hold sprites, animations, or other attached objects.
monoe.entity = {}
monoe.entity.__index = monoe.entity

local base = "monoe.exe.Core.Bridge.Types.Entity2D"

---Creates a new `monoe.entity` instance.
---This entity can be positioned, scaled, moved, and have other objects attached.
---@return monoe.entity Newly created entity object
function monoe.entity.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.entity object: invalid UID!')
  end

  return setmetatable({ uid = uid, zindex = 0 }, monoe.entity)
end

---Gets or sets the position of the entity in 2D space.
---If `x` and `y` are provided, the entity is moved to that position.
---@param x number|nil X-coordinate to set (optional)
---@param y number|nil Y-coordinate to set (optional)
---@return number current_x The current or new X-coordinate
---@return number current_y The current or new Y-coordinate
function monoe.entity:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---Moves the entity by applying a velocity.
---Automatically calls `MoveAndSlide` to update the entity's position according to physics rules.
---@param x number Horizontal velocity
---@param y number Vertical velocity
---@warn Do not use this function outside the physics event.
function monoe.entity:move(x, y)
  engine.call(self.uid, 'Velocity', x, y)
  engine.call(self.uid, 'MoveAndSlide')
end

---Scales the entity in X and Y directions.
---@param x number Scale factor along the X-axis
---@param y number Scale factor along the Y-axis
---@return number new_x The resulting scale along X-axis
---@return number new_y The resulting scale along Y-axis
function monoe.entity:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

---Releases engine resources used by this entity.
---After calling this, the entity should no longer be used.
function monoe.entity:free()
  engine.call(self.uid, 'Free')
end

---Removes the instance from the rendering side.
function monoe.entity:remove()
  engine.call(self.uid, 'Remove')
end

---Attaches another object (sprite, animation, or another entity) to this entity.
---The attached object will move and scale with the entity.
---@diagnostic disable-next-line: undefined-doc-name
---@param obj monoe.image|monoe.sprite|monoe.animation|monoe.entity Object to attach
function monoe.entity:attach(obj)
  engine.call(self.uid, 'Attach', obj.uid)
end

_G.monoe = monoe
_G.monoe.entity = monoe.entity

return monoe.entity
