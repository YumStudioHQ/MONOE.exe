local engine = require('libraries.engine')

monoe = monoe or {}

---@class monoe.audio
---@field uid integer
monoe.audio = {}
monoe.audio.__index = monoe.audio

local base = 'monoe.exe.Core.Bridge.Types.Audio'

---creates a new audio player
---@param source string?
function monoe.audio.new(source)
  local uid = engine.import(base)

  if uid == -1 then
    error('got invalid UID when create an instance of ' .. base)
  end

  if type(source) == "string" then
    engine.call(uid, 'Load', source, source:sub(-3))
  end

  return setmetatable({ uid = uid }, monoe.audio)
end

---loads a sound (.wav or .mp3)
---@param source string
function monoe.audio:load(source)
  engine.call(self.uid, 'Load', source, source:sub(-3))
end

---plays the audio
---@param at number?
---@param loop boolean?
function monoe.audio:play(at, loop)
  engine.call(self.uid, 'Play', at, loop or false)
end

function monoe.audio:stop()
  engine.call(self.uid, 'Stop')
end

---returns the size of the stream
---@return number
function monoe.audio:length()
  return engine.call(self.uid, 'Length')
end

---returns the name of a new event 
---@return string
function monoe.audio:finished()
  return engine.call(self.uid, 'FinishedEvent')
end

function monoe.audio:free()
  engine.call(self.uid, 'Free')
end

_G.monoe = monoe
_G.monoe.audio = monoe.audio
return monoe.audio