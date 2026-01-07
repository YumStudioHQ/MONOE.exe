local engine = require('monoelib.engine')

monoe = monoe or {}

---Represents an image that can be loaded, cleared, and managed in the engine.
---@class monoe.image
---@field uid integer Unique ID for the engine-side image object
monoe.image = {}
monoe.image.__index = monoe.image

local base = 'monoe.exe.Core.Bridge.Types.Image'

---Creates a new `monoe.image` object.
---If a `path` is provided, the image will be loaded from that file.
---@param path string|integer|nil File path, existing UID, or nil for a new empty image
---@return monoe.image Newly created image object
function monoe.image.new(path)
  local uid = -1

  if type(path) == "number" then
    uid = path
  else
    uid = engine.import(base)
  end

  if uid == -1 then
    error('Failed to create monoe.image object: invalid UID!')
  end

  if type(path) == "string" then
    engine.call(uid, 'LoadImage', path)
  end

  return setmetatable({ uid = uid }, monoe.image)
end

---Returns the file path of the image.
---@return string current file path of the image
function monoe.image:path()
---@diagnostic disable-next-line: return-type-mismatch
  return engine.call(self.uid, 'GetPath')
end

---Loads an image from a file path.
---@param path string File path to load
function monoe.image:load(path)
  engine.call(self.uid, 'LoadImage', path)
end

---Clears the image content, resetting it to empty.
function monoe.image:clear()
  engine.call(self.uid, 'Clear')
end

---replaces the color a by the color b.
---@param a string|monoe.color
---@param b string|monoe.color
function monoe.image:replace(a, b)
  local _a = a or monoe.color.white
  local _b = b or monoe.color.white

  if type(a) == "table" then
    ---@cast a monoe.color
    _a = a:string()
  end

  if type(b) == "table" then
    ---@cast b monoe.color
    _a = b:string()
  end

  engine.call(self.uid, 'ReplaceColor', _a, _b)
end

---Frees the engine-side resources associated with this image.
function monoe.image:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.image = monoe.image

return monoe.image
