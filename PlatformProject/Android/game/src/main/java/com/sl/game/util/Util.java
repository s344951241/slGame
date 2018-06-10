package com.sl.game.util;
import com.sl.game.config.Config;
import com.unity3d.player.UnityPlayer;

public class Util {
    public static void CallUnity(String method,String message)
    {
        UnityPlayer.UnitySendMessage(Config.UNITY_GAME,method,message);
    }
}
