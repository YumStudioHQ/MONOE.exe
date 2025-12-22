local monolib = require('libraries.monolib')
local list = require('libraries.list')

monoe = monoe or {}
monoe.io = monoe.io or {}

monoe.io.directory = {}

local base = "monoe.exe.Core.Export.io.Filesystem"

---returns all files in given directories
---@param ... unknown
---@return monoe.list
function monoe.io.directory.files(...)
  return list.new({monolib.staticcall(base, "GetFilesFrom", ...)})
end

---returns all folders in given directories
---@param ... unknown
---@return monoe.list
function monoe.io.directory.folders(...)
  return list.new({monolib.staticcall(base, "GetFoldersFrom", ...)})
end

---returns all files of given directories recursively
---@param ... unknown
---@return monoe.list
---@deprecated This function generally makes the engine crash.
function monoe.io.directory.allfiles(...)
  return list.new({monolib.staticcall(base, "GetFilesRecursive", ...)})
end

---returns full paths
---@param ... unknown
---@return monoe.list
function monoe.io.directory.fullpaths(...)
  return list.new({monolib.staticcall(base, "Absolute", ...)})
end

---returns files' names
---@param ... unknown
---@return monoe.list
function monoe.io.directory.filenames(...)
  return list.new({monolib.staticcall(base, "FileName", ...)})
end

---return the full path of the file
---@param path string
---@return string
function monoe.io.directory.fullpath(path)
  return monolib.staticcall(base, "Absolute", path)
end

---return the file name of the file
---@param path string
---@return string
function monoe.io.directory.filename(path)
  return monolib.staticcall(base, "FileName", path)
end

_G.monoe = monoe
_G.monoe.io = monoe.io
_G.monoe.io.directory = monoe.io.directory
return monoe.io.directory