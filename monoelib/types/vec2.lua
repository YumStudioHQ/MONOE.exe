monoe = monoe or {}

---@class monoe.vec2
---@field x number -- X component
---@field y number -- Y component
monoe.vec2 = {}
monoe.vec2.__index = monoe.vec2

---Creates a new 2D vector.
---If no values are provided, defaults to (0, 0).
---@param x number|nil
---@param y number|nil
---@return monoe.vec2
function monoe.vec2.new(x, y)
  return setmetatable({
    x = x or 0,
    y = y or 0
  }, monoe.vec2)
end

---Returns a zero vector (0, 0).
---@return monoe.vec2
function monoe.vec2.zero()
  return monoe.vec2.new(0, 0)
end

---Adds another vector to this one.
---@param v monoe.vec2
---@return monoe.vec2 result
function monoe.vec2:add(v)
  return monoe.vec2.new(self.x + v.x, self.y + v.y)
end

---Subtracts another vector from this one.
---@param v monoe.vec2
---@return monoe.vec2 result
function monoe.vec2:sub(v)
  return monoe.vec2.new(self.x - v.x, self.y - v.y)
end

---Multiplies this vector by a scalar.
---@param s number
---@return monoe.vec2 result
function monoe.vec2:mul(s)
  return monoe.vec2.new(self.x * s, self.y * s)
end

---Divides this vector by a scalar.
---@param s number
---@return monoe.vec2 result
function monoe.vec2:div(s)
  return monoe.vec2.new(self.x / s, self.y / s)
end

---Returns the squared length of the vector.
---Useful when you want distance comparisons without sqrt().
---@return number
function monoe.vec2:length_sq()
  return self.x * self.x + self.y * self.y
end

---Returns the length (magnitude) of the vector.
---@return number
function monoe.vec2:length()
  return math.sqrt(self:length_sq())
end

---Returns the distance between this vector and another one.
---@param v monoe.vec2
---@return number
function monoe.vec2:distance(v)
  local dx = self.x - v.x
  local dy = self.y - v.y
  return math.sqrt(dx * dx + dy * dy)
end

---Returns a normalized (unit) vector and the original length.
---If the vector is zero, returns (0,0) and length 0.
---@return monoe.vec2 normalized
---@return number length
function monoe.vec2:normalized()
  local len = self:length()
  if len == 0 then
    return monoe.vec2.zero(), 0
  end
  return monoe.vec2.new(self.x / len, self.y / len), len
end

---Returns the dot product between this vector and another.
---Useful for angles, projections, and direction checks.
---@param v monoe.vec2
---@return number
function monoe.vec2:dot(v)
  return self.x * v.x + self.y * v.y
end

---Linearly interpolates between this vector and another.
---t = 0 → this vector
---t = 1 → target vector
---@param v monoe.vec2
---@param t number
---@return monoe.vec2
function monoe.vec2:lerp(v, t)
  return monoe.vec2.new(
    self.x + (v.x - self.x) * t,
    self.y + (v.y - self.y) * t
  )
end

---Clamps the vector length so it does not exceed max_len.
---Keeps direction the same.
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

---Returns x and y as separate values.
---Useful for APIs that expect unpacked numbers.
---@return number x
---@return number y
function monoe.vec2:unpack()
  return self.x, self.y
end

---Sets the vector components directly.
---@param x number
---@param y number
---@return self
function monoe.vec2:set(x, y)
  self.x = x
  self.y = y
  return self
end

---Checks if this vector (point) is inside an axis-aligned bounding box.
---`min` is bottom-left, `max` is top-right.
---@param min monoe.vec2
---@param max monoe.vec2
---@return boolean
function monoe.vec2:inside_aabb(min, max)
  return self.x >= min.x and self.x <= max.x
     and self.y >= min.y and self.y <= max.y
end

_G.monoe = monoe
_G.monoe.vec2 = monoe.vec2

return monoe.vec2
