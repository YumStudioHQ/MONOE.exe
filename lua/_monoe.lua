---@diagnostic disable: lowercase-global
---As the API prefers using lowercase names...
local monoe = monoe or {}

natives = natives or {}

natives._typecall = natives._typecall or function(uid, methodname, ...)
  print('[monoe]: A default function has been called. You should not see this message.')
  return {}
end

natives._new = natives._new or function(name)
  print('[monoe]: A default function has been called. You should not see this message.')
  return -1
end

natives._is_engine_init = natives._is_engine_init or function() return false end

natives._get_methods_in_base = natives._get_methods_in_base or function(...) return {} end

natives._new_from = natives._new_from or function(uid)
  print('[monoe]: A default function has been called. You should not see this message.')
  return -1
end

natives._staticcall = natives._staticcall or function (typename, methodname, ...)
  print('[monoe]: A default function has been called. You should not see this message.')
  return {}
end

---Calls a monoe API function
---@param uid integer
---@param methodname string
---@param ... any
---@return any
function monoe.typecall(uid, methodname, ...)
  return natives._typecall(uid, methodname, ...)
end

---Creates a new type instance
---@param typename string
---@return integer
function monoe.new(typename)
  return natives._new(typename)
end

---returns a new object with the same type than the given UID.
---@param uid integer
---@return integer
function monoe.new_from(uid)
  return natives._new_from(uid)
end

---returns a list of string that contains each methods of the possible given UIDs
---@param ... any
---@return table<string>
function monoe.get_methods_in_base(...)
  return natives._get_methods_in_base(...)
end

---calls the given static method in the given type
---@param typename string
---@param methodname string
---@param ... any
---@return any
function monoe.staticcall(typename, methodname, ...)
  return natives._staticcall(typename, methodname, ...)
end

---Represents any monoe type.
---@class monoe.object
---@field uid integer
monoe.object = {}
monoe.object.__index = monoe.object

---creates a new monoe.object regarding the given argument. String arguments will create a type
---based on the name. If it's an integer, it'll consider the UID as an instance.
---@param uid integer|string
---@return monoe.object
function monoe.object.new(uid)
  if type(uid) == "number" then
    return setmetatable({ uid = uid }, monoe.object)
  elseif type(uid) == "string" then
    return setmetatable({ uid = monoe.new(uid) }, monoe.object)
  else
    return setmetatable({ uid = monoe.new("System.object") }, monoe.object)
  end
end

---converts the current object to a string
---@return string
function monoe.object:tostring()
  local s = monoe.typecall(self.uid, "ToString")
  if type(s) == "string" then return s end
  return "<monoe.object:Unknown>"
end

---calls a method in this object
---@param name string
---@param ... any
---@return any
function monoe.object:call(name, ...)
  return monoe.typecall(self.uid, name, ...)
end

---returns a new object with the same type of the current object.
---@return integer
function monoe.object:new_from()
  return monoe.new_from(self.uid)
end

---@return table<string>
function monoe.object:get_methods_in_base()
  return monoe.get_methods_in_base(self.uid)
end

---@return string
function monoe.object:__tostring()
  return self:tostring()
end

-- =====================================================
-- vector3 (float32)
-- =====================================================
---@class monoe.vector3
---@field x number
---@field y number
---@field z number
monoe.vector3 = { }
monoe.vector3.__index = monoe.vector3

---@param x number
---@param y number
---@param z number
---@return monoe.vector3
function monoe.vector3.new(x, y, z)
  return setmetatable({ x = x or 0.0, y = y or 0.0, z = z or 0.0 }, monoe.vector3)
end

---@return string
function monoe.vector3:pack()
  return string.pack("<fff", self.x, self.y, self.z)
end

---@param str string
---@return monoe.vector3
function monoe.vector3.unpack(str)
  local x, y, z = string.unpack("<fff", str)
  return monoe.vector3.new(x, y, z)
end

---@return string
function monoe.vector3:__tostring()
  return string.format("vector3(%.3f, %.3f, %.3f)", self.x, self.y, self.z)
