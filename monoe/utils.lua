local monoe = monoe or {}

---@param on any
---@return table
function monoe.ensure_table(on)
  if type(on) ~= "table" then
    return { on }
  else
    return on
  end
end

-- =====================================================
-- Base64 encode/decode
-- =====================================================

local b64chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

---Encodes a binary string to base64
---@param data string
---@return string
function monoe.base64_encode(data)
  return ((data:gsub('.', function(x)
    local r, b = '', x:byte()
    for i = 8, 1, -1 do
      r = r .. (b % 2 ^ i - b % 2 ^ (i - 1) > 0 and '1' or '0')
    end
    return r
  end) .. '0000'):gsub('%d%d%d?%d?%d?%d?', function(x)
    if #x < 6 then return '' end
    local c = 0
    for i = 1, 6 do
      c = c + (x:sub(i, i) == '1' and 2 ^ (6 - i) or 0)
    end
    return b64chars:sub(c + 1, c + 1)
  end) .. ({ '', '==', '=' })[#data % 3 + 1])
end

---Decodes a base64 string back to raw binary
---@param data string
---@return string
function monoe.base64_decode(data)
  data = data:gsub('[^' .. b64chars .. '=]', '')
  return (data:gsub('.', function(x)
    if x == '=' then return '' end
    local r, f = '', (b64chars:find(x) - 1)
    for i = 6, 1, -1 do
      r = r .. (f % 2 ^ i - f % 2 ^ (i - 1) > 0 and '1' or '0')
    end
    return r
  end):gsub('%d%d%d?%d?%d?%d?%d?%d?', function(x)
    if #x ~= 8 then return '' end
    local c = 0
    for i = 1, 8 do
      c = c + (x:sub(i, i) == '1' and 2 ^ (8 - i) or 0)
    end
    return string.char(c)
  end))
end

function monoe.is_class(t)
  return type(t) == "table" and t.__index == t
end

function monoe.is_instance(o, class)
  return type(o) == "table" and getmetatable(o) == class
end

---Represents a binary stream.
---@class monoe.binstream
---@field data string
---@field pos integer
monoe.binstream = {}
monoe.binstream.__index = monoe.binstream

---Create new binstream
---@param src? string
---@return monoe.binstream
function monoe.binstream.new(src)
  return setmetatable({
    data = src or "",
    pos = 1,
  }, monoe.binstream)
end
---comment
---@param v integer
function monoe.binstream:write_u8(v)  self.data = self.data .. string.pack("<I1", v) end
---@param v integer
function monoe.binstream:write_i8(v)  self.data = self.data .. string.pack("<b", v) end
---@param v integer
function monoe.binstream:write_u16(v) self.data = self.data .. string.pack("<I2", v) end
---@param v integer
function monoe.binstream:write_i16(v) self.data = self.data .. string.pack("<h", v) end
---@param v integer
function monoe.binstream:write_u32(v) self.data = self.data .. string.pack("<I4", v) end
---@param v integer
function monoe.binstream:write_i32(v) self.data = self.data .. string.pack("<i4", v) end
---@param v number
function monoe.binstream:write_f32(v) self.data = self.data .. string.pack("<f", v) end
---@param v number
function monoe.binstream:write_f64(v) self.data = self.data .. string.pack("<d", v) end
---@param s string
function monoe.binstream:write_str(s)
  s = s or ""
  self:write_u32(#s)
  self.data = self.data .. s
end

---@param vec monoe.vector3
function monoe.binstream:write_vector3(vec)
  self:write_f32(vec.x)
  self:write_f32(vec.y)
  self:write_f32(vec.z)
end

---@param vec monoe.vector3I
function monoe.binstream:write_vector3I(vec)
  self:write_i32(vec.x)
  self:write_i32(vec.y)
  self:write_i32(vec.z)
end

---@param vec monoe.vector2
function monoe.binstream:write_vector2(vec)
  self:write_f32(vec.x)
  self:write_f32(vec.y)
end

---@param vec monoe.vector2I
function monoe.binstream:write_vector2I(vec)
  self:write_i32(vec.x)
  self:write_i32(vec.y)
end


local function read(self, fmt)
  local val
  val, self.pos = string.unpack(fmt, self.data, self.pos)
  return val
end

---@return integer
function monoe.binstream:read_u8()  return read(self, "<I1") end
---@return integer
function monoe.binstream:read_i8()  return read(self, "<b") end
---@return integer
function monoe.binstream:read_u16() return read(self, "<I2") end
---@return integer
function monoe.binstream:read_i16() return read(self, "<h") end
---@return integer
function monoe.binstream:read_u32() return read(self, "<I4") end
---@return integer
function monoe.binstream:read_i32() return read(self, "<i4") end
---@return number
function monoe.binstream:read_f32() return read(self, "<f") end
---@return number
function monoe.binstream:read_f64() return read(self, "<d") end

---@return monoe.vector3
function monoe.binstream:read_vector3()
  return monoe.vector3.new(self:read_f32(), self:read_f32(), self:read_f32())
end

---@return monoe.vector3I
function monoe.binstream:read_vector3I()
  return monoe.vector3I.new(self:read_i32(), self:read_i32(), self:read_i32())
end

---@return monoe.vector2
function monoe.binstream:read_vector2()
  return monoe.vector2.new(self:read_f32(), self:read_f32())
end

---@return monoe.vector2I
function monoe.binstream:read_vector2I()
  return monoe.vector2I.new(self:read_i32(), self:read_i32())
end

---@return string
function monoe.binstream:read_str()
  local len = self:read_u32()
  local s = self.data:sub(self.pos, self.pos + len - 1)
  self.pos = self.pos + len
  return s
end

---@param pos integer
function monoe.binstream:seek(pos)
  self.pos = pos or 1
end

---@return integer
function monoe.binstream:tell()
  return self.pos
end

---@return string
function monoe.binstream:bytes()
  return self.data
end

-- =====================================================
-- Global export
-- =====================================================
_G.monoe = monoe
return monoe
