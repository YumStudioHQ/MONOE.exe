local text = require('monoelib.ui.text')
local engine = require('monoelib.engine')
local mainwin = require('monoelib.io.mainwin')
local event = require('monoelib.event')
local system = require('monoelib.system.monsys')
local node = require('monoelib.types.node')
local image = require('monoelib.types.image')
local sprite = require('monoelib.types.sprite')
local container = require('monoelib.ui.container')

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

  local vbox = container.new('vbox')
  for i = 1, 100, 1 do
    local l = text.new('hihi', i)
    l:font('!', 20)
    vbox:attach(l.uid)
  end

  vbox:position(mainwin:center())
  mainwin.attach(vbox)

  local arr = system.path.content('./')
  for key, value in pairs(arr) do
    print(key, value)
  end
end

function app.attach(element)
  app.root:attach(element)
end