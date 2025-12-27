---@diagnostic disable

local engine = require('libraries.engine')
local image = require('libraries.image')

monoe = monoe or {}

---@class monoe.animation
---@field uid integer
monoe.animation = {}
monoe.animation.__index = monoe.animation

local base = "monoe.exe.Core.Bridge.Types.Animation2D"

function monoe.animation.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Got bad UID when creating monoe.animation object!')
  end

  return setmetatable({ uid = uid }, monoe.animation)
end

---adds a new animation
---@param name string
function monoe.animation:add(name)
  engine.call(self.uid, 'NewAnimation', name)
end

---adds a frame at index
---@param name string
---@param frame monoe.image
---@param duration number|nil
---@param index integer|nil
function monoe.animation:addframe(name, frame, duration, index)
  engine.call(self.uid, 'AddFrame', name, frame.uid, duration or 1.0, index or -1)
end

---plays an animation
---@param name string
function monoe.animation:play(name)
  engine.call(self.uid, 'Play', name)
end

function monoe.animation:backwards(name)
  engine.call(self.uid, 'PlayBackwards', name)
end

function monoe.animation:pause()
  engine.call(self.uid, 'pause')
end

---returns all animations
---@return table<string>
function monoe.animation:animations()
  return { engine.call(self.uid, 'GetAnimations') }
end

function monoe.animation:position(x, y)
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
function monoe.animation:move(x, y)
  return engine.call(self.uid, 'Move', x, y)
end

---@param x number
---@param y number
---@return number
---@return number
function monoe.animation:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

---@param axe 'V'|'H'
---@param state boolean
function monoe.animation:flip(axe, state)
  engine.call(self.uid, 'Flip' .. axe, state)
end

---loads an animation from an image
---@param name string
---@param image monoe.image
---@param width integer
---@param height integer
---@param fromcolumn integer
---@param tocolumn integer
---@param fromrow integer
---@param torow integer
---@param fps number|nil
function monoe.animation:load(name, image, width, height, fromcolumn, tocolumn, fromrow, torow, fps)
  engine.call(self.uid, 'AnimationFromImage', name, image.uid, width, height, fromcolumn, tocolumn, fromrow, torow, fps or 7.0)
end

function monoe.animation:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.animation = monoe.animation
return monoe.animation