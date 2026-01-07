local engine = require('monoelib.engine')
local rendering = require('monoelib.rendering')

monoe = monoe or {}

---@class monoe.renderer
---@field uid integer
---@field objects table
---This class allows you to set a rendering order for your games, sorting with a zindex, if the field is present.
monoe.renderer = {}
monoe.renderer.__index = monoe.renderer

local base = 'monoe.exe.Core.Bridge.Types.RenderingDelegate'

---Creates a new renderer.
---@return monoe.renderer
function monoe.renderer.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.renderer object: invalid UID!')
  end

  return setmetatable({ uid = uid, objects = {} }, monoe.renderer)
end

---Attaches an object to the renderer state.
---@param state any
function monoe.renderer:attach(state)
  self.objects[#self.objects+1] = state
end

---Sorts the internal table, and returns a renderable table (e.g., monoe.io.window)
---@warning You MAY NOT call this function each frame, as it is heavy. Prefer exposing once, or, each new scenes.
---@return table
function monoe.renderer:expose()
  table.sort(self.objects, function(a, b)
    return (a.zindex or 0) < (b.zindex or 0)
  end)

  return {
    root = self.uid,
    objects = self.objects,
  }
end

_G.monoe = monoe
_G.monoe.renderer = monoe.renderer
return monoe.renderer