monoe = monoe or {}

---@class monoe.color
---@field r integer Red   (0–255)
---@field g integer Green (0–255)
---@field b integer Blue  (0–255)
---@field a integer Alpha (0–255)
---represents an RGBA color (encoded on 4 bytes, RRGGBBAA)
monoe.color = {}
monoe.color.__index = monoe.color

-- Utility: clamp to byte range
local function clamp8(v)
  v = math.floor(tonumber(v) or 0)
  if v < 0 then return 0 end
  if v > 255 then return 255 end
  return v
end

---Creates a new color.
---@param r integer|nil
---@param g integer|nil
---@param b integer|nil
---@param a integer|nil Alpha (default 255)
---@return monoe.color
function monoe.color.new(r, g, b, a)
  return setmetatable({
    r = clamp8(r),
    g = clamp8(g),
    b = clamp8(b),
    a = clamp8(a ~= nil and a or 255)
  }, monoe.color)
end

---Creates a color from a grayscale value.
---@param v integer
---@param a integer|nil
---@return monoe.color
function monoe.color.gray(v, a)
  v = clamp8(v)
  return monoe.color.new(v, v, v, a)
end

---Creates a color from a hex integer (0xRRGGBB or 0xRRGGBBAA).
---@param hex integer
---@return monoe.color
function monoe.color.from_hex(hex)
  local r = (hex >> 24) & 0xFF
  local g = (hex >> 16) & 0xFF
  local b = (hex >> 8)  & 0xFF
  local a = hex & 0xFF

  -- If alpha looks missing, assume opaque
  if hex <= 0xFFFFFF then
    return monoe.color.new(
      (hex >> 16) & 0xFF,
      (hex >> 8)  & 0xFF,
      hex & 0xFF,
      255
    )
  end

  return monoe.color.new(r, g, b, a)
end

---Creates a color from a packed RGBA32 integer.
---@param value integer Unsigned 32-bit (0xRRGGBBAA)
---@return monoe.color
function monoe.color.from_int(value)
  return monoe.color.from_hex(value)
end

---Returns the color as a packed RGBA32 unsigned integer.
---@return integer
function monoe.color:to_int()
  return ((self.r & 0xFF) << 24)
       | ((self.g & 0xFF) << 16)
       | ((self.b & 0xFF) << 8)
       |  (self.a & 0xFF)
end

---Returns a copy of this color.
---@return monoe.color
function monoe.color:clone()
  return monoe.color.new(self.r, self.g, self.b, self.a)
end

---Returns a string representation.
function monoe.color:__tostring()
  return string.format(
    "monoe.color(%d, %d, %d, %d)",
    self.r, self.g, self.b, self.a
  )
end

---Adds another color to this one (channel-wise, clamped).
---@param other monoe.color
---@return monoe.color
function monoe.color:add(other)
  return monoe.color.new(
    self.r + other.r,
    self.g + other.g,
    self.b + other.b,
    self.a + other.a
  )
end

function monoe.color.__add(a, b)
  return a:add(b)
end

---Returns a shadow version of the color.
---@param factor number Darkening factor (0–1), default 0.5
---@return monoe.color
function monoe.color:shadow(factor)
  factor = tonumber(factor) or 0.5
  if factor < 0 then factor = 0 end
  if factor > 1 then factor = 1 end

  return monoe.color.new(
    self.r * factor,
    self.g * factor,
    self.b * factor,
    self.a
  )
end

---Returns perceived luminosity (0–255).
---@return integer
function monoe.color:luminosity()
  return math.floor(
    0.2126 * self.r +
    0.7152 * self.g +
    0.0722 * self.b
  )
end

---Returns black or white depending on contrast.
---@return monoe.color
function monoe.color:contrast()
  if self:luminosity() > 128 then
    return monoe.color.black
  else
    return monoe.color.white
  end
end

