# monoe.sprite



## Properties

- **uid** (`integer`): Unique ID for the engine-side sprite object

## monoe.sprite.new

Creates a new `monoe.sprite` object.
If a path is provided, it will load it as an image.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string|nil` | File path to load as the sprite image |

### Returns

- `monoe.sprite`

---

## monoe.sprite:clear

Clears the sprite image.

---

## monoe.sprite:load

Loads an image into the sprite.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` | File path of the image |

---

## monoe.sprite:position

Sets or gets the position of the sprite.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number x`
- `number y`

---

## monoe.sprite:move

Moves the sprite by the specified offset.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `number new_x`
- `number new_y`

---

## monoe.sprite:scale

Scales the sprite.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `number new_x`
- `number new_y`

---

## monoe.sprite:image

Returns the sprite's image object.

### Returns

- `monoe.image`

---

## monoe.sprite:free

Frees engine resources associated with this sprite.

---

