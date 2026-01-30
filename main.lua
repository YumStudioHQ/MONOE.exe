local anim = require('monoelib.types.animation')
local image = require('monoelib.types.image')
local event = require('monoelib.event')

function main()
  event.subscribe('process', process)
end

function process(delta)

end