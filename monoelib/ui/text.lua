---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('monoelib.engine')
monoe = monoe or {}

---@class monoe.text
---@field uid integer
monoe.text = {}
monoe.text.__index = monoe.text

monoe.text.default = {
  font = nil
}

---Creates a new text label
---@param ... unknown
---@return monoe.text
function monoe.text.new(...)
  uid = engine.import('monoe.exe.Core.Bridge.Types.UI.TextLabel')

  if uid == -1 then
    error('got invalid UID on base monoe.text.new!')
  end

  engine.call(uid, 'SetText', ...)

  local self = setmetatable({ uid = uid }, monoe.text)

  if monoe.text.default.font then
    self:font(monoe.text.default.font)
  end

  return self
end

---Moves the label
---@param x number
---@param y number
---@return number
---@return number
function monoe.text:move(x, y)
  return engine.call(self.uid, 'Deplace', x or 0, y or 0)
end

---Resizes the label, and returns its new size. If both x and y arguments are nil, it simply returns the size.
---@param x number|nil
---@param y number|nil
---@return number
---@return number
function monoe.text:size(x, y)
  if x or y then
    engine.call(self.uid, 'SetSize', x or 0, y or 0)
  end

  return engine.call(self.uid, 'GetSize')
end

---Repositions the label, and returns its new position. If both x and y arguments are nil, it simply returns the position.
---@param x number|nil
---@param y number|nil
---@return number
---@return number
function monoe.text:position(x, y)
  if x or y then
    engine.call(self.uid, 'SetPosition', x or 0, y or 0)
  end

  return engine.call(self.uid, 'GetPosition')
end

---Sets the text of the label
---@param ... unknown
function monoe.text:text(...)
  engine.call(self.uid, 'SetText', ...)
end

---Returns the text of the label
---@return string
function monoe.text:get()
  return engine.call(self.uid, 'Text')
end

---Sets the font
---@param path string|nil|'!'
---@param size integer|nil
function monoe.text:font(path, size)
  if path and path ~= '!' then
    engine.call(self.uid, 'SetFont', path)
  end

  if size and size >= 1 then
    engine.call(self.uid, 'SetFontSize', size)
  end
end

---Sets the font color
---@param r number Red
---@param g number Green
---@param b number Blue
---@param a number|nil Alpha
function monoe.text:color(r, g, b, a)
  engine.call(self.uid, 'SetFontColor', r, g, b, a or 1.0)
end

---Frees the resources (no longer usable)
function monoe.text:free()
  engine.call(self.uid, 'Free')
end

---Removes the object from its rendering server
function monoe.text:remove()
  engine.call(self.uid, 'Remove')
end

_G.monoe = monoe
_G.monoe.text = monoe.text
return monoe.text