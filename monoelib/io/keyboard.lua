local engine = require("monoelib.engine")

monoe = monoe or {}
monoe.io = monoe.io or {}
---@class monoe.io.keyboard
---Provides some helpful functions that allows you to use keyboard in the game.
monoe.io.keyboard = {}

local KEYBOARD = "monoe.exe.Core.Bridge.io.Keyboard"

---Returns true if a physical key is currently pressed.
---@param key string Key name (e.g. "a", "space", "enter", "esc")
---@return boolean
function monoe.io.keyboard.key_down(key)
  return engine.staticcall(KEYBOARD, "KeyPressed", key)
end

---Returns true if Shift is pressed.
---@return boolean
function monoe.io.keyboard.shift()
  return engine.staticcall(KEYBOARD, "Shift")
end

---Returns true if Ctrl is pressed.
---@return boolean
function monoe.io.keyboard.ctrl()
  return engine.staticcall(KEYBOARD, "Ctrl")
end

---Returns true if Alt is pressed.
---@return boolean
function monoe.io.keyboard.alt()
  return engine.staticcall(KEYBOARD, "Alt")
end

---Returns true if an action is currently pressed.
---@param action string
---@return boolean
function monoe.io.keyboard.action_down(action)
  return engine.staticcall(KEYBOARD, "ActionPressed", action)
end

---Returns true if an action was just pressed this frame.
---@param action string
---@return boolean
function monoe.io.keyboard.action_just_down(action)
  return engine.staticcall(KEYBOARD, "ActionJustPressed", action)
end

---Returns true if an action was just released this frame.
---@param action string
---@return boolean
function monoe.io.keyboard.action_just_up(action)
  return engine.staticcall(KEYBOARD, "ActionJustReleased", action)
end

---Returns the strength of an action (0.0 → 1.0).
---@param action string
---@return number
function monoe.io.keyboard.action_strength(action)
  return engine.staticcall(KEYBOARD, "ActionStrength", action)
end

_G.monoe = monoe

return monoe.io.keyboard