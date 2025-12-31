# image.lua

Source: `libraries/types/image.lua`

## monoe.image.new

@class monoe.image
@field uid integer Unique ID for the engine-side image object
Represents an image that can be loaded, cleared, and managed in the engine.
Creates a new `monoe.image` object.
If a `path` is provided, the image will be loaded from that file.
@param path string|integer|nil File path, existing UID, or nil for a new empty image
@return monoe.image Newly created image object

| Parameter | Type |
|-----------|------|
| `path` | string|integer|nil File path, existing UID, or nil for a new empty image |

**Returns:** monoe.image Newly created image object

---

## monoe.image:path

@return string Current file path of the image

**Returns:** string Current file path of the image

---

## monoe.image:load

@param path string File path to load

| Parameter | Type |
|-----------|------|
| `path` | string File path to load |

---

## monoe.image:clear

---

## monoe.image:free

---

