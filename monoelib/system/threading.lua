local engine = require('monoelib.engine')
local event = require('monoelib.event')

monoe = monoe or {}
monoe.system = monoe.system or {}

---@class monoe.system.threading
---@field uid integer
---@field endsig string
---Allows multi-threading.
monoe.system.threading = {}
monoe.system.threading.__index = monoe.system.threading

local base = 'monoe.exe.Core.Bridge.Types.LibSys.ManagedThread'

---Creates a new managed thread
---@param source string File path or lua code
---@param entry string Main function's name
---@param finished string Event name, the one that'll be called once the thread finished its task.
---@param libs boolean True if the thread should open Lua's standard libraries.
---@return monoe.system.threading
function monoe.system.threading.new(source, entry, finished, libs)
  local uid, err = engine.import(base, source, entry, finished, libs)

  if uid == -1 then
    error('got invalid uid when creating an instance of monoe.system.thread, ' .. err)
  end

  return setmetatable({ uid = uid, endsig = finished }, monoe.system.threading)
end

---Starts the thread with the given set of arguments.
---@param ... any Arguments.
function monoe.system.threading:start(...)
  engine.call(self.uid, 'Start', ...)
end

---Returns true if the thread is joined after this call.
---@return boolean True if it is joined after this call.
function monoe.system.threading:terminate()
  ---@diagnostic disable-next-line: return-type-mismatch
  return engine.call(self.uid, 'Terminate')
end

function monoe.system.threading:finished(func)
  event.subscribe(self.endsig, func)
end

function monoe.system.threading:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.system = monoe.system
_G.monoe.system.threading = monoe.system.threading

return monoe.system.threading