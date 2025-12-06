---@diagnostic disable: lowercase-global
local monoe = monoe or {}

---@class monoe.runtime
---@field _ready function
---@field _physics_process function
---@field _process function
---@field _exit function
monoe.runtime = {}

_physics_process = function (delta) end
_process = function (delta) end
_exit = function () end
_ready = function () end

_G.monoe = monoe
_G.monoe.runtime = monoe.runtime
return monoe