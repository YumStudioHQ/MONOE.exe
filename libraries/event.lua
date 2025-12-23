monoe = monoe or {}

monoe.event = {
  _once = {}
}

function monoe.event.once(name, fn)
  if not monoe.event._once[name] then
    monoe.event._once[name] = {}
  end
  table.insert(monoe.event._once[name], fn)
end

function monoe.event.emit(name, ...)
  local list = monoe.event._once[name]
  if not list then return end

  monoe.event._once[name] = nil

  for i = 1, #list do
    local ok, err = pcall(list[i], ...)
    if not ok then
      print("Event error:", err)
    end
  end
end

_G.monoe = monoe
_G.monoe.event = monoe.event

return monoe.event