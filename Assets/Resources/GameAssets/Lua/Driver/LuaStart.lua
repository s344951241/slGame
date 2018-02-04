

local CS = CS
local ForLua = CS.ClassForLua 

LuaStart = {}

function LuaStart:Start()
	print("lua已启动")
end

function LuaStart:TestMethod(str)
	ForLua:TheMethod(str)
	ForLua:TheMethodForBack(str,function(param)
		print("来自CSharp的回调参数"..param)
	end)
end