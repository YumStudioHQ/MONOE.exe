# monoe.image

Represents an image that can be loaded, cleared, and managed in the engine.

## Properties

- **uid** (`integer`): Unique ID for the engine-side image object

## monoe.image.new

Creates a new `monoe.image` object.
If a `path` is provided, the image will be loaded from that file.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string|integer|nil` | File path, existing UID, or nil for a new empty image |

### Returns

- `monoe.image Newly created image object`

---

## monoe.image:path

Returns the file path of the image.

### Returns

- `string current file path of the image`

---

## monoe.image:load

Loads an image from a file path.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` | File path to load |

---

## monoe.image:clear

Clears the image content, resetting it to empty.

---

## monoe.image:replace

replaces the color a by the color b.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `a` | `string|monoe.color` |  |
| `b` | `string|monoe.color` |  |

---

## monoe.image:free

Frees the engine-side resources associated with this image.

---

