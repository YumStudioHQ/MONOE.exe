local sprite = require('libraries.types.sprite')
local window = require('libraries.io.window')
local entity = require('libraries.types.entity')
local keyboard = require('libraries.io.keyboard')
local event = require('libraries.event')
local image = require('libraries.image')
local animation = require('libraries.types.animation')
local query = require('libraries.linq')
local engine = require('libraries.engine')

-- todo: screen..

function main()
  engine.load('player', 'game.logic.player')
end

function deps()end