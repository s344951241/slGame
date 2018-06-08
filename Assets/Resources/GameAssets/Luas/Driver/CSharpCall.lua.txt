

CSharpCall = {}

function  CSharpCall.StartLuaModule( )
	-- body
end

function  CSharpCall.DisposeLuaModule( )
	-- body
end

function CSharpCall.OnGUI()
	-- body
end

function CSharpCall.Update(deltaTime)
	-- body
	--print("doUpdate"..tostring(deltaTime))
end

function CSharpCall.LateUpdate(deltaTime)
	-- body
end

function CSharpCall.FixUpdate(fixedDeltaTime)
	-- body
end

function CSharpCall.RecivBytes(id,bytes)
	LuaNet:RecvByteFromCSharp(id,bytes)
end