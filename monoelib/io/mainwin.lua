---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('monoelib.engine')
local rendering = require('monoelib.rendering')
local event = require('monoelib.event')

monoe = monoe or {}
monoe.engine = monoe.engine or {}

---@class monoe.engine.window
---@field uid integer
---Provides functions to manipulate the main engine window.
monoe.engine.window = {}
monoe.engine.window.__index = monoe.engine.window

---Creates a new window.
---@param _uid integer|nil
---@return monoe.engine.window
function monoe.engine.window.new(_uid)
  local uid = _uid or engine.import("monoe.exe.Core.Bridge.io.EngineWindow")

  if uid == -1 then
    error('got bad UID when creating a new window')
  end

  return setmetatable({ uid = uid }, monoe.engine.window)
end

---Changes the title of the main window.
---@param title string New window title
function monoe.engine.window:title(title)
  engine.call(self.uid, 'SetTitle', title)
end

---Sets or queries the size of the main window.
---@param width number|nil New width in pixels
---@param height number|nil New height in pixels
---@return integer current_width
---@return integer current_height
function monoe.engine.window:size(width, height)
  return engine.call(self.uid, 'SetSize', width, height)
end

---Sets or queries the position of the main window.
---@param x number|nil New x position
---@param y number|nil New y position
---@return integer current_x
---@return integer current_y
function monoe.engine.window:position(x, y)
  return engine.call(self.uid, 'SetPosition', x, y)
end

---Scales the window.
---@param x number Scale factor X
---@param y number Scale factor Y
---@return integer scaled_width
---@return integer scaled_height
function monoe.engine.window:scale(x, y)
  return engine.call(self.uid, 'Scale', x or 0, y or 0)
end

---Moves the window by a relative offset.
---@param dx number Horizontal offset
---@param dy number Vertical offset
---@return integer new_x
---@return integer new_y
function monoe.engine.window:move(dx, dy)
  return engine.call(self.uid, 'Move', dx or 0, dy or 0)
end

---Attaches an object or its children to the window for rendering.
---@param obj table Object with `.uid` or `.root` property
function monoe.engine.window:attach(obj)
  if type(obj) == "table" and obj.root then
    rendering.attach_tree(obj.root, obj)
    self:attach(obj.root)
  else
    engine.call(self.uid, 'Attach', obj.uid)
  end
end

---Returns the center coordinates of the window.
---@return number center_x
---@return number center_y
function monoe.engine.window:center()
  local width, height = self:size()
  return width / 2, height / 2
end

---@type monoe.engine.window
---@diagnostic disable-next-line: missing-fields
monoe.engine.mainwin = {}

event.once('_monoe_main', function ()
  local uid = engine.staticcall('monoe.exe.Core.Bridge.io.EngineMainWindow', 'GetMainWindow')
  monoe.engine.mainwin = monoe.engine.window.new(uid)
end)

_G.monoe = monoe
_G.monoe.engine = monoe.engine
_G.monoe.engine.window = monoe.engine.window

return monoe.engine.window
