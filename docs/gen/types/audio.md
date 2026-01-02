# monoe.audio

Represents an audio player capable of loading, playing, and managing sound files.

## Properties

- **uid** (`integer`): Unique engine-side identifier for the audio object

## monoe.audio.new

Creates a new `monoe.audio` object.
If a `source` is provided, the audio file will be loaded automatically.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `source` | `string` | ? Optional file path to load immediately |

### Returns

- `monoe.audio Newly created audio object`

---

## monoe.audio:load

Loads a sound file (.wav, .mp3) into the audio player.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `source` | `string` | File path of the audio to load |

---

## monoe.audio:play

Plays the audio.

### Parameters

| Name | Type | Description |
|------|------|-------------|
| `at` | `number` | ? Optional starting position in seconds |
| `loop` | `boolean` | ? Whether the audio should loop (default: false) |

---

## monoe.audio:stop

Stops playback of the audio.

---

## monoe.audio:length

Returns the length of the audio stream in seconds.

### Returns

- `number`

---

## monoe.audio:finished

Returns the name of the event triggered when playback finishes.

### Returns

- `string Event name`

---

## monoe.audio:free

Frees the engine-side resources associated with this audio object.

---

