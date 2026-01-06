# monoe.event



## Properties

- **_once** (`table<string`): , function[]> Listeners that trigger only once
- **_listeners** (`table<string`): , function[]> Persistent listeners

## monoe.event.once

Subscribes a function to an event that will trigger only once.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | The event name. |
| `fn` | `function` | The callback function to execute when the event is emitted. |

---

## monoe.event.subscribe

Subscribes a function to an event that triggers every time the event is emitted.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | The event name. |
| `fn` | `function` | The callback function to execute. |

---

## call



### Parameters

| Name | Type | Description |
|------|------|-------------|
| `eventName` | `string` |  |
| `fn` | `function` |  |

---

## monoe.event.unsubscribe

Unsubscribes a function from a persistent event.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | The event name. |
| `fn` | `function` | The callback function to remove. |

---

## monoe.event.emit

Emits an event, calling all subscribed functions.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `name` | `string` | The event name. |

---

