local image = require('libraries.image')
local window = require('libraries.io.window')
local keyboard = require('libraries.io.keyboard')

local cow = require('game.data.cow_data')
local global = require('game.data.glob')

function cow.ready()
  local idle = image.new('game/assets/cow/Cow.png')
  cow.animation:load('idle', idle, 32, 32, 0, 3, 0, 0)
  cow.animation:play('idle')
  cow.root:scale(global.scale, global.scale)
  window.attach(cow)
end

return cow