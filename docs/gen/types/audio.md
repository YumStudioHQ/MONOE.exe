# audio.lua

Source: `libraries/types/audio.lua`

## monoe.audio.new

@class monoe.audio
@field uid integer Unique engine-side identifier for the audio object
Represents an audio player capable of loading, playing, and managing sound files.
Creates a new `monoe.audio` object.
If a `source` is provided, the audio file will be loaded automatically.
@param source string? Optional file path to load immediately
@return monoe.audio Newly created audio object

| Parameter | Type |
|-----------|------|
| `source` | string? Optional file path to load immediately |

**Returns:** monoe.audio Newly created audio object

---

## monoe.audio:load

@param source string File path of the audio to load

| Parameter | Type |
|-----------|------|
| `source` | string File path of the audio to load |

---

## monoe.audio:play

@param at number? Optional starting position in seconds
@param loop boolean? Whether the audio should loop (default: false)

| Parameter | Type |
|-----------|------|
| `at` | number? Optional starting position in seconds |
| `loop` | boolean? Whether the audio should loop (default: false) |

---

## monoe.audio:stop

---

## monoe.audio:length

@return number

**Returns:** number

---

## monoe.audio:finished

@return string Event name

**Returns:** string Event name

---

## monoe.audio:free

---

