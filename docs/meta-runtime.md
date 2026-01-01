# MONOE.exe — Meta Engine & Runtime Features

### Overview

MONOE.exe isn’t just a Lua scripting engine — it’s a **meta-runtime**.
It allows Lua scripts to interact dynamically with **all loaded C# types and assemblies**, including built-in engine systems and user-loaded monoelib.

This means you can:

* Instantiate C# objects directly from Lua
* Call instance methods and static methods seamlessly
* Hot-reload scripts while preserving state
* Build complex gameplay systems without leaving Lua

Think of it as a **live bridge between Lua and C#**, powered by reflection and a lightweight UID-based object registry.

---

### Core Features

#### 1. **Dynamic Class Loading**

* Lua can request any class via its full type name.
* The engine resolves the type, calls the constructor, and returns a unique ID.
* Lua then uses this ID to call instance methods safely.

#### 2. **Instance & Static Method Calls**

* `monoe.call(uid, method, ...)` — invokes methods on an object instance
* `monoe.staticcall(typeName, method, ...)` — invokes static methods on any class
* Supports Lua-style variable arguments for flexibility

#### 3. **Hot Reload with State Preservation**

* Scripts can be reloaded on-the-fly (`@hot` event)
* Existing objects are updated without losing runtime state
* Deep table updates allow smooth transitions between old and new versions

#### 4. **Flexible Bridge**

* Lua sees a **clean, simple API** (`monoe.import`, `monoe.call`, `monoe.staticcall`)
* Under the hood, C# handles reflection, method resolution, caching, and UID management
* You get **full engine control** from Lua, without breaking memory safety

---

### Example Workflow

```lua
-- Load a custom C# class
local playerUid = monoe.import("Game.Entities.Player")

-- Call instance method
monoe.call(playerUid, "Move", 10, 0)

-- Call static helper
monoe.staticcall("Game.Helpers.MathUtils", "Clamp", 42, 0, 100)
```

---

### Why It Matters

* You can **experiment freely** in Lua without recompiling C# code
* Reflection + UID system ensures **safe cross-language interaction**
* Hot-reload + deep table updates allow **rapid iteration**
* Enables building **tools, gameplay logic, and engine extensions** directly from Lua

---

### Notes

* Extreme flexibility means **you can do almost anything**, but be mindful:
  misusing reflection or bypassing safeguards can crash scripts
* Designed for **development and experimentation**, not sandboxed end-user environments
