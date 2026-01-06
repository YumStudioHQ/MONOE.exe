# monoe



## monoe.import

Imports a class by name.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `class` | `string` | Name of the class to import. |

### Returns

- `integer uid Returns the unique ID of the created instance or -1 on failure.`

---

## monoe.call

Calls a method on an instance.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `uid` | `integer` | The unique ID of the object. |
| `method` | `string` | The method name to call. |

### Returns

- `any[] The return values from the method call.`

---

## monoe.staticcall

Calls a static method on a static class.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `base` | `string` | Full name of the static base class. |
| `method` | `string` | Method name to call. |

### Returns

- `any The return value of the static method.`

---

## monoe.shell

Opens a shell or executes commands.

---

## fullpath

Environment settings for monoe.
Returns the full path of a file.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` | Relative path. |

### Returns

- `string Absolute path.`

---

## subscribe_all



---

## deep_update



---

## monoe.load

Loads a Lua module and optionally enables hot reloading.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | Name to assign in _G. |

### Returns

- `table The loaded module.`

---

## monoe.breakpoint

Triggers a breakpoint in Lua.

---

## monoe.wait

Pauses execution for a specified number of milliseconds.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `milliseconds` | `integer` |  |

---

## _attach

Attaches an object or its children to the window for rendering.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `obj` | `table` | Object with `.uid` or `.root` property |

---

## monoe.qualify

Subscribes all known methods of a table to events.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `table` | `table` |  |

---

