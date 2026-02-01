# monoe.event

Built-in events, those that are fired by the engine:
* @collect: Called after each physic process, this one is designed for cleaning up some resources.
* @ready: Called once the engine is ready.
* @process: Called each frames.
* @physics: Called at a fixed point, for physic updates. You may move entities here (entity.move() function should be called from here)
* @input: Called when any input interaction happens.
* @onexit: Called when an exit has been requested and not rejected (at this state, you cannot prevent exiting)
* @hot: Internal specific, prefer not using it. (Fired when a file changes)
* @load: Do not use this one. (please)

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

