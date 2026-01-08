# monoe.node

This is a simple node, that can allow you to structurate your scenes.

## Properties

- **uid** (`integer`): 

## monoe.node.new

Creates a new node.

### Returns

- `monoe.node`

---

## monoe.node:attach

Attaches an object to the node state.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `state` | `any` |  |

---

## monoe.node:move

Moves the set of objects.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Horizontal velocity |
| `y` | `number` | Vertical velocity |

---

## monoe.node:position

Gets or sets the position of the node in 2D space.
If `x` and `y` are provided, the node is moved to that position.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` | X-coordinate to set (optional) |
| `y` | `number|nil` | Y-coordinate to set (optional) |

### Returns

- `number current_x The current or new X-coordinate`
- `number current_y The current or new Y-coordinate`

---

## monoe.node:scale

Scales the node in X and Y directions.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Scale factor along the X-axis |
| `y` | `number` | Scale factor along the Y-axis |

### Returns

- `number new_x The resulting scale along X-axis`
- `number new_y The resulting scale along Y-axis`

---

## monoe.node:free

Releases engine resources used by this node.
After calling this, the node should no longer be used.

---

## monoe.node:remove

Removes the instance from the rendering side.

---

