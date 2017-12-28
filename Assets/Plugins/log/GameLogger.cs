using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GameLogger:Singleton<GameLogger>{

    private FileStream mFile;
    private BinaryWriter mFileWriter;

    DateTime now;
    string dir;

    public GameLogger()
    {
        now = DateTime.Now;
        dir = URL.localCachePath + "logs/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public void LogError(string msg)
    {
        string[] formatStrs = new string[] { "[EOORO:", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), "]", msg, "\r\n" };
        mFile = new FileStream(dir + "/" + GameConfig.GAME_NAME + "_Log_" + now.ToString("yyyy-MM-dd") + ".txt", FileMode.Append, FileAccess.Write);
        mFileWriter = new BinaryWriter(mFile);
        mFileWriter.Write(string.Concat(formatStrs).ToCharArray());
        mFileWriter.Close();
        mFile.Close();
    }
}
