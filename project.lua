local sprite = require('libraries.types.sprite')
local event = require('libraries.event')

---@type monoe.sprite
local image

function main()
  image = sprite.new('./icon.png')
  image:render()
  image:move(350, 250)

end

function process()
end

function deps() end
