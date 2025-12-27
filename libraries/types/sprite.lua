---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('libraries.engine')
local image = require('libraries.image')

monoe = monoe or {}

---@class monoe.sprite
---@field uid integer
monoe.sprite = {}
monoe.sprite.__index = monoe.sprite

local base = 'monoe.exe.Core.Bridge.Types.Sprite'

---Crate a new sprite. If a path is provided, it'll load it as an image
---@param path string|nil
---@return monoe.sprite
function monoe.sprite.new(path)
  local uid = engine.import(base)

  if uid == -1 then
    error('Got bad UID when creating monoe.sprite object!')
  end

  if path then
    engine.call(uid, 'LoadImage', path)
  end

  return setmetatable({ uid = uid }, monoe.sprite)
end

function monoe.sprite:clear()
  engine.call(self.uid, 'Clear')
end

function monoe.sprite:load(path)
  engine.call(self.uid, 'LoadImage', path)
end

---places the sprite at x;y
---@param x number
---@param y number
---@return number
---@return number
function monoe.sprite:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---@param x number
---@param y number
---@return number
---@return number
function monoe.sprite:move(x, y)
  return engine.call(self.uid, 'Move', x, y)
end

---@param x number
---@param y number
---@return number
---@return number
function monoe.sprite:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

---@return integer
function monoe.sprite:image()
  return image.new(engine.call(self.uid, 'GetImageUID'))
end

function monoe.sprite:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.sprite = monoe.sprite
return monoe.sprite