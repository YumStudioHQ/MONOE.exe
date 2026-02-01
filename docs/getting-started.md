# Getting Started with **MONOE.exe v3**

Welcome to **MONOE.exe v3** !!
MONOE.exe is a Godot-powered runtime that lets you build applications and games using **Lua**, with a strong C# core handling lifecycle, threading, hot-reload, and system integration.

Version 3 introduces a **fully event-driven runtime**, a new boot process, and a much tighter Lua ↔ engine bridge.

---

## Core Concepts (v3 mindset)

Before writing code, here’s how v3 thinks:

### 1. Lua is event-driven

You **do not** implement engine loops directly.

Instead, Lua reacts to **engine events**:

* `@load`
* `@ready`
* `@process`
* `@physics`
* `@input`
* `@hot`
* `@cleanup`
* `@onexit`

### 2. The engine owns the lifecycle

* Godot runs the main loop
* C# marshals execution onto the main thread
* Lua code **cannot break thread safety**

### 3. Hot reload is a first-class feature

Lua files can opt-in to hot reload using `@hot` events.

### 4. The shell is part of the runtime

You can execute:

* Lua code
* Engine commands
* Debug utilities
  …while the app is running.

---

## Project Structure

A minimal MONOE.exe v3 project:

```
my-game/
├─ res/
│  ├─ main.lua (can be empty, the `main()` function is called once the engine is ready)
│  ├─ project.lua (now optional, and should contain settings code, instead of game logic)
│  └─ game.lua
```

---

## `main.lua` – Entry Point

Every MONOE.exe project starts from **`main.lua`**.

```lua
-- main.lua

-- Optional: request dependencies (DLLs / managed libs)
function deps()
  return {}
end

-- Main entry point
function main()
  print("MONOE v3 runtime booted!")
  return {}
end
```

### What happens under the hood

1. Engine loads `main.lua`
2. Calls `deps()`
3. Loads requested libraries
4. Injects runtime globals
5. Calls `main()`
6. Fires lifecycle events

---

## Runtime Globals (Injected)

MONOE.exe injects a `monoe` table automatically.

### `monoe.info`

```lua
print(monoe.info.os.name)
print(monoe.info.runtime.version)
```

Available sections:

* `monoe.info.os`
* `monoe.info.runtime`

Including:

* OS name / version
* argv
* process ID
* exit function

```lua
monoe.info.os.exit(0)
```

---

## Events in v3

Events are emitted by the engine using **string-based names**.

### Common events

| Event      | When it fires                   |
| ---------- | ------------------------------- |
| `@load`    | After scripts are loaded        |
| `@ready`   | After everything is initialized |
| `@process` | Every frame                     |
| `@physics` | Every physics tick              |
| `@input`   | On input                        |
| `@hot`     | When a Lua file is hot-reloaded |
| `@cleanup` | Periodic cleanup                |
| `@collect` | After each frame                |
| `@onexit`  | Application is exiting          |

---

## Subscribing to Events

MONOE.exe exposes an event system through `monoe.event`.

### Basic subscription

```lua
monoe.event.subscribe("@ready", function()
  print("App is ready!")
end)
```

### Per-frame update

```lua
monoe.event.subscribe("@process", function(delta)
  -- delta is frame time in seconds
end)
```

### Physics update

```lua
monoe.event.subscribe("@physics", function(delta)
end)
```

---

## Example: Minimal Game Loop

```lua
-- game.lua

local x = 0

monoe.event.subscribe("@ready", function()
  print("Game ready")
end)

monoe.event.subscribe("@process", function(delta)
  x = x + 100 * delta
  print("x =", x)
end)
```

Load it from `main.lua`:

```lua
-- main.lua

function main()
  -- This would be a very manual way,
  dofile("res/game.lua")
  -- Instead, this could be sometimes better:
  engine.qualify(require('res.game.lua'), false) -- Or true, if it is a class that needs a self.
end
```

---

## Hot Reloading (v3)

Hot reload is automatic when enabled.

### Reacting to reloads

```lua
monoe.event.subscribe("@hot", function(path)
  print("Reloaded:", path)
end)
```

### Typical pattern

```lua
local function setup()
  print("setup called")
end

monoe.event.subscribe("@load", setup)
monoe.event.subscribe("@hot", function(path)
  if path:endswith("game.lua") then
    setup()
  end
end)
```

---

## Cleanup & Resource Management

Two cleanup phases exist:

### `@collect`

* Fired after physics updates
* Lightweight cleanup

### `@cleanup`

* Fired periodically
* Good for deferred destruction

```lua
monoe.event.subscribe("@cleanup", function()
  -- free resources
end)
```

---

## Exiting the Application

From Lua:

```lua
monoe.info.os.exit(0)
```

From the shell:

```text
:exit
```

On window close, the engine triggers:

```lua
@onexit
```

---

## The Runtime Shell

When enabled, MONOE.exe starts an interactive shell.

### Lua execution

```text
print("hello world")
```

### Engine commands

```text
:reload
:exit
```

Commands are discovered via C# attributes and run safely on the main thread.

---

## Command Line Flags (Common)

| Flag             | Description                    |
| ---------------- | ------------------------------ |
| `-dev`           | Enable dev mode |
| `-nr`            | No Run (exits directly) |
| `-no-shell`      | Disable shell |
| `-no-hot-reload` | Disable hot reload |
| `-diagnostics`   | Enable diagnostics |
| `-silent`        | Reduce output |

---

## Philosophy of v3

* **Lua is userland**
* **C# is kernel**
* **Godot is hardware**
* Everything runs on the **main thread**
* Crashes lock the runtime instead of corrupting state
* Hot reload is safe by default

---

## Migration Notes

| v2                     | v3               |
| ---------------------- | ---------------- |
| `project.lua`          | boot file is now `main.lua`. The `project.lua` file is designed for project settings now.|
| `engine.qualify`       |  changed        |
| File-based lifecycle | Event-based      |
| Named events           | `@event` strings |
| Manual reload logic    | Built-in |
