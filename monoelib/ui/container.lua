---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.container
---@field uid integer
monoe.container = {}
monoe.container.__index = monoe.container

---Creates a new container
---@param kind 'vbox'|'hbox'
---@return monoe.container
function monoe.container.new(kind)
  local uid, err = engine.import('monoe.exe.Core.Bridge.Types.UI.Container', kind)

  if uid == -1 then
    error('got invalid UID on base monoe.container' .. tostring(err))
  end

  return setmetatable({ uid = uid }, monoe.container)
end

---Moves the container
---@param x number
---@param y number
---@return number
---@return number
function monoe.container:move(x, y)
  return engine.call(self.uid, 'Deplace', x, y)
end

---Returns the position of the container, and places at its new position if x or y are given.
---@param x number|nil
---@param y number|nil
---@return number
---@return number
function monoe.container:position(x, y)
  if x or y then
    engine.call(self.uid, 'SetPosition', x or 0.0, y or 0.0)
  end

  return engine.call(self.uid, 'GetPosition')
end

---Returns the size of the container, and places at its new size if x or y are given.
---@param x number|nil
---@param y number|nil
---@return number
---@return number
function monoe.container:size(x, y)
  if x or y then
    engine.call(self.uid, 'SetSize', x or 0.0, y or 0.0)
  end

  return engine.call(self.uid, 'GetSize')
end

---Returns the scale of the container, and places at its new scale if x or y are given.
---@param x number|nil
---@param y number|nil
---@return number
---@return number
function monoe.container:scale(x, y)
  if x or y then
    engine.call(self.uid, 'SetScale', x or 0.0, y or 0.0)
  end

  return engine.call(self.uid, 'GetScale')
end

---Removes the container from its rendering server
function monoe.container:remove()
  engine.call(self.uid, 'Remove')
end

---Frees the container (no more usable)
function monoe.container:free()
  engine.call(self.uid, 'Free')
end

---Attaches given object to the container
---@param ... unknown
function monoe.container:pack(...)
  engine.call(self.uid, 'Attach', ...)
end

---Attaches given object to the container
---@param ... unknown
function monoe.container:attach(...)
  engine.call(self.uid, 'Attach', ...)
end

_G.monoe = monoe
return monoe.container