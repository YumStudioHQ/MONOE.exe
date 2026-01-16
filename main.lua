local engine = require('monoelib.engine')
local text = require('monoelib.ui.text')
local mainwin = require('monoelib.io.mainwin')
local fswatcher = require('monoelib.system.fswatcher')

function main()
  local watcher = fswatcher.new('./', '*.*')
  watcher:set('changed', false, function (...)
    print('changed? idk!', ...)
  end)
end
