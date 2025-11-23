local monoe = monoe or {}

---@class monoe.runtime
---@field libs table<string>
---@field physics_process function
---@field process function
---@field exit function
monoe.runtime = {}
monoe.runtime.__index = monoe.runtime
monoe.runtime.libs = {}

---adds a required library
---@param library string
function _Mrtrequire(library)
  if type(library) == "string" then
    table.insert(monoe.runtime.libs, library)
  end
end

--#region Engine Interface

function _Mrtlibs()
  if type(monoe.runtime.libs) == "nil" then
    return {}
  end
  return table.unpack(monoe.runtime.libs)
end

_Mrtphysics_process = function (delta) end
_Mrtprocess = function (delta) end
_Mrtexit = function () end
_Mrtready = function () end

--#endregion

_G.monoe = monoe
_G.monoe.runtime = monoe.runtime
return monoe