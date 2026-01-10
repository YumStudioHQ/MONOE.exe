# monoe.stobj

Represents a static object that can move, scale, and be freed / removed / attached. Similar to monoe.entitys, this one if designed for objects.

## Properties

- **uid** (`integer`): 

## monoe.stobj.new



---

## monoe.stobj:position

Gets or sets the position of the stobj in 2D space.
If `x` and `y` are provided, the stobj is moved to that position.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` | X-coordinate to set (optional) |
| `y` | `number|nil` | Y-coordinate to set (optional) |

### Returns

- `number current_x The current or new X-coordinate`
- `number current_y The current or new Y-coordinate`

---

## monoe.stobj:move

Moves the stobj by applying a velocity.
Automatically calls `MoveAndSlide` to update the stobj's position according to physics rules.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Horizontal velocity |
| `y` | `number` | Vertical velocity |

---

## monoe.stobj:scale

Scales the stobj in X and Y directions.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Scale factor along the X-axis |
| `y` | `number` | Scale factor along the Y-axis |

### Returns

- `number new_x The resulting scale along X-axis`
- `number new_y The resulting scale along Y-axis`

---

## monoe.stobj:free

Releases engine resources used by this stobj.
After calling this, the stobj should no longer be used.

---

## monoe.stobj:remove

Removes the instance from the rendering side.

---

## monoe.stobj:attach

Attaches another object (sprite, animation, or another stobj) to this stobj.
The attached object will move and scale with the stobj.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `obj` | `monoe.image|monoe.sprite|monoe.animation|monoe.stobj` | Object to attach |

---

