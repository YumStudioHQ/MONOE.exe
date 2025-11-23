local _ = require("monoe._monoe")
local utils = require("monoe.utils")
local runtime = require("monoe.runtime")

_G.monoe = monoe
_G.monoe.utils = utils
_G.monoe.binstream = utils.binstream
_G.monoe.runtime = runtime
return monoe
