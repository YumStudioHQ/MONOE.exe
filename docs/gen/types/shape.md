# shape.lua

Source: `libraries/types/shape.lua`

## monoe.shape.new

@class monoe.shape
@field uid integer Unique ID for the engine-side shape object
Creates a new shape object for collisions.
Supports predefined types: `"rectangle:WIDTHxHEIGHT"`, `"circle:RADIUS"`, `"capsule:RADIUSxHEIGHT"`.
@param shape string Shape description
@return monoe.shape

| Parameter | Type |
|-----------|------|
| `shape` | string Shape description |

**Returns:** monoe.shape

---

## monoe.shape:debug

@param hex integer Color in 0xRRGGBBAA format

| Parameter | Type |
|-----------|------|
| `hex` | integer Color in 0xRRGGBBAA format |

---

