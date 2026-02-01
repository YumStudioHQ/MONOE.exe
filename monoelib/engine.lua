local event = require('monoelib.event')
local rendering = require('monoelib.rendering')

---@class monoe
---monoe.exe's engine base class. This class provides you a quick yet deep access to the engine's internals -- Assemblies.
---You may check out monoe.system in order to get deeper or more low-level features.
monoe = monoe or {}

---@class monoe.info
monoe.info =  {
  os = {
    name = 'unknown',
    version = 'unknown',
    argv = {},
    processorcount = -1,
    isos64 = false,
    isproc64 = false,
    ispriviliged = false,
    machinename = 'unknown',
    procid = -1,
    exit = function(code) end
  },
  runtime = {
    version = 'unknown',
    isdev = false,
    iseditor = false,
  },
}

---Imports a class by name.
---@param class string Name of the class to import.
---@param ... any Optional arguments that you can pass to the constructor.
---@return integer uid Returns the unique ID of the created instance or -1 on failure.
function monoe.import(class, ...)
  print('warn: default function called')
  return -1
end

---Calls a method on an instance.
---@param uid integer The unique ID of the object.
---@param method string The method name to call.
---@param ... any Additional arguments to pass to the method.
---@return any[] The return values from the method call.
function monoe.call(uid, method, ...) return {} end

---Calls a static method on a static class.
---@param base string Full name of the static base class.
---@param method string Method name to call.
---@param ... any Arguments to pass to the method.
---@return any The return value of the static method.
function monoe.staticcall(base, method, ...)
  print('default function called')
  print(debug.traceback())
end

---Opens a shell or executes commands.
---@param ... any Arguments passed to the shell.
function monoe.shell(...) end

---Environment settings for monoe.
monoe.env = monoe.env or {
  debug = false,  --- Enable debug mode
  path = ''       --- Base path for scripts
}

local base = 'monoe.exe.Core.Bridge.io.PathLib'

---Returns the full path of a file.
---@param path string Relative path.
---@return string Absolute path.
local function fullpath(path)
  return monoe.staticcall(base, 'FullPath', path)
end

-- Subscribe table methods to events if they exist
local function subscribe_table(table)
  if type(table) ~= "table" then return end

  if type(table.process) == "function" then
    event.subscribe('@process', table.process)
  end

  if type(table.physics) == "function" then
    event.subscribe('@physics', table.physics)
  end

  if type(table.ready) == "function" then
    table.ready()
  end

  if type(table.input) == "function" then
    event.subscribe('@input', table.input)
  end

  if type(table.exit) == "function" then
    event.subscribe('@onexit', table.exit)
  end
end

-- Subscribe table methods to events if they exist
local function subscribe_object(table)
  if type(table) ~= "table" then return end

  if type(table.process) == "function" then
    event.subscribe('@process', function (delta) table:process(delta) end)
  end

  if type(table.physics) == "function" then
    event.subscribe('@physics', function (delta) table:physics(delta) end)
  end

  if type(table.ready) == "function" then
    table:ready()
  end

  if type(table.input) == "function" then
    event.subscribe('@input', function () table:input() end)
  end

  if type(table.exit) == "function" then
    event.subscribe('@onexit', function () table:exit() end)
  end
end

-- Deeply merge new table into old table
local function deep_update(old, new)
  for k, v in pairs(new) do
    if type(v) == "table" and type(old[k]) == "table" then
      deep_update(old[k], v)
    else
      old[k] = v
    end
  end
end

---Loads a Lua module and optionally enables hot reloading.
---@param name string Name to assign in _G.
---@param path? string Module path. Defaults to `name`.
---@return table The loaded module.
function monoe.load(name, path)
  path = path or name

  event.once('@load', function ()
    package.loaded[path] = nil
    _G[name] = require(path)
    subscribe_table(_G[name])
  end)

  local mpath = path:gsub('%.', '/')
  if mpath:sub(-4) ~= '.lua' then
    mpath = mpath .. '.lua'
  end

  event.subscribe('@hot', function(module)
    if fullpath(module) == fullpath(mpath) then
      package.loaded[path] = nil
      local new = require(path)
      deep_update(_G[name], new)
    end
  end)

  return _G[name]
end

monoe.debug = monoe.debug

---Triggers a breakpoint in Lua.
---@param ... unknown Some debut infos you can add.
function monoe.breakpoint(...)
  error('breakpoint' .. debug.traceback(), ...)
end

---Pauses execution for a specified number of milliseconds.
---@param milliseconds integer
function monoe.wait(milliseconds) end

---Attaches an object or its children to the window for rendering.
---@param obj table Object with `.uid` or `.root` property
local function _attach(obj)
  if type(obj) == "table" and obj.root then
    rendering.attach_tree(obj.root, obj)
    _attach(obj.root)
  else
    monoe.staticcall("monoe.exe.Core.Bridge.io.EngineMainWindow", 'Attach', obj.uid)
  end
end

---Subscribes all known methods of a table to events.
---@param self table
---@param static boolean|nil
function monoe.qualify(self, static)
  if not static then
    subscribe_object(self)
  else
    subscribe_table(self)
  end

  if type(self.root) == "table" then
    _attach(self)
  elseif type(self.uid) == 'number' then
    _attach(self)
  end
end

---Called when the user requests an exit. You can override this function in order to handle exit requests.
---@param code number Exit code
function monoe.exit_requested(code)end

_G.monoe = monoe
return monoe
