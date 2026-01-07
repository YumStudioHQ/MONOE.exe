local monolib = require('monoelib.engine')

monoe = monoe or {}
monoe.io = monoe.io or {}

---@class monoe.io
---Provides functions to query keyboard input and actions.
monoe.io = {}

local kybase = "monoe.exe.Core.Bridge.io.Keyboard"
local msbase = "monoe.exe.Core.Bridge.io.Mouse"

---Checks if a physical key is currently pressed.
---@param key string Key name (e.g., "space", "enter", "a")
---@return boolean True if the key is pressed, false otherwise
function monoe.io.key_down(key)
  return monoe.staticcall(kybase, "KeyPressed", key)
end

---Checks if an action is currently pressed.
---@param action string Action name (as defined in the input map of Godot)
---@note ui_down, ui_up, ui_right and ui_left are true when using ZSQD, WSDA, ...
---@return boolean True if the action is pressed, false otherwise
function monoe.io.down(action)
  return monoe.staticcall(kybase, "ActionPressed", action)
end

---Checks if an action was just pressed this frame.
---@param action string Action name
---@return boolean True if the action was just pressed, false otherwise
function monoe.io.just_down(action)
  return monoe.staticcall(kybase, "ActionJustPressed", action)
end

---Checks if an action was released this frame.
---@param action string Action name
---@return boolean True if the action was just released, false otherwise
function monoe.io.released(action)
  return monoe.staticcall(kybase, "ActionReleased", action)
end

---Returns the position of the mouse.
---@return integer, integer
function monoe.io.mouse()
  ---@diagnostic disable-next-line: return-type-mismatch, missing-return-value
  return monoe.staticcall(msbase, 'Position')
end

_G.monoe = monoe
_G.monoe.io = monoe.io
_G.monoe.io = monoe.io

return monoe.io
