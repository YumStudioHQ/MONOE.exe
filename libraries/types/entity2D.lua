
local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.entity2D
---@field uid integer
monoe.entity2D = {}
monoe.entity2D.__index = monoe.entity2D
monoe.entity2D.__midx = true

local base = "monoe.exe.Core.Export.Types.Entity2D"

---Crate a new entity2D. If a path is provided, it'll load it as an image
---@return monoe.entity2D
function monoe.entity2D.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Got bad UID when creating monoe.entity2D object!')
  end

  return setmetatable({ uid = uid }, monoe.entity2D)
end

function monoe.entity2D:position(x, y)
  if x and y then
    engine.call(self.uid, 'SetPosition', x, y)
    return x, y
  end

  return engine.call(self.uid, 'GetPosition')
end

function monoe.entity2D:move(x, y)
  return engine.call(self.uid, 'Move', x, y)
end

function monoe.entity2D:scale(x, y)
  return engine.call(self.uid, "Scale", x, y)
end

function monoe.entity2D:free()
  engine.call(self.uid, 'Free')
end

function monoe.entity2D:attach(obj)
  engine.call(self.uid, 'Attach', obj.uid)
end

_G.monoe = monoe
_G.monoe.entity2D = monoe.entity2D
return monoe.entity2D