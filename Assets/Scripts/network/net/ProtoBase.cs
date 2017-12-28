public class ProtoBase
{

    public byte m_ModId = 0;
    public byte m_MsgId = 0;
    public int m_ProtoId = 0;

    public ProtoBase()
    {

    }

    public virtual void read(ByteArray kByte)
    {

    }

    public virtual void write(ByteArray kByte)
    {
        kByte.WriteUByte(m_ModId);
        kByte.WriteUByte(m_MsgId);
    }
}