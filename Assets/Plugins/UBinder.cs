using System;
using System.Reflection;
using System.Runtime.Serialization;

public class UBinder : SerializationBinder
{
    //
    // Methods
    //
    public override Type BindToType(string assemblyName, string typeName)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        return executingAssembly.GetType(typeName);
    }
}
