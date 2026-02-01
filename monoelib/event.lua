monoe = monoe or {}

---@class monoe.event
---@field _once table<string, function[]> Listeners that trigger only once
---@field _listeners table<string, function[]> Persistent listeners
---Built-in events, those that are fired by the engine:
---* @collect: Called after each physic process, this one is designed for cleaning up some resources.
---* @ready: Called once the engine is ready.
---* @process: Called each frames.
---* @physics: Called at a fixed point, for physic updates. You may move entities here (entity.move() function should be called from here)
---* @input: Called when any input interaction happens.
---* @onexit: Called when an exit has been requested and not rejected (at this state, you cannot prevent exiting)
---* @hot: Internal specific, prefer not using it. (Fired when a file changes)
---* @load: Do not use this one. (please)
monoe.event = {
  _once = {},
  _listeners = {},
}

---Subscribes a function to an event that will trigger only once.
---@param name string The event name.
---@param fn function The callback function to execute when the event is emitted.
function monoe.event.once(name, fn)
  local list = monoe.event._once[name]
  if not list then
    list = {}
    monoe.event._once[name] = list
  end
  list[#list + 1] = fn
end

---Subscribes a function to an event that triggers every time the event is emitted.
---@param name string The event name.
---@param fn function The callback function to execute.
function monoe.event.subscribe(name, fn)
  local list = monoe.event._listeners[name]
  if not list then
    list = {}
    monoe.event._listeners[name] = list
  end
  list[#list + 1] = fn
end

-- Internal function to safely call a listener
---@param eventName string
---@param fn function
---@param ... any Arguments to pass to the listener
local function call(eventName, fn, ...)
  local ok, err = pcall(fn, ...)
  if not ok then
    error('Error when calling function ' .. tostring(fn) ..
          ' during event ' .. eventName ..
          ' ; ok: ' .. tostring(ok) .. ', err: ' .. err)
  end
end

---Unsubscribes a function from a persistent event.
---@param name string The event name.
---@param fn function The callback function to remove.
function monoe.event.unsubscribe(name, fn)
  local list = monoe.event._listeners[name]
  if not list then return end

  for i = #list, 1, -1 do
    if list[i] == fn then
      table.remove(list, i)
    end
  end
end

---Emits an event, calling all subscribed functions.
---@param name string The event name.
---@param ... any Additional arguments to pass to the listeners.
function monoe.event.emit(name, ...)
  local list = monoe.event._once[name]
  if list then
    monoe.event._once[name] = nil
    for i = 1, #list do
      call(name, list[i], ...)
    end
  end

  list = monoe.event._listeners[name]
  if list then
    for i = 1, #list do
      call(name, list[i], ...)
    end
  end
end

monoe.event.subscribe("@collect", function()
  for name, list in pairs(monoe.event._once) do
    if not list or #list == 0 then
      monoe.event._once[name] = nil
    end
  end

  for name, list in pairs(monoe.event._listeners) do
    if not list or #list == 0 then
      monoe.event._listeners[name] = nil
    end
  end
end)

_G.monoe = monoe
_G.monoe.event = monoe.event

return monoe.event
