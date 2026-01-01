monoe = monoe or {}

local function is_node(obj)
  return type(obj) == "table" and type(obj.uid) == "number"
end

function monoe.attach_tree(root, t)
  for _, v in pairs(t) do
    if is_node(v) and v ~= root then
      root:attach(v)
    elseif type(v) == "table" then
      monoe.attach_tree(root, v)
    end
  end
end

_G.monoe = monoe
return monoe