---lua 入口

Main= {}

local require = require

require "Driver.CSharpCall"
require "Driver.LuaStart"

function Main.Start()
	print("------------lua启动ing")
	LuaStart:Start()
	LuaStart:TestMethod("11111")
end

Main.Start()

return Main