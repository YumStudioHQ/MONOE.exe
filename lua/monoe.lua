local _ = require("lua._monoe")
local utils = require("lua.utils")

_G.monoe = monoe
_G.monoe.utils = utils
_G.monoe.binstream = utils.binstream
return monoe
