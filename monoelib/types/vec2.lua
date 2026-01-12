monoe = monoe or {}

---@class monoe.vec2
---@field x number
---@field y number
monoe.vec2 = {}
monoe.vec2.__index = monoe.vec2

---Creates a new vec2.
---@param x number|nil
---@param y number|nil
---@return monoe.vec2
function monoe.vec2.new(x, y)
  return setmetatable({
    x = x or 0,
    y = y or 0
  }, monoe.vec2)
end

---Zero vector (0, 0).
---@return monoe.vec2
function monoe.vec2.zero()
  return monoe.vec2.new(0, 0)
end

---@param v monoe.vec2
---@return monoe.vec2
function monoe.vec2:add(v)
  return monoe.vec2.new(self.x + v.x, self.y + v.y)
end

---@param v monoe.vec2
---@return monoe.vec2
function monoe.vec2:sub(v)
  return monoe.vec2.new(self.x - v.x, self.y - v.y)
end

---@param s number
---@return monoe.vec2
function monoe.vec2:mul(s)
  return monoe.vec2.new(self.x * s, self.y * s)
end

---@param s number
---@return monoe.vec2
function monoe.vec2:div(s)
  return monoe.vec2.new(self.x / s, self.y / s)
end

---@return number
function monoe.vec2:length_sq()
  return self.x * self.x + self.y * self.y
end

---@return number
function monoe.vec2:length()
  return math.sqrt(self:length_sq())
end

---@param v monoe.vec2
---@return number
function monoe.vec2:distance(v)
  local dx = self.x - v.x
  local dy = self.y - v.y
  return math.sqrt(dx * dx + dy * dy)
end

---Returns normalized vector and original length.
---@return monoe.vec2
---@return number len
function monoe.vec2:normalized()
  local len = self:length()
  if len == 0 then
    return monoe.vec2.zero(), 0
  end
  return monoe.vec2.new(self.x / len, self.y / len), len
end

---@param v monoe.vec2
---@return number
function monoe.vec2:dot(v)
  return self.x * v.x + self.y * v.y
end

---@param v monoe.vec2
---@param t number
---@return monoe.vec2
function monoe.vec2:lerp(v, t)
  return monoe.vec2.new(
    self.x + (v.x - self.x) * t,
    self.y + (v.y - self.y) * t
  )
end

---@param max_len number
---@return monoe.vec2
function monoe.vec2:clamp_length(max_len)
  local len = self:length()
  if len == 0 or len <= max_len then
    return monoe.vec2.new(self.x, self.y)
  end

  local s = max_len / len
  return monoe.vec2.new(self.x * s, self.y * s)
end

---@return number
---@return number
function monoe.vec2:unpack()
  return self.x, self.y
end

---@param x number
---@param y number
---@return self
function monoe.vec2:set(x, y)
  self.x = x
  self.y = y
  return self
end

_G.monoe = monoe
_G.monoe.vec2 = monoe.vec2

return monoe.vec2
