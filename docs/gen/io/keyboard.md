# monoe.io.keyboard

Provides some helpful functions that allows you to use keyboard in the game.

## monoe.io.keyboard.key_down

Returns true if a physical key is currently pressed.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `key` | `string` | Key name (e.g. "a", "space", "enter", "esc") |

### Returns

- `boolean`

---

## monoe.io.keyboard.shift

Returns true if Shift is pressed.

### Returns

- `boolean`

---

## monoe.io.keyboard.ctrl

Returns true if Ctrl is pressed.

### Returns

- `boolean`

---

## monoe.io.keyboard.alt

Returns true if Alt is pressed.

### Returns

- `boolean`

---

## monoe.io.keyboard.action_down

Returns true if an action is currently pressed.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` |  |

### Returns

- `boolean`

---

## monoe.io.keyboard.action_just_down

Returns true if an action was just pressed this frame.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` |  |

### Returns

- `boolean`

---

## monoe.io.keyboard.action_just_up

Returns true if an action was just released this frame.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` |  |

### Returns

- `boolean`

---

## monoe.io.keyboard.action_strength

Returns the strength of an action (0.0 → 1.0).

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `action` | `string` |  |

### Returns

- `number`

---

