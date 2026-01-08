# MONOE.exe

*A runtime-driven, multi-language sandbox engine*

<p align="center">
  <img src="./icon.png" alt="MONOE.exe Logo" width="120">
</p>

---

## What is MONOE.exe?

**MONOE.exe** is a **runtime-oriented game engine / sandbox** built on top of **Godot (C#)**, designed for developers who want **maximum control at runtime** rather than fixed editor workflows.

It combines:

* **Lua** for fast, reloadable gameplay logic
* **C#** for core systems and engine control
* **Native libraries (C/C++)** via dynamic loading

The result is an engine where **you decide how things work**, how they reload, how errors are handled, and how your runtime behaves.

> MONOE.exe is not trying to replace existing engines.
> It’s a **playground that can scale**, if *you* want it to.

---

## Philosophy

> **You control the runtime.**

MONOE.exe is built around these ideas:

* No forced architecture
* No fixed project structure
* No editor-locked workflow
* Runtime inspection, execution, and reloading as first-class features

It can be:

* a toy engine
* a scripting sandbox
* a prototyping tool
* or a serious runtime for a game or tool

That choice is **entirely yours**.

---

## Key Features

| Feature                      | Description                                     |
| ---------------------------- | ----------------------------------------------- |
| **Lua-driven runtime**       | Core gameplay logic written in Lua              |
| **C# engine core**           | Godot-based main loop, lifecycle, threading, IO |
| **Runtime hot reload**       | Reload Lua files without restarting the engine  |
| **Event-driven lifecycle**   | Clear, explicit runtime events                  |
| **Runtime shell**            | Execute Lua code, inspect state, reload project |
| **Dynamic DLL loading**      | Load C# assemblies at runtime from Lua          |
| **Main-thread queue**        | Safely enqueue actions from other threads       |
| **Fail-safe error handling** | Critical state locking + safe recovery          |
| **Minimal assumptions**      | The engine doesn’t decide how you code          |

---

## Installation

Grab the latest release here:
* **[Releases](https://github.com/YumStudioHQ/MONOE.exe/releases/latest)**

No installer, no launcher — just run it.

---

## Architecture Overview

MONOE.exe is structured around a **runtime bridge**, not an editor.

```
+----------------------+
|      Lua Scripts     |
|  (Gameplay / Logic)  |
+----------+-----------+
           ↕
+----------------------+
|      C# Runtime      |
|  Event loop, shell,  |
|  hot reload, bridge  |
+----------+-----------+
           ↕
+----------------------+
|  Native Libraries    |
|   (C / C++ / DLL)    |
+----------------------+
```

* Lua can call **C# methods**, **static methods**, and **native bindings**
* The runtime can inject functions directly into Lua
* The engine owns the loop — Lua owns the logic

---

## Engine Lifecycle

The engine follows an **explicit, event-driven lifecycle**, controlled by Lua.

### Startup

1. **Engine boot**
2. Runtime environment prepared
3. Lua error handler installed
4. Optional filesystem watcher (hot reload)

---

### Project Load

Lua entry file is resolved from:

* CLI argument
* `res/main.lua`
* embedded editor runtime
* fallback `main.lua`

---

### Lifecycle Events

#### `deps()`

Called **before anything else**
Used to declare required C# assemblies:

```lua
function deps()
  return {
    "./MyLibrary.dll"
  }
end
```

Assemblies are loaded *before* `main()` runs.

---

#### `main()`

Project entry point.

* Register events
* Load scripts
* Initialize systems

Return values from `main()` are passed to `ready`.

---

#### `@load`

Triggered after `main()`
Used to load secondary Lua files.

Supports hot reload automatically.

---

#### `ready(...)`

Called once **everything is loaded**.

This is where you initialize state that depends on:

* loaded scripts
* loaded libraries
* registered callbacks

---

### Runtime Loop

| Event            | Description                     |
| ---------------- | ------------------------------- |
| `process(delta)` | Called every frame              |
| `physics(delta)` | Physics step                    |
| `input`          | Input event                     |
| `onfree`         | Fired every 0.5s (GC / cleanup) |
| `onexit`         | Engine shutdown                 |

---

## Basic Lua Example

```lua
local event = require("monoelib.event")

function deps()
  return { "./MyGame.dll" }
end

function main()
  print("Game started")
end

event.subscribe("process", function(delta)
  -- frame update
end)

event.subscribe("physics", function(delta)
  -- physics update
end)

event.subscribe("onexit", function()
  print("Goodbye")
end)
```

---

## Hot Reloading

Hot reload is **runtime-safe** and optional.

* Any `.lua` file change triggers `@hot`
* `project.lua` change triggers a full reload
* Errors lock the runtime instead of crashing it
* Reloading clears the lock automatically

This allows:

* live iteration
* experimentation
* recovery from Lua errors

---

## Runtime Shell

If enabled, MONOE.exe starts a **background shell thread**.

You can:

* Execute Lua code
* Reload the project
* Inspect tables
* View memory statistics

### Example Commands

| Command         | Action               |
| --------------- | -------------------- |
| `:reload`       | Reload project       |
| `:dump <table>` | Print Lua table      |
| `:stats`        | Runtime memory stats |

Shell commands can also be passed via CLI using `-c`.

---

## Error Handling & Critical State

* Lua errors **do not crash the engine**
* Instead, the runtime enters a **locked (critical) state**
* Game logic stops
* Shell remains available
* Hot reload clears the lock safely

This makes MONOE.exe suitable for:

* live development
* modding
* runtime scripting tools

---

## Advanced Runtime Features

### Garbage Collection

* `onfree` event fired every **0.5 seconds**
* Manual one-shot cleanup supported
* Explicit GC queue for finalizers

---

### Dynamic Extensions

From Lua you can:

* Load C# assemblies
* Call instance or static methods
* Push callbacks into Lua
* Extend the runtime dynamically

---

## Documentation

* Getting Started
* Lua API Reference
* Runtime Shell
* Engine Architecture
* Meta Runtime Internals

(See `docs/`)

---

## Final Words

MONOE.exe exists because **runtime freedom is fun**.

You can:

* break it
* bend it
* abuse it
* or build something real with it

There are no rules — only what *you* decide to implement.
