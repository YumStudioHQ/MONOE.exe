# monoe.system.threading

Allows multi-threading.

## Properties

- **uid** (`integer`): 
- **endsig** (`string`): 

## monoe.system.threading.new

Creates a new managed thread

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `source` | `string` | File path or lua code |
| `entry` | `string` | Main function's name |
| `finished` | `string` | Event name, the one that'll be called once the thread finished its task. |
| `libs` | `boolean` | True if the thread should open Lua's standard libraries. |

### Returns

- `monoe.system.threading`

---

## monoe.system.threading:start

Starts the thread with the given set of arguments.

---

## monoe.system.threading:terminate

Returns true if the thread is joined after this call.

### Returns

- `boolean True if it is joined after this call.`

---

## monoe.system.threading:finished



---

## monoe.system.threading:free



---

