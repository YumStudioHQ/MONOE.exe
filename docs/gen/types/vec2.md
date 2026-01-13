# monoe.vec2



## Properties

- **x** (`number`): -- X component
- **y** (`number`): -- Y component

## monoe.vec2.new

Creates a new 2D vector.
If no values are provided, defaults to (0, 0).

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `monoe.vec2`

---

## monoe.vec2.zero

Returns a zero vector (0, 0).

### Returns

- `monoe.vec2`

---

## monoe.vec2:add

Adds another vector to this one.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `v` | `monoe.vec2` |  |

### Returns

- `monoe.vec2 result`

---

## monoe.vec2:sub

Subtracts another vector from this one.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `v` | `monoe.vec2` |  |

### Returns

- `monoe.vec2 result`

---

## monoe.vec2:mul

Multiplies this vector by a scalar.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `s` | `number` |  |

### Returns

- `monoe.vec2 result`

---

## monoe.vec2:div

Divides this vector by a scalar.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `s` | `number` |  |

### Returns

- `monoe.vec2 result`

---

## monoe.vec2:length_sq

Returns the squared length of the vector.
Useful when you want distance comparisons without sqrt().

### Returns

- `number`

---

## monoe.vec2:length

Returns the length (magnitude) of the vector.

### Returns

- `number`

---

## monoe.vec2:distance

Returns the distance between this vector and another one.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `v` | `monoe.vec2` |  |

### Returns

- `number`

---

## monoe.vec2:normalized

Returns a normalized (unit) vector and the original length.
If the vector is zero, returns (0,0) and length 0.

### Returns

- `monoe.vec2 normalized`
- `number length`

---

## monoe.vec2:dot

Returns the dot product between this vector and another.
Useful for angles, projections, and direction checks.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `v` | `monoe.vec2` |  |

### Returns

- `number`

---

## monoe.vec2:lerp

Linearly interpolates between this vector and another.
t = 0 → this vector
t = 1 → target vector

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `v` | `monoe.vec2` |  |
| `t` | `number` |  |

### Returns

- `monoe.vec2`

---

## monoe.vec2:clamp_length

Clamps the vector length so it does not exceed max_len.
Keeps direction the same.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `max_len` | `number` |  |

### Returns

- `monoe.vec2`

---

## monoe.vec2:unpack

Returns x and y as separate values.
Useful for APIs that expect unpacked numbers.

### Returns

- `number x`
- `number y`

---

## monoe.vec2:set

Sets the vector components directly.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `self`

---

## monoe.vec2:inside_aabb

Checks if this vector (point) is inside an axis-aligned bounding box.
`min` is bottom-left, `max` is top-right.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `min` | `monoe.vec2` |  |
| `max` | `monoe.vec2` |  |

### Returns

- `boolean`

---

