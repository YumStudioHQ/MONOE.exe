# monoe.system.fswatcher

Observes changes of files in a directory, and fires events when changes happen, built on top of System.IO.FileSystemWatcher in C#, it runs on another thread, but fires events on the main thread, meaning that the code is totaly safe.

## Properties

- **uid** (`integer`): Internal unique indentifier.
- **event** (`string`): Event's base name. This string is random, and unpredictible. You can add '_changed', '_created', ... at the end of this in order to get the event's name. But prefer using the given method, as it will be more stable through versions.

## monoe.system.fswatcher.new

Creates a new file system watcher.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `path` | `string` | The path of the directory that should be watched. |
| `filter` | `string` | File filters. Use '*.*' to watch all files, and '*.txt' to watch all files that ends with '.txt' extension. |

### Returns

- `monoe.system.fswatcher`

---

## monoe.system.fswatcher:set

Calls the callback when the event is fired.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `once` | `boolean` | If the callback should be called once. |
| `func` | `function<string>` | The callback. |

---

## monoe.system.fswatcher:free

Frees the resource.

---

