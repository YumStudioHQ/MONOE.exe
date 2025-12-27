local event = require('libraries.event')

---@class monoe
monoe = monoe or {}

---imports a class
---@param class string
---@return integer
function monoe.import(class) return -1 end

---calls a method
---@param uid integer
---@param method string
---@param ... any
---@return unknown
function monoe.call(uid, method, ...) return {} end

---calls a static method on a static base
---@param base string
---@param method string
---@param ... any
---@return any
function monoe.staticcall(base, method, ...)
  print('default function called')
  print(debug.traceback())
end

monoe.env = monoe.env or {
  debug = false,
  path = ''
}

local base = 'monoe.exe.Core.Bridge.io.PathLib'

---returns the full path of a path
---@param path string
---@return string
local function fullpath(path)
  return monoe.staticcall(base, 'FullPath', path)
end

local function subscribe_all(name)
  if type(_G[name].process) == "function" then
    event.subscribe('process', function (delta) _G[name].process(delta) end)
  end

  if type(_G[name].physics) == "function" then
    event.subscribe('physics', function (delta) _G[name].physics(delta) end)
  end

  if type(_G[name].ready) == "function" then
    _G[name].ready()
  end

  if type(_G[name].exit) == "function" then
    event.subscribe('onexit', function () _G[name].exit() end)
  end
end

local function deep_update(old, new)
  for k, v in pairs(new) do
    if type(v) == "table" and type(old[k]) == "table" then
      deep_update(old[k], v)
    else
      old[k] = v
    end
  end
end

---loads a script
---@param name string
---@param path? string @module
function monoe.load(name, path)
  path = path or name

  event.once('@load', function ()
    package.loaded[path] = nil
    _G[name] = require(path)
    subscribe_all(name)
  end)

  local mpath = path
  if mpath:sub(-4) ~= '.lua' then
    mpath = mpath .. '.lua'
  end

  event.subscribe('@hot', function (module)
    if fullpath(module) == fullpath(mpath) then
      package.loaded[path] = nil
      local new = require(path)
      deep_update(_G[name], new)
    end
  end)

  return _G[name]
end

_G.monoe = monoe
return monoe