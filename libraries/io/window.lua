---@diagnostic disable: return-type-mismatch, missing-return-value
local engine = require('libraries.engine')
local rendering = require('libraries.rendering')

monoe = monoe or {}
monoe.engine = monoe.engine or {}
monoe.engine.window = {}

local base = "monoe.exe.Core.Bridge.io.EngineWindow"

---changes the title of the main window
---@param name string
function monoe.engine.window.title(name)
  engine.staticcall(base, 'SetTitle', name)
end

---changes the size of the main window
---@param x number|nil
---@param y number|nil
---@return integer, integer
function monoe.engine.window.size(x, y)
  return engine.staticcall(base, 'SetSize', x, y)
end

---@param x number|nil
---@param y number|nil
---@return integer, integer
function monoe.engine.window.position(x, y)
  return engine.staticcall(base, 'SetPosition', x, y)
end

---@param x number
---@param y number
---@return integer, integer
function monoe.engine.window.scale(x, y)
  return engine.staticcall(base, 'Scale', x or 0, y or 0)
end

---@param x number|nil
---@param y number|nil
---@return integer, integer
function monoe.engine.window.move(x, y)
  return engine.staticcall(base, 'Move', x or 0, y or 0)
end

function monoe.engine.window.attach(obj)
  if type(obj) == "table" and obj.root then
    rendering.attach_tree(obj.root, obj)
    monoe.engine.window.attach(obj.root)
  else
    engine.staticcall(base, 'Attach', obj.uid)
  end
end

---returns a 2D vector to the center of the window.
---@return number
---@return number
function monoe.engine.window.center()
  local x, y = monoe.engine.window.size()
  return x / 2, y / 2
end

_G.monoe = monoe
_G.monoe.engine = monoe.engine
_G.monoe.engine.window = monoe.engine.window
return monoe.engine.window