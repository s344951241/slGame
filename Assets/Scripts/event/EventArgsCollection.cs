using System;
using System.Collections.Generic;

class EventArgsCollection
{
    
}

public class SimpleEventArgs : EventArgs
{
    public string Name { get; private set; }

    public SimpleEventArgs(string name) : base()
    {
        this.Name = name;
    }
}

