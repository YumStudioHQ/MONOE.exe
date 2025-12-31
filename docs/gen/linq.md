# linq.lua

Source: `libraries/linq.lua`

## monoe.query.new

@class monoe.query
@field data table
Builds a new query type
@param enum any
@return monoe.query

| Parameter | Type |
|-----------|------|
| `enum` | any |

**Returns:** monoe.query

---

## monoe.query:foreach

@param iterator function
@return monoe.query

| Parameter | Type |
|-----------|------|
| `iterator` | function |

**Returns:** monoe.query

---

## monoe.query:where

@param predicate function
@return monoe.query

| Parameter | Type |
|-----------|------|
| `predicate` | function |

**Returns:** monoe.query

---

## monoe.query:select

@param mapper function
@return monoe.query

| Parameter | Type |
|-----------|------|
| `mapper` | function |

**Returns:** monoe.query

---

## monoe.query:all

@param predicate any
@return boolean

| Parameter | Type |
|-----------|------|
| `predicate` | any |

**Returns:** boolean

---

## monoe.query:any

@param predicate any
@return boolean

| Parameter | Type |
|-----------|------|
| `predicate` | any |

**Returns:** boolean

---

## monoe.query:take

@param n integer
@return monoe.query

| Parameter | Type |
|-----------|------|
| `n` | integer |

**Returns:** monoe.query

---

## monoe.query:skip

@param n number
@return monoe.query

| Parameter | Type |
|-----------|------|
| `n` | number |

**Returns:** monoe.query

---

## monoe.query:distinct

@return monoe.query

**Returns:** monoe.query

---

## monoe.query:sum

@param mapper function
@return integer

| Parameter | Type |
|-----------|------|
| `mapper` | function |

**Returns:** integer

---

## monoe.query:max

@param mapper function
@return any

| Parameter | Type |
|-----------|------|
| `mapper` | function |

**Returns:** any

---

## monoe.query:astable

---

## monoe.query.is_query

@param obj any
@return boolean

| Parameter | Type |
|-----------|------|
| `obj` | any |

**Returns:** boolean

---

## monoe.query.join

@param ... unknown
@return monoe.query

| Parameter | Type |
|-----------|------|
| `...` | unknown |

**Returns:** monoe.query

---

