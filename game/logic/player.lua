local image = require('libraries.image')
local window = require('libraries.io.window')
local keyboard = require('libraries.io.keyboard')

local player = require('game.data.player_data')

function player.ready()
  local idle = image.new('./game/assets/player/idle.png')
  local walk = image.new('./game/assets/player/walk.png')

  player.animation:load('idle_side', idle, 80, 80, 0, 3, 0, 0)
  player.animation:load('idle_down', idle, 80, 80, 0, 3, 1, 1)
  player.animation:load('idle_up', idle, 80, 80, 0, 3, 2, 2)

  player.animation:load('walk_side', walk, 80, 80, 0, 7, 0, 0)
  player.animation:load('walk_down', walk, 80, 80, 0, 7, 1, 1)
  player.animation:load('walk_up', walk, 80, 80, 0, 7, 2, 2)

  player.root:scale(10, 10)
  player.root:position(window.center())

  window.attach(player)
  player.animation:play('idle_down')
end

function player.animate()
  player.animation:play(player.state .. '_' .. player.direction)
  player.animation:flip('H', player.flip)
end

function player.process(delta)
  player.flip = false
  player.state = 'idle'
  if keyboard.down('ui_up') then
    player.root:move(0, -(player.speed * delta))
    player.direction = 'up'
    player.state = 'walk'
  elseif keyboard.down('ui_down') then
    player.root:move(0, player.speed * delta)
    player.direction = 'down'
    player.state = 'walk'
  end
  if keyboard.down('ui_right') then
    player.root:move(player.speed * delta, 0)
    player.direction = 'side'
    player.state = 'walk'
  elseif keyboard.down('ui_left') then
    player.root:move(-(player.speed * delta), 0)
    player.direction = 'side'
    player.flip = true
    player.state = 'walk'
  end

  player.animate()
end

return player