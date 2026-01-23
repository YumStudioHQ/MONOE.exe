# monoe.text



## Properties

- **uid** (`integer`): 

## monoe.text.new

Creates a new text label

### Returns

- `monoe.text`

---

## monoe.text:move

Moves the label

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number` |  |
| `y` | `number` |  |

### Returns

- `number`
- `number`

---

## monoe.text:size

Resizes the label, and returns its new size. If both x and y arguments are nil, it simply returns the size.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number`
- `number`

---

## monoe.text:position

Repositions the label, and returns its new position. If both x and y arguments are nil, it simply returns the position.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `x` | `number|nil` |  |
| `y` | `number|nil` |  |

### Returns

- `number`
- `number`

---

## monoe.text:text

Sets the text of the label

---

## monoe.text:get

Returns the text of the label

### Returns

- `string`

---

## monoe.text:font

Sets the font

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string|nil|` | '!' |
| `size` | `integer|nil` |  |

---

## monoe.text:color

Sets the font color

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `r` | `number` | Red |
| `g` | `number` | Green |
| `b` | `number` | Blue |
| `a` | `number|nil` | Alpha |

---

## monoe.text:free

Frees the resources (no longer usable)

---

## monoe.text:remove

Removes the object from its rendering server

---

