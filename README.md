# MONOE.exe Game Engine

<p align="center">
  <img src="./icon.png" alt="icon">
</p>

### What is MONOE.exe?

**MONOE.exe** is a small **meta-engine** designed for experimentation, learning, and flexible game development.

It combines multiple languages:

* **C / C++** — core native libraries (via YumEngine)
* **C#** — engine logic and Godot integration
* **Lua** — main scripting language

Lua scripts can interact with C# and native libraries at runtime.

---

### Why MONOE.exe?

Most engines force you into a fixed workflow.

MONOE.exe does the opposite.

You can:

* Write fast gameplay logic in **Lua**
* Use **C#** for engine systems or tools
* Load **custom DLLs** at runtime
* Reload scripts while the game is running

There is no “one correct way” to use the engine.

---

### Philosophy

> **Code your game your way.**

MONOE.exe focuses on **freedom**, **runtime control**, and **experimentation**, instead of strict rules or heavy abstractions.

---

## Engine Lifecycle

MONOE.exe uses an **event-driven lifecycle**.

The main Lua script (`project.lua`) follows this order:

1. **`deps()`**

   * Called first
   * Return paths to C# DLLs to load
   * Only minimal Lua libraries are available here

2. **Libraries are loaded**

   * C# assemblies become visible to Lua

3. **`main()`**

   * Project initialization
   * Register events, load scripts, setup state

4. **`@load` event**

   * Used to load secondary Lua files
   * Supports hot-reload

5. **`ready` event**

   * Called once everything is loaded

6. **Runtime loop**

   * `process(delta)` — every frame
   * `physics(delta)` — physics update

7. **Exit**

   * `onexit` event
   * Cleanup logic

---

## Basic Lua Example

```lua
local event = require('libraries.event')

function deps()
  return {
    "./MyLibrary.dll"
  }
end

function main()
  print("Game started")
end

event.subscribe('process', function(delta)
  -- update!
end)

event.subscribe('physics', function(delta)
  -- update physics!
end)

event.subscribe('onexit', function(delta)
  -- clean up here
end)
```

---

## Hot Reloading

MONOE.exe supports **hot reloading** for Lua files.

* When a `.lua` file changes:

  * `@hot` event is emitted
  * Scripts can reload themselves
* If `project.lua` changes:

  * The engine reboots the project

This allows fast iteration without restarting the engine.

---

## Garbage Collector

Each 0.5 seconds, the engine fires the 'onfree' event that allows to clean up data later (e.g., dropped stuff).
Prefer using `monoe.event.once('onfree', function()end)` for this event, as subscribe calls the function every time the event is fired. On the other hand, once calls once.

---

## Runtime Shell

MONOE.exe includes a **runtime shell**.

You can:

* Execute Lua code at runtime
* Inspect globals
* Force reloads
* Trigger garbage collection

### Shell commands

* `$reload` — reload the project
* `$dump <table>` — print a Lua table
* `$gc` — force garbage collection
* `$stats` — show memory stats

---

## Error Handling

* Lua errors can trigger a **critical state**
* In critical state:

  * Game logic stops
  * Error handler is executed
* Hot reload clears the critical state

This prevents crashes and allows recovery.

---

## Final note

MONOE.exe is not meant to compete with large engines.

It is  a **sandbox**, and a **tool for experimentation**.

If you enjoy building engines, MONOE.exe is for you.

---

## See more !

* [meta-runtime](./docs/meta-runtime.md)

## Important 

everything in game/assets is **not** my art, but the one of: https://otterisk.itch.io !