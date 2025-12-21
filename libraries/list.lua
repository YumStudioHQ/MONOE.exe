local linq = require('libraries.linq')

monoe = monoe or {}

---@class monoe.list : monoe.query
---@field data table
monoe.list = {}
monoe.list.__index = monoe.list
setmetatable(monoe.list, { __index = monoe.query })

---@param from any
---@return monoe.list
function monoe.list.new(from)
  local data = {}
  local final = {}
  if type(from) == "table" then
    if from.__index == linq.__index then
      data = from.data
    else
      data = from
    end
  elseif from then
    data[1] = from
  end

  for key, value in pairs(data) do
    final[key] = value
  end

  local list = setmetatable({ data = final }, monoe.list)
  ---@cast list monoe.list
  return list
end

function monoe.list:append(element)
  self.data[#self.data+1] = element

  return self
end

---@return monoe.list
function monoe.list:reverse()
  local new = {}
  for i = #self.data, 1, -1 do
    new[#new+1] = self.data[i]
  end
  return monoe.list.new(new)
end

---@return unknown
function monoe.list:first()
  return self.data[1]
end

---@return unknown
function monoe.list:last()
  return self.data[#self.data]
end

function monoe.query:aslist()
  local is_arr = true
  local n = 0
  for k, v in pairs(self.data) do
    if type(k) ~= "number" or k <= 0 then
      is_arr = false
      break
    end
    n = n + 1
  end

  if is_arr and n == #self.data then
    return monoe.list.new(self.data)
  else
    local list = monoe.list.new()
    for k, v in pairs(self.data) do
      if type(k) == "number" then
        list:append(v)
      else
        list:append({k, v})
      end
    end
    return list
  end
end

_G.monoe = monoe
_G.monoe.list = monoe.list
return monoe.list