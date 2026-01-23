---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require("monoelib.engine")

monoe = monoe or {}
monoe.io = monoe.io or {}

---@class monoe.io.mouse
---Provides helpful functions that'll allow you to use the mouse during your game.
monoe.io.mouse = {}

local MOUSE = "monoe.exe.Core.Bridge.io.Mouse"

---Returns the mouse position in viewport space.
---@return number x, number y
function monoe.io.mouse.position()
  return engine.staticcall(MOUSE, "Position")
end

---Returns the mouse movement delta since last frame.
---@return number dx, number dy
function monoe.io.mouse.delta()
  return engine.staticcall(MOUSE, "Delta")
end

---Returns true if a mouse button is pressed.
---@param button "left"|"right"|"middle"|"x1"|"x2"
---@return boolean
function monoe.io.mouse.down(button)
  return engine.staticcall(MOUSE, "ButtonPressed", button)
end

_G.monoe = monoe

return monoe.io.mouse