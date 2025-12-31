# sprite.lua

Source: `libraries/types/sprite.lua`

## monoe.sprite.new

@diagnostic disable: return-type-mismatch, missing-return-value
@class monoe.sprite
@field uid integer Unique ID for the engine-side sprite object
Creates a new `monoe.sprite` object.
If a path is provided, it will load it as an image.
@param path string|nil File path to load as the sprite image
@return monoe.sprite

| Parameter | Type |
|-----------|------|
| `path` | string|nil File path to load as the sprite image |

**Returns:** monoe.sprite

---

## monoe.sprite:clear

---

## monoe.sprite:load

@param path string File path of the image

| Parameter | Type |
|-----------|------|
| `path` | string File path of the image |

---

## monoe.sprite:position

@param x number|nil
@param y number|nil
@return number x
@return number y

| Parameter | Type |
|-----------|------|
| `x` | number|nil |
| `y` | number|nil |

**Returns:** number x, number y

---

## monoe.sprite:move

@param x number
@param y number
@return number new_x
@return number new_y

| Parameter | Type |
|-----------|------|
| `x` | number |
| `y` | number |

**Returns:** number new_x, number new_y

---

## monoe.sprite:scale

@param x number
@param y number
@return number new_x
@return number new_y

| Parameter | Type |
|-----------|------|
| `x` | number |
| `y` | number |

**Returns:** number new_x, number new_y

---

## monoe.sprite:image

@return monoe.image

**Returns:** monoe.image

---

## monoe.sprite:free

---

