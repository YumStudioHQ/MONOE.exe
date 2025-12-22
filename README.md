# MONOE.exe Game Engine

## What is MONOE.exe?

MONOE.exe is a small **meta-engine toy**, built using **C#, C, C++, and Lua**.

* **C/C++** powers core libraries through [YumEngine](https://github.com/YumStudioHQ/YumEngine).
* **C#** handles Godot integration.
* **Lua** is available for easy scripting.

In future versions, C/C++ libraries may also become fully usable from Lua!

---

## Why MONOE.exe?

Because you should be able to **code your game your way**. No limits, no fuss—Lua, C#, or any language visible to Lua can be used to build your game.

---

## Philosophy

> Code as you wish. MONOE.exe gives you the freedom to use Lua for quick scripting, or C# for deeper control.

---

## How do I make my first game?

Super simple! Start with a few functions:

```lua
function main() 
  -- Initialization code here
end

function process()
  -- Called every frame
end

function physics()
  -- Update physics if your game uses it
end

function exit()
  -- Save your game or cleanup here
end

function deps()
  -- Load custom libraries (C# DLLs)
end
```

**Tips:**

* `deps()` is called first, before your libraries are loaded. Only Lua's standard libraries and manually imported ones are available at this point.
* When passing data between C# and Lua, you can use **long (integer), double, string, boolean**, and optionally binary data or unique IDs.

---

## Libraries

* MONOE.exe provides **built-in standard libraries**.
* You can also load **any Lua standard library**.
* Need your own types? Write them in C#, compile to `.dll`, and return the file path in `deps()`.

---

> MONOE.exe is about giving you **freedom and flexibility**. Explore, experiment, and create your game your way!
