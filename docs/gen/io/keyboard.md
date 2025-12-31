# keyboard.lua

Source: `libraries/io/keyboard.lua`

## monoe.io.keyboard.key_down

@class monoe.io.keyboard
Provides functions to query keyboard input and actions.
Checks if a physical key is currently pressed.
@param key string Key name (e.g., "space", "enter", "a")
@return boolean True if the key is pressed, false otherwise

| Parameter | Type |
|-----------|------|
| `key` | string Key name (e.g., "space", "enter", "a") |

**Returns:** boolean True if the key is pressed, false otherwise

---

## monoe.io.keyboard.down

@param action string Action name (as defined in the input map of Godot)
@note ui_down, ui_up, ui_right and ui_left are true when using ZSQD, WSDA, ...
@return boolean True if the action is pressed, false otherwise

| Parameter | Type |
|-----------|------|
| `action` | string Action name (as defined in the input map of Godot) |

**Returns:** boolean True if the action is pressed, false otherwise

---

## monoe.io.keyboard.just_down

@param action string Action name
@return boolean True if the action was just pressed, false otherwise

| Parameter | Type |
|-----------|------|
| `action` | string Action name |

**Returns:** boolean True if the action was just pressed, false otherwise

---

## monoe.io.keyboard.released

@param action string Action name
@return boolean True if the action was just released, false otherwise

| Parameter | Type |
|-----------|------|
| `action` | string Action name |

**Returns:** boolean True if the action was just released, false otherwise

---

