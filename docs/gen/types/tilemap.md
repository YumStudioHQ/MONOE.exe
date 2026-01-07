# monoe.tilemap

Represents a 2D tilemap that can place tiles, fill areas, and draw patterns using tilesets.

## Properties

- **uid** (`integer`): Engine-side unique ID
- **tilesets** (`table<string`): , integer> Loaded tilesets by name
- **last** (`integer`): Last used ID or counter
- **zindex** (`integer`): The Z index of the tilemap. Won't be used if not used with monoe.layermap.

## monoe.tilemap.new

Creates a new `monoe.tilemap` object.

### Returns

- `monoe.tilemap Newly created tilemap`

---

## monoe.tilemap:place

Places a single tile at the specified tile coordinates.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `tile_x` | `integer` | X coordinate on the tilemap |
| `tile_y` | `integer` | Y coordinate on the tilemap |
| `tileset_name` | `string` | Name of the tileset |
| `tileset_x` | `integer` | X coordinate within the tileset |
| `tileset_y` | `integer` | Y coordinate within the tileset |

---

## monoe.tilemap:load

Loads a tileset into the tilemap.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Name of the tileset |
| `source` | `monoe.image|string` | Either a `monoe.image` or a file path |
| `tile_width` | `integer` | Tile width in pixels (default 16) |
| `tile_height` | `integer` | Tile height in pixels (default = `tile_width`) |

---

## monoe.tilemap:scale

Scales the entire tilemap.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Scale factor in X |
| `y` | `number` | Scale factor in Y |

---

## monoe.tilemap:rectangle

Draws a filled rectangle of tiles.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `integer` | Starting tile X |
| `y` | `integer` | Starting tile Y |
| `width` | `integer` | Width in tiles |
| `height` | `integer` | Height in tiles |
| `tileset` | `string` | Tileset name |
| `tileset_x` | `integer` | Tileset X coordinate |
| `tileset_y` | `integer` | Tileset Y coordinate |

---

## monoe.tilemap:fill

Fills a rectangular area with the same tile.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `integer` | Starting tile X |
| `y` | `integer` | Starting tile Y |
| `width` | `integer` | Width in tiles |
| `height` | `integer` | Height in tiles |
| `tileset` | `string` | Tileset name |
| `tileset_x` | `integer` | Tileset X coordinate |
| `tileset_y` | `integer` | Tileset Y coordinate |

---

## monoe.tilemap:line

Draws a line of tiles between two points.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x0` | `integer` | Start tile X |
| `y0` | `integer` | Start tile Y |
| `x1` | `integer` | End tile X |
| `y1` | `integer` | End tile Y |
| `tileset` | `string` | Tileset name |
| `tileset_x` | `integer` | Tileset X coordinate |
| `tileset_y` | `integer` | Tileset Y coordinate |

---

## monoe.tilemap:border

Draws a rectangular border of tiles.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `integer` | Starting tile X |
| `y` | `integer` | Starting tile Y |
| `width` | `integer` | Width in tiles |
| `height` | `integer` | Height in tiles |
| `tileset` | `string` | Tileset name |
| `tileset_x` | `integer` | Tileset X coordinate |
| `tileset_y` | `integer` | Tileset Y coordinate |

---

## monoe.tilemap:stamp

Stamps a 2D pattern of tiles onto the map.
Use `nil` in the pattern to skip a tile.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `integer` | Top-left tile X |
| `y` | `integer` | Top-left tile Y |
| `pattern` | `table` | 2D array of {tileset_x, tileset_y} or nil |
| `tileset` | `string` | Tileset name |

---

## monoe.tilemap:place_random

Places a random tile from a list of choices.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `integer` | Tile X |
| `y` | `integer` | Tile Y |
| `tileset` | `string` | Tileset name |
| `choices` | `table` | Array of {tileset_x, tileset_y} |

---

## monoe.tilemap:noise

Fills an area using random tiles based on a probability threshold.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `integer` | Starting tile X |
| `y` | `integer` | Starting tile Y |
| `width` | `integer` | Width in tiles |
| `height` | `integer` | Height in tiles |
| `tileset` | `string` | Tileset name |
| `tiles` | `table` | Array of {tileset_x, tileset_y} |
| `threshold` | `number` | Probability (0–1) for a tile to be placed (default 0.5) |

---

## monoe.tilemap:free

Frees engine resources associated with this tilemap.

---

## monoe.tilemap:remove

Removes the instance from the rendering side.

---

