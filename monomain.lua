-- EXAMPLE
local _ = require("monoe.monoe")

function main()
  print("Hello, world! -- MONOE")

  -- This function will get called each frame.
  _Mrtprocess = function (delta)
    print("called", delta)
  end

  -- This one is caled when exiting the application
  _Mrtexit = function ()
    print("Bye bye!")
  end

  -- This one... For physic processes.
  _Mrtphysics_process = function (delta) 
  end

  -- This one will get called after loading libraries, and pushing types in the 
  -- lua state.
  _Mrtready = function () end

  -- By returning something else than 0, the engine will crash.
  -- You can request C# library loading by calling this functions:
  _Mrtrequire("name")
  return 0
end
