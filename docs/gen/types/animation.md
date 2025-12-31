# animation.lua

Source: `libraries/types/animation.lua`

## monoe.animation.new

@diagnostic disable
@class monoe.animation
@field uid integer Unique ID for the engine-side animation object
Creates a new `monoe.animation` object.
@return monoe_animation

**Returns:** monoe_animation

---

## monoe.animation:add

@param name string Name of the animation

| Parameter | Type |
|-----------|------|
| `name` | string Name of the animation |

---

## monoe.animation:addframe

@param name string Name of the animation
@param frame monoe.image The frame image to add
@param duration number|nil Duration of the frame in seconds (default 1.0)
@param index integer|nil Index at which to insert the frame (-1 = append)

| Parameter | Type |
|-----------|------|
| `name` | string Name of the animation |
| `frame` | monoe.image The frame image to add |
| `duration` | number|nil Duration of the frame in seconds (default 1.0) |
| `index` | integer|nil Index at which to insert the frame (-1 = append) |

---

## monoe.animation:play

@param name string Animation name

| Parameter | Type |
|-----------|------|
| `name` | string Animation name |

---

## monoe.animation:backwards

@param name string Animation name

| Parameter | Type |
|-----------|------|
| `name` | string Animation name |

---

## monoe.animation:pause

---

## monoe.animation:animations

@return string[]

**Returns:** string[]

---

## monoe.animation:position

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

## monoe.animation:move

@param x number
@param y number
@return number newX
@return number newY

| Parameter | Type |
|-----------|------|
| `x` | number |
| `y` | number |

**Returns:** number newX, number newY

---

## monoe.animation:scale

@param x number
@param y number
@return number newX
@return number newY

| Parameter | Type |
|-----------|------|
| `x` | number |
| `y` | number |

**Returns:** number newX, number newY

---

## monoe.animation:flip

@param axe 'V'|'H' Vertical or Horizontal
@param state boolean True to flip, false to reset

| Parameter | Type |
|-----------|------|
| `axe` | 'V'|'H' Vertical or Horizontal |
| `state` | boolean True to flip, false to reset |

---

## monoe.animation:load

@param name string Animation name
@param image monoe.image Source image
@param width integer Frame width
@param height integer Frame height
@param fromcolumn integer Starting column
@param tocolumn integer Ending column
@param fromrow integer Starting row
@param torow integer Ending row
@param fps number|nil Frames per second (default 7.0)

| Parameter | Type |
|-----------|------|
| `name` | string Animation name |
| `image` | monoe.image Source image |
| `width` | integer Frame width |
| `height` | integer Frame height |
| `fromcolumn` | integer Starting column |
| `tocolumn` | integer Ending column |
| `fromrow` | integer Starting row |
| `torow` | integer Ending row |
| `fps` | number|nil Frames per second (default 7.0) |

---

## monoe.animation:free

---

