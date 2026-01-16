local engine = require('monoelib.engine')
local text = require('monoelib.ui.text')
local mainwin = require('monoelib.io.mainwin')

function main()
  print(engine.import('monoe.exe.Core.Bridge.Types.LibSys.FSWatcher', 'hiii'))
end
