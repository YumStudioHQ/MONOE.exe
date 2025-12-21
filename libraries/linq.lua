monoe = monoe or {}

---@class monoe.query
---@field data table
monoe.query = {}
monoe.query.__index = monoe.query

---Builds a new query type
---@param enum any
---@return monoe.query
function monoe.query.new(enum)
  local data = {}

  if type(enum) == "table" then
    for k, v in pairs(enum) do
      data[k] = v
    end
  else
    data[1] = enum
  end

  return setmetatable({ data = data }, monoe.query)
end

---iterates all elements of the query
---@param iterator function
---@return monoe.query
function monoe.query:foreach(iterator)
  for key, value in pairs(self.data) do
    iterator(key, value)
  end

  return self
end

---selects elements when the predicator returns true.
---@param predicate function
---@return monoe.query
function monoe.query:where(predicate)
  local arr = {}

  for key, value in pairs(self.data) do
    if predicate(key, value) then
      arr[key] = value
    end
  end

  return monoe.query.new(arr)
end

---maps a new query from the current query, releying on the provided mapper.
---@param mapper function
---@return monoe.query
function monoe.query:select(mapper)
  local arr = {}

  for key, value in pairs(self.data) do
    nkey, nvalue = mapper(key, value)
    arr[nkey] = nvalue
  end

  return monoe.query.new(arr)
end

---returns true when the predicator returns true for all elements.
---@param predicate any
---@return boolean
function monoe.query:all(predicate)
  for key, value in pairs(self.data) do
    if not predicate(key, value) then
      return false
    end
  end

  return true
end

---returns true when the predicator returns true with at least one element.
---@param predicate any
---@return boolean
function monoe.query:any(predicate)
  for key, value in pairs(self.data) do
    if predicate(key, value) then
      return true
    end
  end

  return false
end

---@param n integer
---@return monoe.query
function monoe.query:take(n)
  local result = {}
  local count = 0
  for k, v in pairs(self.data) do
    if count >= n then break end
    result[k] = v
    count = count + 1
  end
  return monoe.query.new(result)
end

---@param n number
---@return monoe.query
function monoe.query:skip(n)
  local result = {}
  local count = 0
  for k, v in pairs(self.data) do
    if count >= n then
      result[k] = v
    end
    count = count + 1
  end
  return monoe.query.new(result)
end

---@return monoe.query
function monoe.query:distinct()
  local seen = {}
  local result = {}
  for k, v in pairs(self.data) do
    if not seen[v] then
      seen[v] = true
      result[#result+1] = v
    end
  end
  return monoe.query.new(result)
end

---sums all numeric values
---@param mapper function
---@return integer
function monoe.query:sum(mapper)
  local total = 0
  for k, v in pairs(self.data) do
    local value = mapper and mapper(k, v) or v
    total = total + value
  end
  return total
end

---returns the maximum value
---@param mapper function
---@return any
function monoe.query:max(mapper)
  local maxValue = nil
  for k, v in pairs(self.data) do
    local value = mapper and mapper(k, v) or v
    if maxValue == nil or value > maxValue then
      maxValue = value
    end
  end
  return maxValue
end

function monoe.query:astable()
  local array = {}

  for key, value in pairs(self.data) do
    array[key] = value
  end

  return array
end

_G.monoe = monoe
_G.monoe.query = monoe.query
return monoe.query