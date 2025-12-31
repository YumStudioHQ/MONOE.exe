# tilemap.lua

Source: `libraries/types/tilemap.lua`

## monoe.tilemap.new

@class monoe.tilemap
@field uid integer Engine-side unique ID
@field tilesets table<string, integer> Loaded tilesets by name
@field last integer Last used ID or counter
Represents a 2D tilemap that can place tiles, fill areas, and draw patterns using tilesets.
Creates a new `monoe.tilemap` object.
@return monoe.tilemap Newly created tilemap

**Returns:** monoe.tilemap Newly created tilemap

---

## monoe.tilemap:place

@param tile_x integer X coordinate on the tilemap
@param tile_y integer Y coordinate on the tilemap
@param tileset_name string Name of the tileset
@param tileset_x integer X coordinate within the tileset
@param tileset_y integer Y coordinate within the tileset

| Parameter | Type |
|-----------|------|
| `tile_x` | integer X coordinate on the tilemap |
| `tile_y` | integer Y coordinate on the tilemap |
| `tileset_name` | string Name of the tileset |
| `tileset_x` | integer X coordinate within the tileset |
| `tileset_y` | integer Y coordinate within the tileset |

---

## monoe.tilemap:load

@param name string Name of the tileset
@param source monoe.image|string Either a `monoe.image` or a file path
@param tile_width integer Tile width in pixels (default 16)
@param tile_height integer Tile height in pixels (default = `tile_width`)

| Parameter | Type |
|-----------|------|
| `name` | string Name of the tileset |
| `source` | monoe.image|string Either a `monoe.image` or a file path |
| `tile_width` | integer Tile width in pixels (default 16) |
| `tile_height` | integer Tile height in pixels (default = `tile_width`) |

---

## monoe.tilemap:scale

@param x number Scale factor in X
@param y number Scale factor in Y

| Parameter | Type |
|-----------|------|
| `x` | number Scale factor in X |
| `y` | number Scale factor in Y |

---

## monoe.tilemap:rectangle

@param x integer Starting tile X
@param y integer Starting tile Y
@param width integer Width in tiles
@param height integer Height in tiles
@param tileset string Tileset name
@param tileset_x integer Tileset X coordinate
@param tileset_y integer Tileset Y coordinate

| Parameter | Type |
|-----------|------|
| `x` | integer Starting tile X |
| `y` | integer Starting tile Y |
| `width` | integer Width in tiles |
| `height` | integer Height in tiles |
| `tileset` | string Tileset name |
| `tileset_x` | integer Tileset X coordinate |
| `tileset_y` | integer Tileset Y coordinate |

---

## monoe.tilemap:fill

@param x integer Starting tile X
@param y integer Starting tile Y
@param width integer Width in tiles
@param height integer Height in tiles
@param tileset string Tileset name
@param tileset_x integer Tileset X coordinate
@param tileset_y integer Tileset Y coordinate

| Parameter | Type |
|-----------|------|
| `x` | integer Starting tile X |
| `y` | integer Starting tile Y |
| `width` | integer Width in tiles |
| `height` | integer Height in tiles |
| `tileset` | string Tileset name |
| `tileset_x` | integer Tileset X coordinate |
| `tileset_y` | integer Tileset Y coordinate |

---

## monoe.tilemap:line

@param x0 integer Start tile X
@param y0 integer Start tile Y
@param x1 integer End tile X
@param y1 integer End tile Y
@param tileset string Tileset name
@param tileset_x integer Tileset X coordinate
@param tileset_y integer Tileset Y coordinate

| Parameter | Type |
|-----------|------|
| `x0` | integer Start tile X |
| `y0` | integer Start tile Y |
| `x1` | integer End tile X |
| `y1` | integer End tile Y |
| `tileset` | string Tileset name |
| `tileset_x` | integer Tileset X coordinate |
| `tileset_y` | integer Tileset Y coordinate |

---

## monoe.tilemap:border

@param x integer Starting tile X
@param y integer Starting tile Y
@param width integer Width in tiles
@param height integer Height in tiles
@param tileset string Tileset name
@param tileset_x integer Tileset X coordinate
@param tileset_y integer Tileset Y coordinate

| Parameter | Type |
|-----------|------|
| `x` | integer Starting tile X |
| `y` | integer Starting tile Y |
| `width` | integer Width in tiles |
| `height` | integer Height in tiles |
| `tileset` | string Tileset name |
| `tileset_x` | integer Tileset X coordinate |
| `tileset_y` | integer Tileset Y coordinate |

---

## monoe.tilemap:stamp

@param x integer Top-left tile X
@param y integer Top-left tile Y
@param pattern table 2D array of {tileset_x, tileset_y} or nil
@param tileset string Tileset name

| Parameter | Type |
|-----------|------|
| `x` | integer Top-left tile X |
| `y` | integer Top-left tile Y |
| `pattern` | table 2D array of {tileset_x, tileset_y} or nil |
| `tileset` | string Tileset name |

---

## monoe.tilemap:place_random

@param x integer Tile X
@param y integer Tile Y
@param tileset string Tileset name
@param choices table Array of {tileset_x, tileset_y}

| Parameter | Type |
|-----------|------|
| `x` | integer Tile X |
| `y` | integer Tile Y |
| `tileset` | string Tileset name |
| `choices` | table Array of {tileset_x, tileset_y} |

---

## monoe.tilemap:noise

@param x integer Starting tile X
@param y integer Starting tile Y
@param width integer Width in tiles
@param height integer Height in tiles
@param tileset string Tileset name
@param tiles table Array of {tileset_x, tileset_y}
@param threshold number Probability (0–1) for a tile to be placed (default 0.5)

| Parameter | Type |
|-----------|------|
| `x` | integer Starting tile X |
| `y` | integer Starting tile Y |
| `width` | integer Width in tiles |
| `height` | integer Height in tiles |
| `tileset` | string Tileset name |
| `tiles` | table Array of {tileset_x, tileset_y} |
| `threshold` | number Probability (0–1) for a tile to be placed (default 0.5) |

---

