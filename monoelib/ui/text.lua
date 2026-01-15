---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('monoelib.engine')
monoe = monoe or {}

---@class monoe.text
---@field uid integer
monoe.text = {}
monoe.text.__index = monoe.text

---Creates a new text label
---@param ... unknown
---@return monoe.text
function monoe.text.new(...)
  uid = engine.import('monoe.exe.Core.Bridge.Types.UI.TextLabel')

  if uid == -1 then
    error('got invalid UID on base monoe.text.new!')
  end

  engine.call(uid, 'SetText', ...)
  return setmetatable({ uid = uid }, monoe.text)
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
function monoe.text:gettext()
  return engine.call(self.uid, 'Text')
end

_G.monoe = monoe
_G.monoe.text = monoe.text
return monoe.text