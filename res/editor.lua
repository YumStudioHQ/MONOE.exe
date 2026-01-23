local tilemap = require('monoelib.types.tilemap')
local input = require('monoelib.io.input')
local image = require('monoelib.types.image')
local mainwin = require('monoelib.io.mainwin')

local editor = {
  tilemap = tilemap.new(),
  palette = {
    name = 'default',
    color = { x = 2, y = 0 }
  }
}

function editor.ready()
  local img = image.new('res/assets/base-palette.png')
  editor.tilemap:load('default', img, 10, 10)
  mainwin.attach(editor.tilemap)
end

function editor.process()
  if input.mouse.down("left") then
    local x, y = editor.tilemap:tolocal(input.mouse.position())
    editor.tilemap:place(x, y, editor.palette.name, editor.palette.color.x, editor.palette.color.y)
  end
end

return editor