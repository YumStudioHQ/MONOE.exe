# monoe.path

Provides utilty functions for basic io operations.

## monoe.path.fullpath

Returns the full path of a file or directory

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

### Returns

- `string`

---

## monoe.path.copy

Copies a file or directory

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `isdir` | `boolean` | True if copying a directory |
| `src` | `string` | Source path |
| `dst` | `string` | Destination path |

---

## monoe.path.content

Returns all files and directories in a path

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

### Returns

- `string[]`

---

## monoe.path.parent

Returns the parent directory of a path

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

### Returns

- `string`

---

## monoe.path.create

Creates a directory

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

---

## monoe.path.isfile

Checks if a path is a file

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

### Returns

- `boolean`

---

## monoe.path.exist

Checks if a path exists

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

### Returns

- `boolean`

---

## monoe.path.filename

Returns the file's name

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` |  |

### Returns

- `string`

---

