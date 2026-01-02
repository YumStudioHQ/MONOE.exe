# monoe.shape



## Properties

- **uid** (`integer`): Unique ID for the engine-side shape object

## monoe.shape.new

Creates a new shape object for collisions.
Supports predefined types: `"rectangle:WIDTHxHEIGHT"`, `"circle:RADIUS"`, `"capsule:RADIUSxHEIGHT"`.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `shape` | `string` | Shape description |

### Returns

- `monoe.shape`

---

## monoe.shape:debug

Sets a debug outline color for the shape.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `hex` | `integer` | Color in 0xRRGGBBAA format |

---

