local engine = require('monoelib.engine')
local event = require('monoelib.event')

monoe = monoe or {}
monoe.system = monoe.system or {}

---@class monoe.system.fswatcher
---@field uid integer Internal unique indentifier.
---@field event string Event's base name. This string is random, and unpredictible. You can add '_changed', '_created', ... at the end of this in order to get the event's name. But prefer using the given method, as it will be more stable through versions.
---Observes changes of files in a directory, and fires events when changes happen, built on top of System.IO.FileSystemWatcher in C#, it runs on another thread, but fires events on the main thread, meaning that the code is totaly safe.
monoe.system.fswatcher = {}
monoe.system.fswatcher.__index = monoe.system.fswatcher

---Creates a new file system watcher.
---@param path string The path of the directory that should be watched.
---@param filter string File filters. Use '*.*' to watch all files, and '*.txt' to watch all files that ends with '.txt' extension.
---@return monoe.system.fswatcher
function monoe.system.fswatcher.new(path, filter)
  local uid, err = engine.import('monoe.exe.Core.Bridge.Types.LibSys.FSWatcher', path, filter)

  if uid == -1 then
    error('got invalid UID when creating instance of monoe.system.fswatcher', err)
  end

  local event = engine.call(uid, 'GetEventBaseName')

  return setmetatable({ uid = uid, event = event }, monoe.system.fswatcher)
end

---Calls the callback when the event is fired.
---@param func function<string> The callback.
---@param once boolean If the callback should be called once.
---@param kind 'changed'|'created'|'deleted'|'renamed' Event type
function monoe.system.fswatcher:set(func, kind, once)
  if once then
    event.once(self.event .. kind, func)
  else
    event.subscribe(self.event .. kind, func)
  end
end

---Frees the resource.
function monoe.system.fswatcher:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.system = monoe.system
_G.monoe.system.fswatcher = monoe.system.fswatcher

return monoe.system.fswatcher