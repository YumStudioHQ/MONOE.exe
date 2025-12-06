-- EXAMPLE
local _ = require("monoe.monoe")

function main()
  print("Hello, world! -- MONOE")

  return 0
end

function _physics_process(delta)
  monoe.staticcall("monoe.exe.Source.Core.Engine.MonoeEngine", "TESTAPP")
end