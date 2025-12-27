---@diagnostic disable: missing-return-value, return-type-mismatch
local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.entity
---@field uid integer
monoe.entity = {}
monoe.entity.__index = monoe.entity
monoe.entity.__midx = true

local base = "monoe.exe.Core.Bridge.Types.Entity2D"

---Crate a new entity. If a path is provided, it'll load it as an image
---@return monoe.entity
function monoe.entity.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Got bad UID when creating monoe.entity object!')
  end

  return setmetatable({ uid = uid }, monoe.entity)
end

---@param x number|nil
---@param y number|nil
---@return number, number
function monoe.entity:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---@param x number
---@param y number
---@return number, number
function monoe.entity:move(x, y)
  return engine.call(self.uid, 'Move', x, y)
end

---@param x number
---@param y number
---@return number, number
function monoe.entity:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

function monoe.entity:free()
  engine.call(self.uid, 'Free')
end

function monoe.entity:attach(obj)
  engine.call(self.uid, 'Attach', obj.uid)
end

_G.monoe = monoe
_G.monoe.entity = monoe.entity
return monoe.entity