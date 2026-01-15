# monoe.animation



## Properties

- **uid** (`integer`): Unique ID for the engine-side animation object
- **zindex** (`integer`): 

## monoe.animation.new

Creates a new `monoe.animation` object.

### Returns

- `monoe.animation`

---

## monoe.animation:add

Adds a new animation by name.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Name of the animation |

---

## monoe.animation:addframe

Adds a frame to an animation.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Name of the animation |
| `frame` | `monoe.image` | The frame image to add |
| `duration` | `number|nil` | Duration of the frame in seconds (default 1.0) |
| `index` | `integer|nil` | Index at which to insert the frame (-1 = append) |

---

## monoe.animation:play

Plays the specified animation.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Animation name |
| `loop` | `boolean` | If the animation should loop or not |

---

## monoe.animation:backwards

Plays an animation backwards.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Animation name |
| `loop` | `boolean` | If the animation should loop or not |

---

## monoe.animation:pause

Pauses the current animation.

---

## monoe.animation:animations

Returns a list of all animation names.

### Returns

- `string[]`

---

## monoe.animation:position

Sets or gets the position of the animation.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number x`
- `number y`

---

## monoe.animation:move

Moves the animation by a given offset.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `number newX`
- `number newY`

---

## monoe.animation:scale

Scales the animation.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `number newX`
- `number newY`

---

## monoe.animation:flip

Flips the animation on a given axis.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `state` | `boolean` | True to flip, false to reset |

---

## monoe.animation:load

Loads an animation from a sprite sheet image.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Animation name |
| `image` | `monoe.image` | Source image |
| `width` | `integer` | Frame width |
| `height` | `integer` | Frame height |
| `fromcolumn` | `integer` | Starting column |
| `tocolumn` | `integer` | Ending column |
| `fromrow` | `integer` | Starting row |
| `torow` | `integer` | Ending row |
| `fps` | `number|nil` | Frames per second (default 7.0) |

---

## monoe.animation:current

Returns the current played animation.

### Returns

- `string`

---

## monoe.animation:finished

Calls the given function when an animation finishes.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `func` | `function` |  |
| `isonce` | `boolean` |  |

---

## monoe.animation:free

Frees engine resources associated with this animation.

---

## monoe.animation:remove

Removes the instance from the rendering side.

---

