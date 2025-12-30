local engine = require('libraries.engine')
local image = require('libraries.image')

monoe = monoe or {}

---@class monoe.tilemap
---@field uid integer
---@field tilesets table<string, integer>
---@field last integer
monoe.tilemap = {}
monoe.tilemap.__index = monoe.tilemap

local base = 'monoe.exe.Core.Bridge.Types.MTileMap'

function monoe.tilemap.new()
  local uid = engine.import(base)

  if uid == -1 then
    error('got invalid UID when creating an instance of base ' .. base)
  end

  return setmetatable({ uid = uid, tilesets = {}, last = 0 }, monoe.tilemap)
end

---places a tile at the given x,y position.
---@param tile_x integer The x position on the timemap
---@param tile_y integer The y position on the timemap
---@param tilesetname string The tileset_x's name
---@param tileset_x integer The tileset's x position
---@param tileset_y integer The tileset's y position
function monoe.tilemap:place(tile_x, tile_y, tilesetname, tileset_x, tileset_y)
  local id = self.tilesets[tilesetname]

  if type(id) ~= "number" then
    error('tileset ' .. tostring(tilesetname) .. ' does not exist!')
  end

  engine.call(self.uid, 'PlaceTile', tile_x, tile_y, id, tileset_x, tileset_y)
end

---loads a tileset.
---@param name string
---@param img monoe.image|string
---@param tile_w integer
---@param tile_h integer
function monoe.tilemap:load(name, img, tile_w, tile_h)
  local src = img

  if type(img) == "string" then
    src = image.new(img)
  end

  tile_w = tile_w or 16
  tile_h = tile_h or tile_w

  self.tilesets[name] = engine.call(self.uid, 'AddImage', src.uid, tile_w, tile_h)
end

function monoe.tilemap:scale(x, y)
  engine.call(self.uid, 'Scale', x, y)
end

---generates a rectangle
---@param x integer
---@param y integer
---@param h integer
---@param w integer
---@param set string
---@param set_x integer
---@param set_y integer
function monoe.tilemap:rectangle(x, y, h, w, set, set_x, set_y)
  for i = 0, h, 1 do
    for j = 0, w, 1 do
      self:place(x + i, y + j, set, set_x, set_y)
    end
  end
end

---Fills a rectangular area with the same tile.
---@param x integer Starting tile x
---@param y integer Starting tile y
---@param w integer Width in tiles
---@param h integer Height in tiles
---@param set string Tileset name
---@param set_x integer Tileset x coordinate
---@param set_y integer Tileset y coordinate
function monoe.tilemap:fill(x, y, w, h, set, set_x, set_y)
  for ty = y, y + h - 1 do
    for tx = x, x + w - 1 do
      self:place(tx, ty, set, set_x, set_y)
    end
  end
end

---Draws a line of tiles between two points.
---@param x0 integer Start tile x
---@param y0 integer Start tile y
---@param x1 integer End tile x
---@param y1 integer End tile y
---@param set string Tileset name
---@param set_x integer Tileset x coordinate
---@param set_y integer Tileset y coordinate
function monoe.tilemap:line(x0, y0, x1, y1, set, set_x, set_y)
  local dx = math.abs(x1 - x0)
  local dy = -math.abs(y1 - y0)
  local sx = x0 < x1 and 1 or -1
  local sy = y0 < y1 and 1 or -1
  local err = dx + dy

  while true do
    self:place(x0, y0, set, set_x, set_y)
    if x0 == x1 and y0 == y1 then break end
    local e2 = err * 2
    if e2 >= dy then err = err + dy; x0 = x0 + sx end
    if e2 <= dx then err = err + dx; y0 = y0 + sy end
  end
end

---Draws a rectangular border using tiles.
---@param x integer Starting tile x
---@param y integer Starting tile y
---@param w integer Width in tiles
---@param h integer Height in tiles
---@param set string Tileset name
---@param set_x integer Tileset x coordinate
---@param set_y integer Tileset y coordinate
function monoe.tilemap:border(x, y, w, h, set, set_x, set_y)
  for i = 0, w - 1 do
    self:place(x + i, y, set, set_x, set_y)
    self:place(x + i, y + h - 1, set, set_x, set_y)
  end

  for j = 1, h - 2 do
    self:place(x, y + j, set, set_x, set_y)
    self:place(x + w - 1, y + j, set, set_x, set_y)
  end
end

---Places a 2D pattern of tiles.
---Use `nil` to skip a tile.
---@param x integer Top-left tile x
---@param y integer Top-left tile y
---@param pattern table A 2D array of {set_x, set_y} or nil
---@param set string Tileset name
function monoe.tilemap:stamp(x, y, pattern, set)
  for py, row in ipairs(pattern) do
    for px, tile in ipairs(row) do
      if tile then
        self:place(x + px - 1, y + py - 1, set, tile[1], tile[2])
      end
    end
  end
end

---Places a random tile from a list of choices.
---@param x integer Tile x
---@param y integer Tile y
---@param set string Tileset name
---@param choices table Array of {set_x, set_y}
function monoe.tilemap:place_random(x, y, set, choices)
  local tile = choices[math.random(#choices)]
  self:place(x, y, set, tile[1], tile[2])
end

---Fills an area using random noise.
---@param x integer Starting tile x
---@param y integer Starting tile y
---@param w integer Width in tiles
---@param h integer Height in tiles
---@param set string Tileset name
---@param tiles table Array of {set_x, set_y}
---@param threshold number Probability threshold (0.0–1.0)
function monoe.tilemap:noise(x, y, w, h, set, tiles, threshold)
  threshold = threshold or 0.5

  for ty = y, y + h - 1 do
    for tx = x, x + w - 1 do
      if math.random() > threshold then
        local tile = tiles[math.random(#tiles)]
        self:place(tx, ty, set, tile[1], tile[2])
      end
    end
  end
end

_G.monoe = monoe
_G.monoe.tilemap = monoe.tilemap

return monoe.tilemap