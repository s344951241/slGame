using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgHeader {
    public const int FS_LENGTH = 2;
    public const int FS_CHECK = 4;
    public const int FS_MSGID = 4;
    public const int HEADER_SIZE = FS_LENGTH + FS_CHECK + FS_MSGID;

    public ushort Length { get; set; }
    public int Check { get; set; }
    public uint MsgId { set; get; }
}
