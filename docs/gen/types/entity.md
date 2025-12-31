# entity.lua

Source: `libraries/types/entity.lua`

## monoe.entity.new

@diagnostic disable: missing-return-value, return-type-mismatch
@class monoe.entity
@field uid integer Unique engine-side identifier for this entity
Represents a 2D game entity that can hold sprites, animations, or other attached objects.
Creates a new `monoe.entity` instance.
This entity can be positioned, scaled, moved, and have other objects attached.
@return monoe.entity Newly created entity object

**Returns:** monoe.entity Newly created entity object

---

## monoe.entity:position

@param x number|nil X-coordinate to set (optional)
@param y number|nil Y-coordinate to set (optional)
@return number current_x The current or new X-coordinate
@return number current_y The current or new Y-coordinate

| Parameter | Type |
|-----------|------|
| `x` | number|nil X-coordinate to set (optional) |
| `y` | number|nil Y-coordinate to set (optional) |

**Returns:** number current_x The current or new X-coordinate, number current_y The current or new Y-coordinate

---

## monoe.entity:move

@param x number Horizontal velocity
@param y number Vertical velocity

| Parameter | Type |
|-----------|------|
| `x` | number Horizontal velocity |
| `y` | number Vertical velocity |

---

## monoe.entity:scale

@param x number Scale factor along the X-axis
@param y number Scale factor along the Y-axis
@return number new_x The resulting scale along X-axis
@return number new_y The resulting scale along Y-axis

| Parameter | Type |
|-----------|------|
| `x` | number Scale factor along the X-axis |
| `y` | number Scale factor along the Y-axis |

**Returns:** number new_x The resulting scale along X-axis, number new_y The resulting scale along Y-axis

---

## monoe.entity:free

---

## monoe.entity:attach

@param obj monoe.image|monoe.sprite|monoe.animation|monoe.entity Object to attach

| Parameter | Type |
|-----------|------|
| `obj` | monoe.image|monoe.sprite|monoe.animation|monoe.entity Object to attach |

---

