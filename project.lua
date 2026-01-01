local engine = require('monoelib.engine')
monoe = monoe or {}
monoe.editor = {}
local editor = monoe.editor -- Such a lazy dev...

function editor.ready()
  print('Hi ! Welcome to the monoe.editor editor~')
  -- TODO:
  -- - Project Name
  -- - Symbols????
end

function deps()end

engine.qualify(editor)