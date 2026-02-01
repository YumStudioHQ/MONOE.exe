local event = require('monoelib.event')
local console = require('monoelib.system.console')

monoe = monoe or {}

---@class monoe.resources
---Provides useful functions that makes loading easier.
monoe.resources = {}

---@class monoe.resources.entry
---@field res any            -- Actual resource instance
---@field T table            -- Resource representation type
---@field pinned boolean     -- Whether the resource is kept alive

---@type table<string, monoe.resources.entry>
monoe.resources._cache = {}

local cache = monoe.resources._cache

---Returns true if the resource can be loaded.
---@param T any
---@return boolean
local function is_loadable(T)
  if type(T) ~= "table" then
    console.warn('cannot load a resource as an integral type!')
    return false
  elseif type(T.new) ~= "function" then
    console.warn('cannot load a resource when its representation type does not have a new() method!')
    return false
  end

  return true
end

---Internal loader function.
---Creates a cache entry if none exists.
---@generic T : table The resource's representation type in Lua
---@param T T
---@param path string
---@return monoe.resources.entry?
local function _load_res(T, path)
  if not is_loadable(T) then return end

  local entry = cache[path]

  if entry then
    if entry.T.__index ~= T.__index then
      console.warn('cached element for ' .. path .. ' is reloaded with a different type (overwriting)')
      if type(entry.T.free) == "function" then
        entry.res:free()
      end
      ---@diagnostic disable-next-line: cast-local-type
      entry = nil
    end
  end

  if not entry then
    console.info('loading "' .. path .. '"')
    entry = {
      res = T.new(path),
      T = T,
      pinned = false
    }
    cache[path] = entry
  end

  return entry
end

---Preloads a resource and pins it in memory.
---Pinned resources are not freed during collection.
---@generic T : table The resource's representation type in Lua
---@param T T
---@param path string Resource's path
function monoe.resources.preload(T, path)
  local entry = _load_res(T, path)
  if not entry then return end

  entry.pinned = true
end

---Loads a resource for temporary usage.
---Unpinned resources are freed on the next collection.
---@generic T : table
---@param T T
---@param path string
---@return T
function monoe.resources.load(T, path)
  if not is_loadable(T) then
    error('cannot load with the given type!')
  end

  local entry = _load_res(T, path)
  if not entry then
    error('resource loading failed!')
  end

  if not entry._collect_hooked then
    ---@diagnostic disable-next-line: inject-field
    entry._collect_hooked = true

    event.once('@collect', function ()
      local e = cache[path]
      if not e or e.pinned then return end

      if type(e.T.free) == "function" then
        e.res:free()
      end
      cache[path] = nil
    end)
  end

  return entry.res
end

_G.monoe = monoe

return monoe.resources
