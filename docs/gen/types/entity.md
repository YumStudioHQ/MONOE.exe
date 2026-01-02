# monoe.entity

Represents a 2D game entity that can hold sprites, animations, or other attached objects.

## Properties

- **uid** (`integer`): Unique engine-side identifier for this entity

## monoe.entity.new

Creates a new `monoe.entity` instance.
This entity can be positioned, scaled, moved, and have other objects attached.

### Returns

- `monoe.entity Newly created entity object`

---

## monoe.entity:position

Gets or sets the position of the entity in 2D space.
If `x` and `y` are provided, the entity is moved to that position.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` | X-coordinate to set (optional) |
| `y` | `number|nil` | Y-coordinate to set (optional) |

### Returns

- `number current_x The current or new X-coordinate`
- `number current_y The current or new Y-coordinate`

---

## monoe.entity:move

Moves the entity by applying a velocity.
Automatically calls `MoveAndSlide` to update the entity's position according to physics rules.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Horizontal velocity |
| `y` | `number` | Vertical velocity |

---

## monoe.entity:scale

Scales the entity in X and Y directions.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` | Scale factor along the X-axis |
| `y` | `number` | Scale factor along the Y-axis |

### Returns

- `number new_x The resulting scale along X-axis`
- `number new_y The resulting scale along Y-axis`

---

## monoe.entity:free

Releases engine resources used by this entity.
After calling this, the entity should no longer be used.

---

## monoe.entity:attach

Attaches another object (sprite, animation, or another entity) to this entity.
The attached object will move and scale with the entity.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `obj` | `monoe.image|monoe.sprite|monoe.animation|monoe.entity` | Object to attach |

---

