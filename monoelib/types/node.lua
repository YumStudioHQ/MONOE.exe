---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.node
---@field uid integer
---This is a simple node, that can allow you to structurate your scenes.
monoe.node = {}
monoe.node.__index = monoe.node

local base = 'monoe.exe.Core.Bridge.Types.RenderingDelegate'

---Creates a new node.
---@return monoe.node
function monoe.node.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.node object: invalid UID!')
  end

  return setmetatable({ uid = uid }, monoe.node)
end

---Attaches an object to the node state.
---@param state any
function monoe.node:attach(state)
  engine.call(self.uid, 'Attach', state.uid)
end

---@param x number Horizontal velocity
---@param y number Vertical velocity
---Moves the set of objects.
function monoe.node:move(x, y)
  engine.call(self.uid, 'Deplace', x, y)
end


---Gets or sets the position of the node in 2D space.
---If `x` and `y` are provided, the node is moved to that position.
---@param x number|nil X-coordinate to set (optional)
---@param y number|nil Y-coordinate to set (optional)
---@return number current_x The current or new X-coordinate
---@return number current_y The current or new Y-coordinate
function monoe.node:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---Scales the node in X and Y directions.
---@param x number Scale factor along the X-axis
---@param y number Scale factor along the Y-axis
---@return number new_x The resulting scale along X-axis
---@return number new_y The resulting scale along Y-axis
function monoe.node:scale(x, y)
  return engine.call(self.uid, "SetScale", x, y)
end

---Releases engine resources used by this node.
---After calling this, the node should no longer be used.
function monoe.node:free()
  engine.call(self.uid, 'Free')
end

---Removes the instance from the rendering side.
function monoe.node:remove()
  engine.call(self.uid, 'Remove')
end

_G.monoe = monoe
_G.monoe.node = monoe.node
return monoe.node