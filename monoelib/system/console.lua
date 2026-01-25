local engine = require('monoelib.engine')

monoe = monoe or {}
monoe.system = monoe.system or {}

---@class monoe.system.console
---Provides interactions with the engine's console.
monoe.system.console = {}

local base = 'monoe.exe.Core.Engine.EngineConsole'

local get_strings = function (...)
  local t = { ... }
  local s = {}
  for _, value in ipairs(t) do
    s[#s+1] = tostring(value)
  end
  return table.unpack(s)
end

---Writes in the console. Arguments are not formated.
---@param ... any
function monoe.system.console.write(...)
  engine.staticcall(base, 'LWrite', get_strings(...))
end

---Writes in the console, and adds a line at the end. Arguments are not formated.
---@param ... any
function monoe.system.console.writeline(...)
  engine.staticcall(base, 'LWriteLine', get_strings(...))
end

---Writes a warning in the console. The warning is printed in yellow, with a time stamp.
---@param ... any
function monoe.system.console.warn(...)
  engine.staticcall(base, 'WriteWarning', get_strings(...))
end

---Writes an error in the console. The error is printed in red, with a time stamp.
---@param ... any
function monoe.system.console.error(...)
  engine.staticcall(base, 'WriteError', get_strings(...))
end

---Writes an information in the console. The information is printed in grey, with a time stamp. If the engine does not run in verbose mode, the message won't be show.
---@param ... any
function monoe.system.console.info(...)
  engine.staticcall(base, 'Verbose', get_strings(...))
end

_G.monoe = monoe
_G.monoe.system = monoe.system
_G.monoe.system.console = monoe.system.console

return monoe.system.console