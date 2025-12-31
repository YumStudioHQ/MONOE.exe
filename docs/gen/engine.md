# engine.lua

Source: `libraries/engine.lua`

## monoe.import

@class monoe
Imports a class by name.
@param class string Name of the class to import.
@return integer uid Returns the unique ID of the created instance or -1 on failure.

| Parameter | Type |
|-----------|------|
| `class` | string Name of the class to import. |

**Returns:** integer uid Returns the unique ID of the created instance or -1 on failure.

---

## monoe.call

@param uid integer The unique ID of the object.
@param method string The method name to call.
@param ... any Additional arguments to pass to the method.
@return any[] The return values from the method call.

| Parameter | Type |
|-----------|------|
| `uid` | integer The unique ID of the object. |
| `method` | string The method name to call. |
| `...` | unknown |

**Returns:** any[] The return values from the method call.

---

## monoe.staticcall

@param base string Full name of the static base class.
@param method string Method name to call.
@param ... any Arguments to pass to the method.
@return any The return value of the static method.

| Parameter | Type |
|-----------|------|
| `base` | string Full name of the static base class. |
| `method` | string Method name to call. |
| `...` | unknown |

**Returns:** any The return value of the static method.

---

## monoe.shell

@param ... any Arguments passed to the shell.

| Parameter | Type |
|-----------|------|
| `...` | unknown |

---

## fullpath

@param path string Relative path.
@return string Absolute path.

| Parameter | Type |
|-----------|------|
| `path` | string Relative path. |

**Returns:** string Absolute path.

---

## subscribe_all

| Parameter | Type |
|-----------|------|
| `table` | unknown |

---

## deep_update

| Parameter | Type |
|-----------|------|
| `old` | unknown |
| `new` | unknown |

---

## monoe.load

@param name string Name to assign in _G.
@param path? string Module path. Defaults to `name`.
@return table The loaded module.

| Parameter | Type |
|-----------|------|
| `name` | string Name to assign in _G. |
| `path` | unknown |

**Returns:** table The loaded module.

---

## monoe.breakpoint

---

## monoe.wait

@param milliseconds integer

| Parameter | Type |
|-----------|------|
| `milliseconds` | integer |

---

## monoe.qualify

@param table table

| Parameter | Type |
|-----------|------|
| `table` | table |

---

