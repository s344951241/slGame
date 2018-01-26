---lua 入口

Main= {}

local require = require

require "Driver.CSharpCall"
require "Driver.LuaStart"

pb = require "protobuf"

local CS = CS
local EventManager = CS.EventManager
local EventConst = CS.EventConst
local SimpleEventArgs = CS.SimpleEventArgs
local LuaProtoTest = CS.LuaProtoTest

function Main:Start()
	print("------------lua启动ing")
	LuaStart:Start()
	LuaStart:TestMethod("11111")
	EventManager.Instance:addEvent(EventConst.EVENT_TEST2,self.EventCall)
	EventManager.Instance:invokeEvent(EventConst.EVENT_TEST,SimpleEventArgs("11111"),self)

	local path = "Assets/Resources/GameAssets/Lua/Proto/Test.pb"
	local file = io.open(path,"rb")
	local buffer = file:read "*a"
	file:close()
	pb.register(buffer)
	
	local test = {
		id = 123456,
		name = "vv"
	}
	local code = pb.encode("ProtoBuf.TestProto",test)
	LuaProtoTest.Instance:getLuaCode(code)
	local code2 = pb.decode("ProtoBuf.TestProto",code)
	print(code2.name)
end

function Main.EventCall(sender,e)
	print(sender,"触发了lua定义的事件",e.Name)
end	

Main:Start()

return Main