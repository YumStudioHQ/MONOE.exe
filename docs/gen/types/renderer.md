# monoe.renderer

This class allows you to set a rendering order for your games, sorting with a zindex, if the field is present.

## Properties

- **uid** (`integer`): 
- **objects** (`table`): 

## monoe.renderer.new

Creates a new renderer.

### Returns

- `monoe.renderer`

---

## monoe.renderer:attach

Attaches an object to the renderer state.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `state` | `any` |  |

---

## monoe.renderer:expose

Sorts the internal table, and returns a renderable table (e.g., monoe.io.window)

### Returns

- `table`

> **Warning:** You MAY NOT call this function each frame, as it is heavy. Prefer exposing once, or, each new scenes.

---

