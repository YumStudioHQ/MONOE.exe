local animation = require('libraries.types.animation')
local entity = require('libraries.types.entity')
local image = require('libraries.image')
local window = require('libraries.io.window')
local event = require('libraries.event')
local keyboard = require('libraries.io.keyboard')

local player = {
  root = entity.new(),
  animation = animation.new(),
  speed = 300,
}

function player.ready()
  local idle = image.new('./assets/player/idle.png')
  player.animation:load('default', idle, 80, 80, 0, 3, 0, 0)
  player.root:scale(10, 10)
  player.root:position(window.center())

  window.attach(player)
  player.animation:play('default')
end

function player.update(delta)
  if keyboard.down('ui_up') then
    player.root:move(0, -(player.speed * delta))
  elseif keyboard.down('ui_down') then
      player.root:move(0, player.speed * delta)
  end
  if keyboard.down('ui_right') then
    player.root:move(player.speed * delta, 0)
  elseif keyboard.down('ui_left') then
    player.root:move(-(player.speed * delta), 0)
  end
end

event.once('ready', player.ready)
event.subscribe('process', player.update)

return player