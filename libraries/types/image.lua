local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.image
---@field uid integer Unique ID for the engine-side image object
---Represents an image that can be loaded, cleared, and managed in the engine.
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
---@return string Current file path of the image
function monoe.image:path()
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

---Frees the engine-side resources associated with this image.
function monoe.image:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.image = monoe.image

return monoe.image
