local engine = require('libraries.engine')
local window = require('libraries.io.window')
local tilemap = require('libraries.types.tilemap')

function main()
  engine.debug = true

  engine.load('player', 'game.logic.player')
  engine.load('cow', 'game.logic.cow')

end

function deps()end