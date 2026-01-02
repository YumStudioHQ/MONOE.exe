# monoe.engine.window

Provides functions to manipulate the main engine window.

## monoe.engine.window.title

Changes the title of the main window.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `title` | `string` | New window title |

---

## monoe.engine.window.size

Sets or queries the size of the main window.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `width` | `number|nil` | New width in pixels |
| `height` | `number|nil` | New height in pixels |

### Returns

- `integer current_width`
- `integer current_height`

---

## monoe.engine.window.position

Sets or queries the position of the main window.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` | New x position |
| `y` | `number|nil` | New y position |

### Returns

- `integer current_x`
- `integer current_y`

---

## monoe.engine.window.scale

Scales the window.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Scale factor X |
| `y` | `number` | Scale factor Y |

### Returns

- `integer scaled_width`
- `integer scaled_height`

---

## monoe.engine.window.move

Moves the window by a relative offset.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `dx` | `number` | Horizontal offset |
| `dy` | `number` | Vertical offset |

### Returns

- `integer new_x`
- `integer new_y`

---

## monoe.engine.window.attach

Attaches an object or its children to the window for rendering.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `obj` | `table` | Object with `.uid` or `.root` property |

---

## monoe.engine.window.center

Returns the center coordinates of the window.

### Returns

- `number center_x`
- `number center_y`

---

