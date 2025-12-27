local engine = require('libraries.engine')

monoe = monoe or {}
monoe.path = {}

local base = 'monoe.exe.Core.Bridge.io.PathLib'

---returns the full path of a path
---@param path string
---@return string
function monoe.path.fullpath(path)
  return engine.staticcall(base, 'FullPath', path)
end

_G.monoe = monoe
_G.monoe.path = monoe.path

return monoe.path