end

---@return table<number>
function monoe.vector3:flat()
  return { self.x, self.y, self.z }
end

---@return monoe.vector3I
function monoe.vector3:to_i()
  return monoe.vector3I.new(
    math.floor(self.x + 0.5),
    math.floor(self.y + 0.5),
    math.floor(self.z + 0.5)
  )
end

-- Arithmetic metamethods
monoe.vector3.__add = function(a, b)
  return monoe.vector3.new(a.x + b.x, a.y + b.y, a.z + b.z)
end

monoe.vector3.__sub = function(a, b)
  return monoe.vector3.new(a.x - b.x, a.y - b.y, a.z - b.z)
end

monoe.vector3.__mul = function(a, b)
  if type(a) == "number" then
    return monoe.vector3.new(a * b.x, a * b.y, a * b.z)
  elseif type(b) == "number" then
    return monoe.vector3.new(a.x * b, a.y * b, a.z * b)
  end
end

monoe.vector3.__div = function(a, b)
  if type(b) == "number" then
    return monoe.vector3.new(a.x / b, a.y / b, a.z / b)
  end
end

monoe.vector3.__unm = function(a)
  return monoe.vector3.new(-a.x, -a.y, -a.z)
end

monoe.vector3.__eq = function(a, b)
  return a.x == b.x and a.y == b.y and a.z == b.z
end

-- =====================================================
-- vector2 (float32)
-- =====================================================
---@class monoe.vector2
---@field x number
---@field y number
monoe.vector2 = {}
monoe.vector2.__index = monoe.vector2

---@param x number
---@param y number
---@return monoe.vector2
function monoe.vector2.new(x, y)
  return setmetatable({ x = x or 0.0, y = y or 0.0 }, monoe.vector2)
end

---@return string
function monoe.vector2:pack()
  return string.pack("<ff", self.x, self.y)
end

---@param str string
---@return monoe.vector2
function monoe.vector2.unpack(str)
  local x, y = string.unpack("<ff", str)
  return monoe.vector2.new(x, y)
end

---@return string
function monoe.vector2:__tostring()
  return string.format("vector2(%.3f, %.3f)", self.x, self.y)
end

---@return table<number>
function monoe.vector2:flat()
  return { self.x, self.y }
end

---@return monoe.vector2I
function monoe.vector2:to_i()
  return monoe.vector2I.new(
    math.floor(self.x + 0.5),
    math.floor(self.y + 0.5)
  )
end

-- Arithmetic metamethods
monoe.vector2.__add = function(a, b)
  return monoe.vector2.new(a.x + b.x, a.y + b.y)
end

monoe.vector2.__sub = function(a, b)
  return monoe.vector2.new(a.x - b.x, a.y - b.y)
end

monoe.vector2.__mul = function(a, b)
  if type(a) == "number" then
    return monoe.vector2.new(a * b.x, a * b.y)
  elseif type(b) == "number" then
    return monoe.vector2.new(a.x * b, a.y * b)
  end
end

monoe.vector2.__div = function(a, b)
  if type(b) == "number" then
    return monoe.vector2.new(a.x / b, a.y / b)
  end
end

monoe.vector2.__unm = function(a)
  return monoe.vector2.new(-a.x, -a.y)
end

monoe.vector2.__eq = function(a, b)
  return a.x == b.x and a.y == b.y
end

-- =====================================================
-- vector3I (int32)
-- =====================================================
---@class monoe.vector3I
---@field x integer
---@field y integer
---@field z integer
monoe.vector3I = {}
monoe.vector3I.__index = monoe.vector3I

---@param x integer|table
---@param y integer
---@param z integer
---@return monoe.vector3I
function monoe.vector3I.new(x, y, z)
  if type(x) == "table" then
    return setmetatable({ x = x[1] or 0, y = x[2] or 0, z = x[3] or 0 }, monoe.vector3I)
  end
  return setmetatable({ x = x or 0, y = y or 0, z = z or 0 }, monoe.vector3I)
end

