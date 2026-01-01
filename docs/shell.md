# monoe.exe Shell

`monoe.exe` comes with a **built-in shell** for live project interaction, Lua execution, and engine management. It’s a lightweight REPL (Read-Eval-Print Loop) with powerful built-in commands for developers.

## Features

* Execute Lua code on the fly.
* Run built-in C# commands directly from the shell.
* Inspect Lua tables, C# objects, and assemblies.
* Manage your project: reload, compile, create new projects, copy monoelib.
* Thread-safe execution and engine locking.

## Built-in Commands

All commands can be executed by prefixing them with `:` in the shell. For example:

```text
monoe> :reload
monoe> :help stats
```

Here’s a quick list of what’s available:

| Command      | Description                                       |
| ------------ | ------------------------------------------------- |
| `dump`       | Dumps a Lua table (default: `_G`)                 |
| `reload`     | Reloads the whole project                         |
| `lock`       | Locks or unlocks the engine main loop             |
| `inspect`    | Inspects a Lua value                              |
| `sleep`      | Pauses the shell for N milliseconds               |
| `stats`      | Prints GC and memory statistics                   |
| `help`       | Lists all built-ins or shows details of a command |
| `clear`      | Clears the console                                |
| `exit`       | Quits the engine                                  |
| `emit`       | Emits an event with arguments                     |
| `object`     | Shows detailed info about a C# object by UID      |
| `assemblies` | Lists loaded assemblies with filters              |
| `compile`    | Compiles the whole project                        |
| `copylibs`   | Copies engine libraries to a specified path       |
| `newproject` | Creates a new project at the specified path       |

> Tip: Type `:help <command>` for more info about arguments and usage.

## Example Usage

```text
monoe> :dump myTable
monoe> :inspect someVariable
monoe> :emit myEvent 123 "hello" true
monoe> :assemblies contains:Core
```

You can also run **raw Lua code** without `:`:

```text
monoe> print("Hello from Lua!")
```

## How it Works

1. The shell uses `BuiltInAttribute` to mark static C# methods as shell commands.
2. Commands are loaded dynamically into a dictionary for quick lookup.
3. User input is parsed; commands run in the engine main thread to avoid race conditions.
4. Lua code is executed directly using the engine’s runtime.

## Developer Notes

* The shell supports **thread-safe execution** — commands don’t block the engine.
* All C# objects can be inspected using their **unique UID**.
* Built-in commands can be extended easily by adding new static methods and annotating them with `BuiltInAttribute`.
