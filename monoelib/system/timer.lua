local engine = require('monoelib.engine')

monoe = monoe or {}
monoe.system = monoe.system or {}

---@class monoe.system.timer
monoe.system.timer = {}

local TIMER = 'monoe.exe.Core.Bridge.Types.LibSys.ManagedTimer'

---Spawns a timer that'll trigger an event once the given duration is reached
---@param finished string The event's name
---@param duration number Time (in seconds)
---@param oneshot boolean Should this run once, or forever ?
function monoe.system.timer.spawn(finished, duration, oneshot)
  engine.staticcall(TIMER, 'SetTimer', finished, (duration * 1.0), oneshot)
end

_G.monoe = monoe

return monoe.system.timer