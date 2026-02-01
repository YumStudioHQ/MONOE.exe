# monoe.csbuff

This is an interface that helps using C# specific monoe.exe.Core.Bridge.Types.Internals.LazyReadonlyBuffer type.

## Properties

- **uid** (`integer`): 

## monoe.csbuff.new

Creates a new interface in order to use a C# LazyBuffer in Lua. You generally may not use this class by yourself, unless building a specific C# API, as the monoe C# API tries to avoid using this type in end users code.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `uid` | `integer` |  |

### Returns

- `monoe.csbuff`

---

## monoe.csbuff:get

Returns the element at the given index (1-based indexes)

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `index` | `integer` |  |

### Returns

- `any`

---

## monoe.csbuff:size

Returns the size of the C# buffer

### Returns

- `integer`

---

## monoe.csbuff:unpack

Returns an array, now editable

### Returns

- `any[]`

---

## monoe.csbuff:free

Frees the resource (you may free it after using it)

---

