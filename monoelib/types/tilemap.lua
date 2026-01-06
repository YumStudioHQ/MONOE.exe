local engine = require('monoelib.engine')
local image = require('monoelib.types.image')

monoe = monoe or {}

---@class monoe.tilemap
---@field uid integer Engine-side unique ID
---@field tilesets table<string, integer> Loaded tilesets by name
---@field last integer Last used ID or counter
---Represents a 2D tilemap that can place tiles, fill areas, and draw patterns using tilesets.
monoe.tilemap = {}
monoe.tilemap.__index = monoe.tilemap

local base = 'monoe.exe.Core.Bridge.Types.MTileMap'

---Creates a new `monoe.tilemap` object.
---@return monoe.tilemap Newly created tilemap
function monoe.tilemap.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.tilemap object: invalid UID for ' .. base)
  end

  return setmetatable({ uid = uid, tilesets = {}, last = 0 }, monoe.tilemap)
end

---Places a single tile at the specified tile coordinates.
---@param tile_x integer X coordinate on the tilemap
---@param tile_y integer Y coordinate on the tilemap
---@param tileset_name string Name of the tileset
---@param tileset_x integer X coordinate within the tileset
---@param tileset_y integer Y coordinate within the tileset
function monoe.tilemap:place(tile_x, tile_y, tileset_name, tileset_x, tileset_y)
  local id = self.tilesets[tileset_name]

  if type(id) ~= "number" then
    error('Tileset ' .. tostring(tileset_name) .. ' does not exist!')
  end

  engine.call(self.uid, 'PlaceTile', tile_x, tile_y, id, tileset_x, tileset_y)
end

---Loads a tileset into the tilemap.
---@param name string Name of the tileset
---@param source monoe.image|string Either a `monoe.image` or a file path
---@param tile_width integer Tile width in pixels (default 16)
---@param tile_height integer Tile height in pixels (default = `tile_width`)
function monoe.tilemap:load(name, source, tile_width, tile_height)
  local img = source
  local allocated = false
  
  if type(source) == "string" then
    img = image.new(source)
  end

  tile_width = tile_width or 16
  tile_height = tile_height or tile_width

  ---@diagnostic disable-next-line: assign-type-mismatch
  self.tilesets[name] = engine.call(self.uid, 'AddImage', img.uid, tile_width, tile_height)
  if allocated then image:free() end
end

---Scales the entire tilemap.
---@param x number Scale factor in X
---@param y number Scale factor in Y
function monoe.tilemap:scale(x, y)
  engine.call(self.uid, 'Scale', x, y)
end

---Draws a filled rectangle of tiles.
---@param x integer Starting tile X
---@param y integer Starting tile Y
---@param width integer Width in tiles
---@param height integer Height in tiles
---@param tileset string Tileset name
---@param tileset_x integer Tileset X coordinate
---@param tileset_y integer Tileset Y coordinate
function monoe.tilemap:rectangle(x, y, width, height, tileset, tileset_x, tileset_y)
  for i = 0, width - 1 do
    for j = 0, height - 1 do
      self:place(x + i, y + j, tileset, tileset_x, tileset_y)
    end
  end
end

---Fills a rectangular area with the same tile.
---@param x integer Starting tile X
---@param y integer Starting tile Y
---@param width integer Width in tiles
---@param height integer Height in tiles
---@param tileset string Tileset name
---@param tileset_x integer Tileset X coordinate
---@param tileset_y integer Tileset Y coordinate
function monoe.tilemap:fill(x, y, width, height, tileset, tileset_x, tileset_y)
  for ty = y, y + height - 1 do
    for tx = x, x + width - 1 do
      self:place(tx, ty, tileset, tileset_x, tileset_y)
    end
  end
end

---Draws a line of tiles between two points.
---@param x0 integer Start tile X
---@param y0 integer Start tile Y
---@param x1 integer End tile X
---@param y1 integer End tile Y
---@param tileset string Tileset name
---@param tileset_x integer Tileset X coordinate
---@param tileset_y integer Tileset Y coordinate
function monoe.tilemap:line(x0, y0, x1, y1, tileset, tileset_x, tileset_y)
  local dx = math.abs(x1 - x0)
  local dy = -math.abs(y1 - y0)
  local sx = x0 < x1 and 1 or -1
  local sy = y0 < y1 and 1 or -1
  local err = dx + dy

  while true do
    self:place(x0, y0, tileset, tileset_x, tileset_y)
    if x0 == x1 and y0 == y1 then break end
    local e2 = err * 2
    if e2 >= dy then err = err + dy; x0 = x0 + sx end
    if e2 <= dx then err = err + dx; y0 = y0 + sy end
  end
end

---Draws a rectangular border of tiles.
---@param x integer Starting tile X
---@param y integer Starting tile Y
---@param width integer Width in tiles
---@param height integer Height in tiles
---@param tileset string Tileset name
---@param tileset_x integer Tileset X coordinate
---@param tileset_y integer Tileset Y coordinate
function monoe.tilemap:border(x, y, width, height, tileset, tileset_x, tileset_y)
  for i = 0, width - 1 do
    self:place(x + i, y, tileset, tileset_x, tileset_y)
    self:place(x + i, y + height - 1, tileset, tileset_x, tileset_y)
  end
  for j = 1, height - 2 do
    self:place(x, y + j, tileset, tileset_x, tileset_y)
    self:place(x + width - 1, y + j, tileset, tileset_x, tileset_y)
  end
end

---Stamps a 2D pattern of tiles onto the map.
---Use `nil` in the pattern to skip a tile.
---@param x integer Top-left tile X
---@param y integer Top-left tile Y
---@param pattern table 2D array of {tileset_x, tileset_y} or nil
---@param tileset string Tileset name
function monoe.tilemap:stamp(x, y, pattern, tileset)
  for py, row in ipairs(pattern) do
    for px, tile in ipairs(row) do
      if tile then
        self:place(x + px - 1, y + py - 1, tileset, tile[1], tile[2])
      end
    end
  end
end

---Places a random tile from a list of choices.
---@param x integer Tile X
---@param y integer Tile Y
---@param tileset string Tileset name
---@param choices table Array of {tileset_x, tileset_y}
function monoe.tilemap:place_random(x, y, tileset, choices)
  local tile = choices[math.random(#choices)]
  self:place(x, y, tileset, tile[1], tile[2])
end

---Fills an area using random tiles based on a probability threshold.
---@param x integer Starting tile X
---@param y integer Starting tile Y
---@param width integer Width in tiles
---@param height integer Height in tiles
---@param tileset string Tileset name
---@param tiles table Array of {tileset_x, tileset_y}
---@param threshold number Probability (0–1) for a tile to be placed (default 0.5)
function monoe.tilemap:noise(x, y, width, height, tileset, tiles, threshold)
  threshold = threshold or 0.5

  for ty = y, y + height - 1 do
    for tx = x, x + width - 1 do
      if math.random() > threshold then
        local tile = tiles[math.random(#tiles)]
        self:place(tx, ty, tileset, tile[1], tile[2])
      end
    end
  end
end

_G.monoe = monoe
_G.monoe.tilemap = monoe.tilemap

return monoe.tilemap
