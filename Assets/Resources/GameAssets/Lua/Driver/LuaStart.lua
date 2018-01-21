local  require = require

local CS = CS
local ForLua = CS.ClassForLua 

LuaStart = {}

function LuaStart:Start()
	print("lua已启动")
end

function LuaStart:TestMethod(str)
	ForLua:TheMethod(str)
end