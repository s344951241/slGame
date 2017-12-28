using UnityEngine;
using System.Collections;

public class GameConfig  {
    public static readonly string GAME_NAME = "wxr";

    public static bool isStart = false;

    public static int WAIT_TIME = 5;

    public static int punish = 0;

    public static uint serverID = 0;

    public static string portID = string.Empty;

    public static string serverIP = string.Empty;

    public static int gid = 1;

    public static string serverName = string.Empty;

    public static bool isGameInit = false;

    public static string deviceName = string.Empty;

    public static string deviceUniqueIdentifier = string.Empty;

    public static string programVersion = "0.0.0.1";

    public static string platformName = string.Empty;

    public static string platformId;

    public static string unityVersion = string.Empty;

    public static bool isAbLoading = false;
    public static bool isUnityEditor = true;
    public static void Init()
    {
        GameConfig.deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        GameConfig.deviceName = SystemInfo.deviceName;
        GameConfig.isGameInit = false;
        GameConfig.unityVersion = "Unity " + Application.unityVersion;
        GameConfig.serverName = WWW.UnEscapeURL(PlayerPrefs.GetString("name"));
        GameConfig.serverIP = WWW.UnEscapeURL(PlayerPrefs.GetString("ip"));
        GameConfig.portID = WWW.UnEscapeURL(PlayerPrefs.GetString("port"));
        GameConfig.serverID = (uint)PlayerPrefs.GetInt("id");
    }
}
