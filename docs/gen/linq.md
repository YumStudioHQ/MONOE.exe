# monoe.query



## Properties

- **data** (`table`): 

## monoe.query.new

Builds a new query type

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `enum` | `any` |  |

### Returns

- `monoe.query`

---

## monoe.query:foreach

iterates all elements of the query

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `iterator` | `function` |  |

### Returns

- `monoe.query`

---

## monoe.query:where

selects elements when the predicator returns true.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `predicate` | `function` |  |

### Returns

- `monoe.query`

---

## monoe.query:select

maps a new query from the current query, releying on the provided mapper.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `mapper` | `function` |  |

### Returns

- `monoe.query`

---

## monoe.query:all

returns true when the predicator returns true for all elements.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `predicate` | `any` |  |

### Returns

- `boolean`

---

## monoe.query:any

returns true when the predicator returns true with at least one element.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `predicate` | `any` |  |

### Returns

- `boolean`

---

## monoe.query:take



### Parameters

| Name | Type | Description |
|------|------|-------------|
| `n` | `integer` |  |

### Returns

- `monoe.query`

---

## monoe.query:skip



### Parameters

| Name | Type | Description |
|------|------|-------------|
| `n` | `number` |  |

### Returns

- `monoe.query`

---

## monoe.query:distinct



### Returns

- `monoe.query`

---

## monoe.query:sum

sums all numeric values

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `mapper` | `function` |  |

### Returns

- `integer`

---

## monoe.query:max

returns the maximum value

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `mapper` | `function` |  |

### Returns

- `any`

---

## monoe.query:astable



---

## monoe.query.is_query

returns true if the object is a query instance.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `obj` | `any` |  |

### Returns

- `boolean`

---

## monoe.query.join

joins elements in a single query

### Returns

- `monoe.query`

---

