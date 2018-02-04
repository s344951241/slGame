
local ProtoDict = require "Net.ProtoDict"

local CS = CS
local ProtoManager = CS.ProtoManager

local LuaNet = {}

LuaNet.netFun = {}
function LuaNet:Init()

	for _,value in pairs(ProtoDict.ProtoName) do
		ProtoManager.Instance:loadProto(value,function(bytes)
			protobuf.register(tostring(bytes))
		end)
	end
	
	
end

function LuaNet:AddNetFun(id,fun)
	self.netFun[id] = fun
	ProtoManager.Instance:AddLuaProto(id)
end

function LuaNet:InvokeFun(id,bytes)
	if self.netFun[id]==nil then
		return
	else
		local proto = protobuf.decode(ProtoDict[id].Name,bytes)
		self.netFun[id](proto)
	end
end

function LuaNet:SendProto(name,proto)
	local code = protobuf.encode(name,proto)
	ProtoManager.Instance:SendMsgFromLua(ProtoDict[name].Id,code)
end

function LuaNet:RecvByteFromCSharp(id,bytes)
	self:InvokeFun(id,bytes)
end

_G.LuaNet = LuaNet

