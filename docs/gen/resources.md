# monoe.resources

Provides useful functions that makes loading easier.

## is_loadable

Returns true if the resource can be loaded.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `T` | `any` |  |

### Returns

- `boolean`

---

## _load_res

Internal loader function.
Creates a cache entry if none exists.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `T` | `T` |  |
| `path` | `string` |  |

### Returns

- `monoe.resources.entry?`

---

## monoe.resources.preload

Preloads a resource and pins it in memory.
Pinned resources are not freed during collection.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `T` | `T` |  |
| `path` | `string` | Resource's path |

---

## monoe.resources.load

Loads a resource for temporary usage.
Unpinned resources are freed on the next collection.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `T` | `T` |  |
| `path` | `string` |  |

### Returns

- `T`

---

