monoe = monoe or {}

monoe.event = {
  _once = {},
  _listeners = {},
}

---Subscribes a function to an event once.
---@param name string
---@param fn function
function monoe.event.once(name, fn)
  local list = monoe.event._once[name]
  if not list then
    list = {}
    monoe.event._once[name] = list
  end
  list[#list + 1] = fn
end

---Subscribes a function to an event.
---@param name string
---@param fn function
function monoe.event.subscribe(name, fn)
  local list = monoe.event._listeners[name]
  if not list then
    list = {}
    monoe.event._listeners[name] = list
  end
  list[#list + 1] = fn
end

---Emits an event
---@param name string
---@param ... any
function monoe.event.emit(name, ...)
  local list = monoe.event._once[name]
  if list then
    monoe.event._once[name] = nil

    for i = 1, #list do
      local ok, err = pcall(list[i], ...)
      if not ok then
        print("Event error:", err)
      end
    end
  end

  list = monoe.event._listeners[name]

  if list then

    for i = 1, #list do
      local ok, err = pcall(list[i], ...)
      if not ok then
        print("Event error:", err)
      end
    end
  end
end

_G.monoe = monoe
_G.monoe.event = monoe.event

return monoe.event