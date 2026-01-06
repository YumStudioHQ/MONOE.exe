---@diagnostic disable

local engine = require('monoelib.engine')
local image = require('monoelib.types.image')

monoe = monoe or {}

---@class monoe.animation
---@field uid integer Unique ID for the engine-side animation object
monoe.animation = {}
monoe.animation.__index = monoe.animation

local base = "monoe.exe.Core.Bridge.Types.Animation2D"

---Creates a new `monoe.animation` object.
---@return monoe.animation
function monoe.animation.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.animation object: invalid UID!')
  end

  return setmetatable({ uid = uid }, monoe.animation)
end

---Adds a new animation by name.
---@param name string Name of the animation
function monoe.animation:add(name)
  engine.call(self.uid, 'NewAnimation', name)
end

---Adds a frame to an animation.
---@param name string Name of the animation
---@param frame monoe.image The frame image to add
---@param duration number|nil Duration of the frame in seconds (default 1.0)
---@param index integer|nil Index at which to insert the frame (-1 = append)
function monoe.animation:addframe(name, frame, duration, index)
  engine.call(self.uid, 'AddFrame', name, frame.uid, duration or 1.0, index or -1)
end

---Plays the specified animation.
---@param name string Animation name
function monoe.animation:play(name)
  engine.call(self.uid, 'Play', name)
end

---Plays an animation backwards.
---@param name string Animation name
function monoe.animation:backwards(name)
  engine.call(self.uid, 'PlayBackwards', name)
end

---Pauses the current animation.
function monoe.animation:pause()
  engine.call(self.uid, 'Pause')
end

---Returns a list of all animation names.
---@return string[]
function monoe.animation:animations()
  return { engine.call(self.uid, 'GetAnimations') }
end

---Sets or gets the position of the animation.
---@param x number|nil
---@param y number|nil
---@return number x
---@return number y
function monoe.animation:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

---Moves the animation by a given offset.
---@param x number
---@param y number
---@return number newX
---@return number newY
function monoe.animation:move(x, y)
  return engine.call(self.uid, 'Move', x, y)
end

---Scales the animation.
---@param x number
---@param y number
---@return number newX
---@return number newY
function monoe.animation:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

---Flips the animation on a given axis.
---@param axe 'V'|'H' Vertical or Horizontal
---@param state boolean True to flip, false to reset
function monoe.animation:flip(axe, state)
  engine.call(self.uid, 'Flip' .. axe, state)
end

---Loads an animation from a sprite sheet image.
---@param name string Animation name
---@param image monoe.image Source image
---@param width integer Frame width
---@param height integer Frame height
---@param fromcolumn integer Starting column
---@param tocolumn integer Ending column
---@param fromrow integer Starting row
---@param torow integer Ending row
---@param fps number|nil Frames per second (default 7.0)
function monoe.animation:load(name, image, width, height, fromcolumn, tocolumn, fromrow, torow, fps)
  engine.call(self.uid, 'AnimationFromImage', name, image.uid, width, height, fromcolumn, tocolumn, fromrow, torow, fps or 7.0)
end

---Frees engine resources associated with this animation.
function monoe.animation:free()
  engine.call(self.uid, 'Free')
end

-- Expose globally
_G.monoe = monoe
_G.monoe.animation = monoe.animation

return monoe.animation
