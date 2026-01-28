local anim = require('monoelib.types.animation')
local image = require('monoelib.types.image')

function main()
  for i = 1, 1000, 1 do
    local img = image.new('/Users/wys/Documents/MONOE.exe/icon.png')
    anim:load('idle_' .. tostring(i), img, 4, 4, 0, 8, 2, 3)
    anim:free()
    img:free()
  end
end