local engine = require('monoelib.engine')

monoe = monoe or {}

---@class monoe.audio
---@field uid integer Unique engine-side identifier for the audio object
---Represents an audio player capable of loading, playing, and managing sound files.
monoe.audio = {}
monoe.audio.__index = monoe.audio

local base = 'monoe.exe.Core.Bridge.Types.Audio'

---Creates a new `monoe.audio` object.  
---If a `source` is provided, the audio file will be loaded automatically.
---@param source string? Optional file path to load immediately
---@return monoe.audio Newly created audio object
function monoe.audio.new(source)
  local uid = engine.import(base)

  if uid == -1 then
    error('Failed to create monoe.audio object: invalid UID for ' .. base)
  end

  if type(source) == "string" then
    engine.call(uid, 'Load', source, source:sub(-3))
  end

  return setmetatable({ uid = uid }, monoe.audio)
end

---Loads a sound file (.wav, .mp3) into the audio player.
---@param source string File path of the audio to load
function monoe.audio:load(source)
  engine.call(self.uid, 'Load', source, source:sub(-3))
end

---Plays the audio.
---@param at number? Optional starting position in seconds
---@param loop boolean? Whether the audio should loop (default: false)
function monoe.audio:play(at, loop)
  engine.call(self.uid, 'Play', at, loop or false)
end

---Stops playback of the audio.
function monoe.audio:stop()
  engine.call(self.uid, 'Stop')
end

---Returns the length of the audio stream in seconds.
---@return number
function monoe.audio:length()
  return engine.call(self.uid, 'Length')
end

---Returns the name of the event triggered when playback finishes.
---@return string Event name
function monoe.audio:finished()
  return engine.call(self.uid, 'FinishedEvent')
end

---Frees the engine-side resources associated with this audio object.
function monoe.audio:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.audio = monoe.audio

return monoe.audio
