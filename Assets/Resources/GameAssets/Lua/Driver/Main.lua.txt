---lua 入口

Main= {}

local require = require
require "Net.protobuf"
require "Driver.CSharpCall"
require "Driver.LuaStart"
require "Net.LuaNet"

local CS = CS
local EventManager = CS.EventManager
local EventConst = CS.EventConst
local SimpleEventArgs = CS.SimpleEventArgs
local LuaProtoTest = CS.LuaProtoTest
local ProtoManager = CS.ProtoManager

function Main:Start()
	print("------------lua启动ing")
	LuaStart:Start()
	LuaStart:TestMethod("11111")
	EventManager.Instance:addEvent(EventConst.EVENT_TEST2,self.EventCall)
	EventManager.Instance:invokeEvent(EventConst.EVENT_TEST,SimpleEventArgs("11111"),self)
	
	
	LuaNet:Init()
	
	--[[local test = {
		id = 123456,
		name = "vv"
	}
	local code = pb.encode("ProtoBuf.TestProto",test)
	LuaProtoTest.Instance:getLuaCode(code)
	local code2 = pb.decode("ProtoBuf.TestProto",code)
	print(code2.name)--]]
end

function Main.EventCall(sender,e)
	print(sender,"触发了lua定义的事件",e.Name)
end	

Main:Start()

return Main