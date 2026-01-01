# MONOE.exe Game Engine

<p align="center">
  <img src="./icon.png" alt="MONOE.exe Logo" width="120">
</p>

## Overview

**MONOE.exe** is a modular, multi-language game engine designed for **maximum flexibility, runtime extensibility, and rapid iteration**.
It allows developers to combine **Lua scripting**, **C# engine logic**, and **native C/C++ libraries**, all in a unified workflow.

MONOE.exe is built for experimentation and serious game development alike — no fixed workflows, no rigid rules, just **powerful runtime control**.

---

## Install
 * [Here!](https://github.com/YumStudioHQ/MONOE.exe/releases/latest)

## Key Features

| Feature                    | Description                                                                        |
| -------------------------- | ---------------------------------------------------------------------------------- |
| **Multi-language support** | Lua scripting with full C# and C/C++ integration                                   |
| **Hot Reload**             | Reload Lua scripts or the entire project at runtime                                |
| **Event-driven Lifecycle** | Lifecycle events: `deps`, `main`, `@load`, `ready`, `process`, `physics`, `onexit` |
| **Runtime Shell**          | Inspect engine state, execute Lua code, reload scripts, trigger garbage collection |
| **Garbage Collection**     | Automatic cleanup every 0.5s with optional manual control                          |
| **Extensible**             | Load custom DLLs, push callbacks, and extend the engine runtime                    |

---

## Architecture

MONOE.exe’s architecture is designed for modularity and runtime flexibility:

```
   +------------------+
   |   Lua Scripts    |
   |  (Gameplay)      |
   +--------+---------+
            ↕
   +------------------+
   |     C# Engine    |
   |  (Core Systems,  |
   |    Hot Reload,   |
   |    Event Loop)   |
   +--------+---------+
            ↕
+------------------------+
|    Native Libraries    |
| (C/C++ via YumEngine)  |
+------------------------+
```

* **Lua scripts** interact directly with C# assemblies and native monoelib.
* The **runtime shell** enables live code execution, hot reloads, and debugging.
* Events drive the engine lifecycle, keeping logic clean and modular.

---

## Engine Lifecycle

The engine follows an **event-driven lifecycle**, controlled by `project.lua`:

1. **`deps()`**
   Load required C# DLLs. Minimal Lua environment available.

2. **Libraries Loaded**
   C# assemblies become accessible to Lua.

3. **`main()`**
   Initialize project, register events, load scripts.

4. **`@load` event**
   Load secondary Lua files. Supports hot-reload.

5. **`ready` event**
   Fired when all scripts and libraries are loaded.

6. **Runtime Loop**

   * `process(delta)` — called every frame
   * `physics(delta)` — physics updates

7. **Exit**
   `onexit` event triggers cleanup.

---

## Basic Lua Example

```lua
local event = require('monoelib.event')

function deps()
  return { "./MyLibrary.dll" }
end

function main()
  print("Game started")
end

event.subscribe('process', function(delta)
  -- update game logic
end)

event.subscribe('physics', function(delta)
  -- update physics
end)

event.subscribe('onexit', function()
  -- cleanup logic
end)
```

---

## Hot Reloading

MONOE.exe supports hot reloading of Lua scripts:

* Changing a `.lua` file emits the `@hot` event.
* Changing `project.lua` triggers a full project reload.
* Hot reload keeps the engine running and prevents crashes.

---

## Runtime Shell

The runtime shell allows:

* Executing Lua code at runtime
* Inspecting globals and tables
* Reloading scripts
* Triggering garbage collection

**[Shell Commands](./docs/shell.md):**

| Command         | Description                |
| --------------- | -------------------------- |
| `:reload`       | Reload the entire project  |
| `:dump <table>` | Print a Lua table          |
| `:stats`        | Display memory statistics  |

---

## Error Handling

* Lua errors trigger a **critical state**
* In critical state, the game logic stops and error handler executes
* Hot reload clears critical state for safe recovery

---

## Advanced Topics

* **Garbage Collection**: `onfree` event every 0.5s, with optional `once()` for single-run cleanup.
* **Main-thread queue**: enqueue actions safely from other threads.
* **Custom DLLs & Callbacks**: extend engine capabilities at runtime.

---

## Philosophy

> **Code your game your way.**

MONOE.exe emphasizes **freedom**, **experimentation**, and **runtime control**.
It’s a tool for developers who want a sandbox that can scale to full-featured games.

---

## See More

* [Getting Started](./docs/getting-started.md)
* [Lua API Reference](./docs/monoe_lua_api.md)
* [Meta Runtime](./docs/meta-runtime.md)
* [Shell Commands](./docs/shell.md)
* [Engine Architecture](./docs/architecture.md)
