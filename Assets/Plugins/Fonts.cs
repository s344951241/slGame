using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fonts  {

    public readonly static string FONT_FZY4JW_PATH = "GameAssets/UI/Font/FZY4JW";
    private static Dictionary<string, Font> _fontDic = new Dictionary<string, Font>();

    public static Font font_FZY4JW
    {
        get {
            if (!_fontDic.ContainsKey(FONT_FZY4JW_PATH))
            {
                var font = Resources.Load<Font>(FONT_FZY4JW_PATH);
                if (font == null)
                    return null;
                _fontDic.Add(FONT_FZY4JW_PATH, font);
            }
            return _fontDic[FONT_FZY4JW_PATH];
        }
    }
}
