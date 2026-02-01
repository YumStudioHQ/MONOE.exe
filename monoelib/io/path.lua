local engine = require('monoelib.engine')
local csbuff = require('monoelib.types.csbuff')

monoe = monoe or {}

---@class monoe.path
---Provides utilty functions for basic io operations.
monoe.path = {}
monoe.path.__index = monoe.path

local base = 'monoe.exe.Core.Bridge.io.PathLib'

---Returns the full path of a file or directory
---@param path string
---@return string
function monoe.path.fullpath(path)
  return engine.staticcall(base, 'FullPath', path)
end

---Copies a file or directory
---@param isdir boolean True if copying a directory
---@param src string Source path
---@param dst string Destination path
function monoe.path.copy(isdir, src, dst)
  if not isdir then
    engine.staticcall(base, 'CopyFile', src, dst)
  else
    engine.staticcall(base, 'CopyDirectory', src, dst)
  end
end

---Returns all files and directories in a path
---@param path string
---@return string[]
function monoe.path.content(path)
  local content = csbuff.new(engine.staticcall(base, 'GetContent', path))
  return content:unpack()
end

---Returns the parent directory of a path
---@param path string
---@return string
function monoe.path.parent(path)
  return engine.staticcall(base, 'GetParent', path)
end

---Creates a directory
---@param path string
function monoe.path.create(path)
  engine.staticcall(base, 'CreateDirectory', path)
end

---Checks if a path is a file
---@param path string
---@return boolean
function monoe.path.isfile(path)
  return engine.staticcall(base, 'IsFile', path)
end

---Checks if a path exists
---@param path string
---@return boolean
function monoe.path.exist(path)
  return engine.staticcall(base, 'Exist', path)
end

---Returns a random file or folder
---@return string
function monoe.path.random()
  return engine.staticcall(base, 'Random')
end

---Returns the file's name
---@param path string
---@return string
function monoe.path.filename(path)
  return path:match("^.+[/\\](.+)$") or path
end

_G.monoe = _G.monoe or {}
_G.monoe.path = monoe.path

return monoe.path