local animation = require('libraries.types.animation')
local entity = require('libraries.types.entity')

return {
  root = entity.new(),
  animation = animation.new(),
  speed = 300,
  ---@type 'down'|'up'|'side'
  direction = 'down',
  flip = false,
  ---@type 'idle'|'walk'
  state = 'idle'
}