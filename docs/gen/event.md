# event.lua

Source: `libraries/event.lua`

## monoe.event.once

@class monoe_event
@field _once table<string, function[]> Listeners that trigger only once
@field _listeners table<string, function[]> Persistent listeners
Subscribes a function to an event that will trigger only once.
@param name string The event name.
@param fn function The callback function to execute when the event is emitted.

| Parameter | Type |
|-----------|------|
| `name` | string The event name. |
| `fn` | function The callback function to execute when the event is emitted. |

---

## monoe.event.subscribe

@param name string The event name.
@param fn function The callback function to execute.

| Parameter | Type |
|-----------|------|
| `name` | string The event name. |
| `fn` | function The callback function to execute. |

---

## call

@param eventName string
@param fn function
@param ... any Arguments to pass to the listener

| Parameter | Type |
|-----------|------|
| `eventName` | string |
| `fn` | function |
| `...` | unknown |

---

## monoe.event.unsubscribe

@param name string The event name.
@param fn function The callback function to remove.

| Parameter | Type |
|-----------|------|
| `name` | string The event name. |
| `fn` | function The callback function to remove. |

---

## monoe.event.emit

@param name string The event name.
@param ... any Additional arguments to pass to the listeners.

| Parameter | Type |
|-----------|------|
| `name` | string The event name. |
| `...` | unknown |

---