---@return string
function monoe.vector3I:pack()
  return string.pack("<iii", self.x, self.y, self.z)
end

---@param str string
---@return monoe.vector3I
function monoe.vector3I.unpack(str)
  local x, y, z = string.unpack("<iii", str)
  return monoe.vector3I.new(x, y, z)
end

---@return string
function monoe.vector3I:__tostring()
  return string.format("vector3I(%d, %d, %d)", self.x, self.y, self.z)
end

---@return table<integer>
function monoe.vector3I:flat()
  return { self.x, self.y, self.z }
end

---@return monoe.vector3
function monoe.vector3I:to_f()
  return monoe.vector3.new(self.x * 1.0, self.y * 1.0, self.z * 1.0)
end

-- Arithmetic metamethods
monoe.vector3I.__add = function(a, b)
  return monoe.vector3I.new(a.x + b.x, a.y + b.y, a.z + b.z)
end

monoe.vector3I.__sub = function(a, b)
  return monoe.vector3I.new(a.x - b.x, a.y - b.y, a.z - b.z)
end

monoe.vector3I.__mul = function(a, b)
  if type(a) == "number" then
    return monoe.vector3I.new(a * b.x, a * b.y, a * b.z)
  elseif type(b) == "number" then
    return monoe.vector3I.new(a.x * b, a.y * b, a.z * b)
  end
end

monoe.vector3I.__div = function(a, b)
  if type(b) == "number" then
    return monoe.vector3I.new(math.floor(a.x / b), math.floor(a.y / b), math.floor(a.z / b))
  end
end

monoe.vector3I.__unm = function(a)
  return monoe.vector3I.new(-a.x, -a.y, -a.z)
end
monoe.vector3I.__eq = function(a, b)
  return a.x == b.x and a.y == b.y and a.z == b.z
end

-- =====================================================
-- vector2I (int32)
-- =====================================================
---@class monoe.vector2I
---@field x integer
---@field y integer
monoe.vector2I = {}
monoe.vector2I.__index = monoe.vector2I

---@param x integer|table
---@param y integer
---@return monoe.vector2I
function monoe.vector2I.new(x, y)
  if type(x) == "table" then
    return setmetatable({ x = x[1] or 0, y = x[2] or 0 }, monoe.vector2I)
  end
  return setmetatable({ x = x or 0, y = y or 0 }, monoe.vector2I)
end

---@return string
function monoe.vector2I:pack()
  return string.pack("<ii", self.x, self.y)
end

---@param str string
---@return monoe.vector2I
function monoe.vector2I.unpack(str)
  local x, y = string.unpack("<ii", str)
  return monoe.vector2I.new(x, y)
end

---@return string
function monoe.vector2I:__tostring()
  return string.format("vector2I(%d, %d)", self.x, self.y)
end

---@return table<integer>
function monoe.vector2I:flat()
  return { self.x, self.y }
end

---@return monoe.vector2
function monoe.vector2I:to_f()
  return monoe.vector2.new(self.x * 1.0, self.y * 1.0)
end

-- Arithmetic metamethods
monoe.vector2I.__add = function(a, b)
  return monoe.vector2I.new(a.x + b.x, a.y + b.y)
end
monoe.vector2I.__sub = function(a, b)
  return monoe.vector2I.new(a.x - b.x, a.y - b.y)
end

monoe.vector2I.__mul = function(a, b)
  if type(a) == "number" then
    return monoe.vector2I.new(a * b.x, a * b.y)
  elseif type(b) == "number" then
    return monoe.vector2I.new(a.x * b, a.y * b)
  end
end

monoe.vector2I.__div = function(a, b)
  if type(b) == "number" then
    return monoe.vector2I.new(math.floor(a.x / b), math.floor(a.y / b))
  end
end

monoe.vector2I.__unm = function(a)
  return monoe.vector2I.new(-a.x, -a.y)
end
monoe.vector2I.__eq = function(a, b)
  return a.x == b.x and a.y == b.y
end

-- =====================================================
-- Global export
-- =====================================================
_G.monoe = monoe

return monoe