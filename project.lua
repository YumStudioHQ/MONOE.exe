local sprite = require('libraries.types.sprite')
local window = require('libraries.io.window')
local entity = require('libraries.types.entity2D')
local keyboard = require('libraries.io.keyboard')

-- TODO: render table!
-- - Change this logo!
-- - Event driven input?

local player = {}

function main()
  window.title('Hello, Teto')
  player.e = entity.new()
  player.sprite = sprite.new('./icon.png')
  player.e:attach(player.sprite)
  window.attach(player.e)
end

function process()
  if keyboard.key_down('S') then
    player.e:move(0, 10)
  elseif keyboard.key_down('Z') then
    player.e:move(0, -10)
  elseif keyboard.key_down('Q') then
    player.e:move(-10, 0)
  elseif keyboard.key_down('D') then
    player.e:move(10, 0)
  end
end

function deps() end