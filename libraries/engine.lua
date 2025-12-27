local event = require('libraries.event')

---@class monoe
monoe = monoe or {}

---imports a class
---@param class string
---@return integer
function monoe.import(class) return -1 end

---calls a method
---@param uid integer
---@param method string
---@param ... any
---@return unknown
function monoe.call(uid, method, ...) return {} end

---calls a static method on a static base
---@param base string
---@param method string
---@param ... any
---@return any
function monoe.staticcall(base, method, ...)
  print('default function called')
  print(debug.traceback())
end

monoe.env = monoe.env or {
  debug = false,
  path = ''
}

function monoe.load(name, path)
  event.once('@load', function ()
    _G[name] = require(path)
  end)
end

_G.monoe = monoe
return monoe