# monoe.color

represents an RGBA color (encoded on 4 bytes, RRGGBBAA)

## Properties

- **r** (`integer`): Red   (0–255)
- **g** (`integer`): Green (0–255)
- **b** (`integer`): Blue  (0–255)
- **a** (`integer`): Alpha (0–255)

## clamp8



---

## monoe.color.new

Creates a new color.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `r` | `integer|nil` |  |
| `g` | `integer|nil` |  |
| `b` | `integer|nil` |  |
| `a` | `integer|nil` | Alpha (default 255) |

### Returns

- `monoe.color`

---

## monoe.color.gray

Creates a color from a grayscale value.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `v` | `integer` |  |
| `a` | `integer|nil` |  |

### Returns

- `monoe.color`

---

## monoe.color.from_hex

Creates a color from a hex integer (0xRRGGBB or 0xRRGGBBAA).

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `hex` | `integer` |  |

### Returns

- `monoe.color`

---

## monoe.color.from_int

Creates a color from a packed RGBA32 integer.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `value` | `integer` | Unsigned 32-bit (0xRRGGBBAA) |

### Returns

- `monoe.color`

---

## monoe.color:to_int

Returns the color as a packed RGBA32 unsigned integer.

### Returns

- `integer`

---

## monoe.color:clone

Returns a copy of this color.

### Returns

- `monoe.color`

---

## monoe.color:__tostring

Returns a string representation.

---

## monoe.color:add

Adds another color to this one (channel-wise, clamped).

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `other` | `monoe.color` |  |

### Returns

- `monoe.color`

---

## monoe.color.__add



---

## monoe.color:shadow

Returns a shadow version of the color.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `factor` | `number` | Darkening factor (0–1), default 0.5 |

### Returns

- `monoe.color`

---

## monoe.color:luminosity

Returns perceived luminosity (0–255).

### Returns

- `integer`

---

## monoe.color:contrast

Returns black or white depending on contrast.

### Returns

- `monoe.color`

---

## monoe.color:palette

Generates a palette from this color.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `steps` | `integer` | Number of variations |

### Returns

- `monoe.color[]`

---

## monoe.color:string

Returns a Godot-friendly color string.
Either "#RRGGBB" if alpha is 255, else "#RRGGBBAA"

### Returns

- `string`

---

## byte_to_hex



---

## monoe.color.from_string

Creates a color from a string.
Supports "#RRGGBB", "#RRGGBBAA", "R,G,B" or "R,G,B,A"

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `str` | `string` |  |

### Returns

- `monoe.color`

---

