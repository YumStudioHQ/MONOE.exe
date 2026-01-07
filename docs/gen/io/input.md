# monoe.io

Provides functions to query keyboard input and actions.

## monoe.io.key_down

Checks if a physical key is currently pressed.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `key` | `string` | Key name (e.g., "space", "enter", "a") |

### Returns

- `boolean True if the key is pressed, false otherwise`

---

## monoe.io.down

Checks if an action is currently pressed.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` | Action name (as defined in the input map of Godot) |

### Returns

- `boolean True if the action is pressed, false otherwise`

> **Note:** ui_down, ui_up, ui_right and ui_left are true when using ZSQD, WSDA, ...

---

## monoe.io.just_down

Checks if an action was just pressed this frame.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` | Action name |

### Returns

- `boolean True if the action was just pressed, false otherwise`

---

## monoe.io.released

Checks if an action was released this frame.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` | Action name |

### Returns

- `boolean True if the action was just released, false otherwise`

---

## monoe.io.mouse

Returns the position of the mouse.

### Returns

- `integer, integer`

---

