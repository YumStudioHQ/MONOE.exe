monoe = monoe or {}

monoe.unique_event = {
  _once = {}
}

function monoe.unique_event.once(name, fn)
  if not monoe.unique_event._once[name] then
    monoe.unique_event._once[name] = {}
  end
  table.insert(monoe.unique_event._once[name], fn)
end

function monoe.emit(name, ...)
  local list = monoe.unique_event._once[name]
  if not list then return end

  monoe.unique_event._once[name] = nil

  for i = 1, #list do
    local ok, err = pcall(list[i], ...)
    if not ok then
      print("Event error:", err)
    end
  end
end

_G.monoe = monoe
_G.monoe.unique_event = monoe.unique_event

return monoe.unique_event