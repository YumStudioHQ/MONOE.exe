local monolib = require('libraries.engine')

monoe = monoe or {}
monoe.io = monoe.io or {}

monoe.io.keyboard = {}

local base =  "monoe.exe.Core.Bridge.io.Keyboard"

---@param key string
---@return boolean
function monoe.io.keyboard.key_down(key)
  return monoe.staticcall(base, "KeyPressed", key)
end

---@param action string
---@return boolean
function monoe.io.keyboard.down(action)
  return monoe.staticcall(base, "ActionPressed", action)
end

---@param action string
---@return boolean
function monoe.io.keyboard.just_down(action)
  return monoe.staticcall(base, "ActionJustPressed", action)
end

---@param action string
---@return boolean
function monoe.io.keyboard.released(action)
  return monoe.staticcall(base, "ActionReleased", action)
end

_G.monoe = monoe
_G.monoe.io = monoe.io
_G.monoe.io.keyboard = monoe.io.keyboard

return monoe.io.keyboard