---Generates a palette from this color.
---@param steps integer Number of variations
---@return monoe.color[]
function monoe.color:palette(steps)
  steps = math.max(1, math.floor(steps or 5))
  local palette = {}

  for i = 1, steps do
    local t = (i - 1) / (steps - 1)
    local factor = 0.3 + t * 0.7
    palette[i] = self:shadow(factor)
  end

  return palette
end

---Returns a Godot-friendly color string.
---Either "#RRGGBB" if alpha is 255, else "#RRGGBBAA"
---@return string
function monoe.color:string()
  local function byte_to_hex(b)
    return string.format("%02X", b & 0xFF)
  end

  if self.a == 255 then
    return "#" .. byte_to_hex(self.r) .. byte_to_hex(self.g) .. byte_to_hex(self.b)
  else
    return "#" .. byte_to_hex(self.r) .. byte_to_hex(self.g) .. byte_to_hex(self.b) .. byte_to_hex(self.a)
  end
end

---Creates a color from a string.
---Supports "#RRGGBB", "#RRGGBBAA", "R,G,B" or "R,G,B,A"
---@param str string
---@return monoe.color
function monoe.color.from_string(str)
  if type(str) ~= "string" then
    error("from_string expects a string")
  end

  if monoe.color[str] ~= nil then
    return monoe.color[str]
  end

  str = str:match("^%s*(.-)%s*$")

  -- Hex format: #RRGGBB or #RRGGBBAA
  if str:sub(1,1) == "#" then
    local hex = str:sub(2)
    if #hex == 6 then
      local r = tonumber(hex:sub(1,2), 16)
      local g = tonumber(hex:sub(3,4), 16)
      local b = tonumber(hex:sub(5,6), 16)
      return monoe.color.new(r, g, b, 255)
    elseif #hex == 8 then
      local r = tonumber(hex:sub(1,2), 16)
      local g = tonumber(hex:sub(3,4), 16)
      local b = tonumber(hex:sub(5,6), 16)
      local a = tonumber(hex:sub(7,8), 16)
      return monoe.color.new(r, g, b, a)
    else
      error("Invalid hex color string: " .. str)
    end
  end

  -- CSV format: "R,G,B" or "R,G,B,A"
  local parts = {}
  for num in str:gmatch("%d+") do
    table.insert(parts, tonumber(num))
  end

  if #parts == 3 then
    return monoe.color.new(parts[1], parts[2], parts[3], 255)
  elseif #parts == 4 then
    return monoe.color.new(parts[1], parts[2], parts[3], parts[4])
  else
    error("Invalid color string: " .. str)
  end
end

-- Common colors
monoe.color.white   = monoe.color.new(255, 255, 255)
monoe.color.black   = monoe.color.new(0, 0, 0)
monoe.color.red     = monoe.color.new(255, 0, 0)
monoe.color.green   = monoe.color.new(0, 255, 0)
monoe.color.blue    = monoe.color.new(0, 0, 255)
monoe.color.clear   = monoe.color.new(0, 0, 0, 0)

monoe.color.normal_gray        = monoe.color.gray(128)
monoe.color.light_gray  = monoe.color.gray(192)
monoe.color.dark_gray   = monoe.color.gray(64)

monoe.color.yellow      = monoe.color.new(255, 255, 0)
monoe.color.cyan        = monoe.color.new(0, 255, 255)
monoe.color.magenta     = monoe.color.new(255, 0, 255)

monoe.color.orange      = monoe.color.new(255, 165, 0)
monoe.color.purple      = monoe.color.new(128, 0, 128)
monoe.color.pink        = monoe.color.new(255, 105, 180)
monoe.color.brown       = monoe.color.new(139, 69, 19)

monoe.color.lime        = monoe.color.new(50, 205, 50)
monoe.color.navy        = monoe.color.new(0, 0, 128)
monoe.color.teal        = monoe.color.new(0, 128, 128)
monoe.color.olive       = monoe.color.new(128, 128, 0)


_G.monoe = monoe
_G.monoe.color = monoe.color

return monoe.color
