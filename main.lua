local text = require('monoelib.ui.text')
local engine = require('monoelib.engine')
local mainwin = require('monoelib.io.mainwin')
local event = require('monoelib.event')
local system = require('monoelib.system.monsys')
local node = require('monoelib.types.node')

local app = {
  ---@type monoe.node
  root = { uid = -1 }
}

function main()
  app.root = node.new()

  mainwin.title('monoe editor (best one hihi)')
  engine.qualify(app)
end

function app.ready()
  local label = text.new('This is the MONOE.exe engine !\n'
    .. 'If you see this, it means that the engine works (yeepee) !\n'
    .. 'You can see the work on Github here: https://github.com/YumStudioHQ/MONOE.exe\n\n'
    .. 'infos:\n'
    .. '* os: ' .. engine.info.os.name .. '@' .. engine.info.os.version .. '\n'
    .. '* engine: ' .. engine.info.runtime.version
  )

  label:font('!', 32)
  app.attach(label)
  local win = system.windows.win.new()
  win:close_request(function ()
    print('nuh uh, never.')
    engine.info.os.exit(1)
  end)
  engine.qualify(win)
end

function app.attach(element)
  app.root:attach(element)
end