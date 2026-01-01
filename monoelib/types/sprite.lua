---@diagnostic disable: return-type-mismatch, missing-return-value

local engine = require('monoelib.engine')
local image = require('monoelib.types.image')

monoe = monoe or {}

---@class monoe.sprite
---@field uid integer Unique ID for the engine-side sprite object
monoe.sprite = {}
monoe.sprite.__index = monoe.sprite

local base = 'monoe.exe.Core.Bridge.Types.Sprite'

---Creates a new `monoe.sprite` object.  
---If a path is provided, it will load it as an image.
---@param path string|nil File path to load as the sprite image
---@return monoe.sprite
function monoe.sprite.new(path)
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.sprite object: invalid UID!')
  end

  if path then
    engine.call(uid, 'LoadImage', path)
  end

  return setmetatable({ uid = uid }, monoe.sprite)
end

---Clears the sprite image.
function monoe.sprite:clear()
  engine.call(self.uid, 'Clear')
end

---Loads an image into the sprite.
---@param path string File path of the image
function monoe.sprite:load(path)
  engine.call(self.uid, 'LoadImage', path)
end

---Sets or gets the position of the sprite.
---@param x number|nil
---@param y number|nil
---@return number x
---@return number y
function monoe.sprite:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---Moves the sprite by the specified offset.
---@param x number
---@param y number
---@return number new_x
---@return number new_y
function monoe.sprite:move(x, y)
  return engine.call(self.uid, 'Move', x, y)
end

---Scales the sprite.
---@param x number
---@param y number
---@return number new_x
---@return number new_y
function monoe.sprite:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

---Returns the sprite's image object.
---@return monoe.image
function monoe.sprite:image()
  return image.new(engine.call(self.uid, 'GetImageUID'))
end

---Frees engine resources associated with this sprite.
function monoe.sprite:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.sprite = monoe.sprite

return monoe.sprite
