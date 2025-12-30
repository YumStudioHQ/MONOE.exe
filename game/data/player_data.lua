local animation = require('libraries.types.animation')
local entity = require('libraries.types.entity')
local cam = require('libraries.types.cam')
local shape = require('libraries.types.shape')

return {
  root = entity.new(),
  animation = animation.new(),
  cam = cam.new(),
  shape = shape.new('capsule:20x40'),
  speed = 300,
  ---@type 'down'|'up'|'side'
  direction = 'down',
  flip = false,
  ---@type 'idle'|'walk'
  state = 'idle'
}