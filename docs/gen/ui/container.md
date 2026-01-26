# monoe.container



## Properties

- **uid** (`integer`): 

## monoe.container.new

Creates a new container

### Returns

- `monoe.container`

---

## monoe.container:move

Moves the container

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `number`
- `number`

---

## monoe.container:position

Returns the position of the container, and places at its new position if x or y are given.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number`
- `number`

---

## monoe.container:size

Returns the size of the container, and places at its new size if x or y are given.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number`
- `number`

---

## monoe.container:scale

Returns the scale of the container, and places at its new scale if x or y are given.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number`
- `number`

---

## monoe.container:remove

Removes the container from its rendering server

---

## monoe.container:free

Frees the container (no more usable)

---

## monoe.container:pack

Attaches given object to the container

---

## monoe.container:attach

Attaches given object to the container

---

