local engine = require('libraries.engine')

monoe.editor = {}
local editor = monoe.editor -- Such a lazy dev...

function editor.ready()
  print('Hi ! Welcome to the monoe.editor editor~')
end

function deps()end

engine.qualify(editor)