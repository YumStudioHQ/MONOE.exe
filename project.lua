local sprite = require('libraries.types.sprite')
local window = require('libraries.io.window')
local entity = require('libraries.types.entity2D')
local keyboard = require('libraries.io.keyboard')

-- TODO: render table!
-- - Event driven input?

local player = {}

function main()
  player = {
    root = entity.new(),
    sprite = sprite.new("./icon.png"),
    skins = {
      hat = sprite.new("./icon.png"),
    }
  }

  window.attach(player)
end

function process()

end

function deps() end