local animation = require('libraries.types.animation')
local entity = require('libraries.types.entity')
local shape = require('libraries.types.shape')

return {
  root = entity.new(),
  animation = animation.new(),
  speed = 300,
  shape = shape.new('capsule:16x32'),
  ---@type 'down'|'up'|'side'
  direction = 'down',
  flip = false,
  ---@type 'idle'|'walk'
  state = 'idle'
}