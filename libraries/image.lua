local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.image
---@field uid integer
monoe.image = {}
monoe.image.__index = monoe.image
monoe.image.__midx = true

local base = 'monoe.exe.Core.Bridge.Types.Image'

---Crate a new image. If a path is provided, it'll load it as an image
---@param path string|nil|integer
---@return monoe.image
function monoe.image.new(path)
  local uid = -1

  if type(path) == "number" then
    uid = path
  else uid = engine.import(base)
  end

  if uid == -1 then
    error('Got bad UID when creating monoe.image object!')
  end

  if type(path) == "string" then
    engine.call(uid, 'LoadImage', path)
  end

  return setmetatable({ uid = uid }, monoe.image)
end

---returns the path of the image.
---@return string
function monoe.image:path()
  return engine.call(self.uid, 'GetPath')
end

---Loads an image
---@param path string
function monoe.image:load(path)
  engine.call(self.uid, 'LoadImage', path)
end

function monoe.image:clear()
  engine.call(self.uid, 'Clear')
end

function monoe.image:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.image = monoe.image
return monoe.image