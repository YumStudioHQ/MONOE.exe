# window.lua

Source: `libraries/io/window.lua`

## monoe.engine.window.title

@diagnostic disable: return-type-mismatch, missing-return-value
@class monoe.engine.window
Provides functions to manipulate the main engine window.
Changes the title of the main window.
@param title string New window title

| Parameter | Type |
|-----------|------|
| `title` | string New window title |

---

## monoe.engine.window.size

@param width number|nil New width in pixels
@param height number|nil New height in pixels
@return integer current_width
@return integer current_height

| Parameter | Type |
|-----------|------|
| `width` | number|nil New width in pixels |
| `height` | number|nil New height in pixels |

**Returns:** integer current_width, integer current_height

---

## monoe.engine.window.position

@param x number|nil New x position
@param y number|nil New y position
@return integer current_x
@return integer current_y

| Parameter | Type |
|-----------|------|
| `x` | number|nil New x position |
| `y` | number|nil New y position |

**Returns:** integer current_x, integer current_y

---

## monoe.engine.window.scale

@param x number Scale factor X
@param y number Scale factor Y
@return integer scaled_width
@return integer scaled_height

| Parameter | Type |
|-----------|------|
| `x` | number Scale factor X |
| `y` | number Scale factor Y |

**Returns:** integer scaled_width, integer scaled_height

---

## monoe.engine.window.move

@param dx number Horizontal offset
@param dy number Vertical offset
@return integer new_x
@return integer new_y

| Parameter | Type |
|-----------|------|
| `dx` | number Horizontal offset |
| `dy` | number Vertical offset |

**Returns:** integer new_x, integer new_y

---

## monoe.engine.window.attach

@param obj table Object with `.uid` or `.root` property

| Parameter | Type |
|-----------|------|
| `obj` | table Object with `.uid` or `.root` property |

---

## monoe.engine.window.center

@return number center_x
@return number center_y

**Returns:** number center_x, number center_y

---

