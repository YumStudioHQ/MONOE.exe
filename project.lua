local engine = require('libraries.engine')

function main()
  engine.load('player', 'game.logic.player')
  engine.load('cow', 'game.logic.cow')
end

function deps()end