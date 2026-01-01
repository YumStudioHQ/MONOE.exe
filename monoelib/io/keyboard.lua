local monolib = require('monoelib.engine')

monoe = monoe or {}
monoe.io = monoe.io or {}

---@class monoe.io.keyboard
---Provides functions to query keyboard input and actions.
monoe.io.keyboard = {}

local base = "monoe.exe.Core.Bridge.io.Keyboard"

---Checks if a physical key is currently pressed.
---@param key string Key name (e.g., "space", "enter", "a")
---@return boolean True if the key is pressed, false otherwise
function monoe.io.keyboard.key_down(key)
  return monoe.staticcall(base, "KeyPressed", key)
end

---Checks if an action is currently pressed.
---@param action string Action name (as defined in the input map of Godot)
---@note ui_down, ui_up, ui_right and ui_left are true when using ZSQD, WSDA, ...
---@return boolean True if the action is pressed, false otherwise
function monoe.io.keyboard.down(action)
  return monoe.staticcall(base, "ActionPressed", action)
end

---Checks if an action was just pressed this frame.
---@param action string Action name
---@return boolean True if the action was just pressed, false otherwise
function monoe.io.keyboard.just_down(action)
  return monoe.staticcall(base, "ActionJustPressed", action)
end

---Checks if an action was released this frame.
---@param action string Action name
---@return boolean True if the action was just released, false otherwise
function monoe.io.keyboard.released(action)
  return monoe.staticcall(base, "ActionReleased", action)
end

_G.monoe = monoe
_G.monoe.io = monoe.io
_G.monoe.io.keyboard = monoe.io.keyboard

return monoe.io.keyboard
