package com.sl.game;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.sl.game.util.Util;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    public void CallUnityMethod(String str)
    {
        Util.CallUnity("UnityMethod",str);
    }
}
